using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Orb");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1f;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.damage = 10;
            projectile.knockBack = 7.5f;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.netImportant = true;
        }

        public float degrees
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        public void Initialize()
        {
            Player player = Main.player[projectile.owner];

            center = new Vector2((player.position.X - projectile.width / 2) + player.width / 2, (player.position.Y - projectile.height / 2) + player.height / 2);

            ProjX = center.X + (float)(radius * Math.Cos(projectile.ai[0]));
            ProjY = center.Y + (float)(radius * Math.Sin(projectile.ai[0]));

            startAngle = (float)Math.Atan2(center.Y - ProjY, center.X - ProjX);

            projectile.position = center;
        }
        bool init = false;
        bool target = false;
        int npcTarget = 0, oldNpcTarget;
        int ticks = 15;
        int timer;
        int dustType;
        float ProjX, ProjY;
        float startAngle, npcAngle;
        float radius = 16f;
        const float radians = 0.017f;
        Vector2 center;
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            Player player = Main.player[projectile.owner];

            float Angle2 = (float)Math.Atan2(player.position.Y - projectile.position.Y, player.position.X - projectile.position.X);
            projectile.rotation = Angle2 + (radians * -90f);

            if (ticks > 0)
            {
                ticks--;
                projectile.position += Distance(null, startAngle, 8f);
            }

            if (ticks == 0)
            {
                if (!target)
                {
                    center = new Vector2((player.position.X - projectile.width / 2) + player.width / 2, (player.position.Y - projectile.height / 2) + player.height / 2);
                    radius = 128f;

                    degrees += radians * 3f;
                    projectile.position.X = center.X + (float)(radius * Math.Cos(degrees));
                    projectile.position.Y = center.Y + (float)(radius * Math.Sin(degrees));
                }
                foreach (NPC n in Main.npc)
                {
                    if((!target && npcTarget == 0f) && n.active && !n.friendly && !n.dontTakeDamage && !n.immortal && n.target == player.whoAmI && ((n.lifeMax >= 50 && (Main.expertMode || Main.hardMode)) || (n.lifeMax >= 15 && !Main.expertMode && !Main.hardMode)))
                    {
                        if (Vector2.Distance(n.position - projectile.position, Vector2.Zero) < 256f)
                        {
                            oldNpcTarget = npcTarget;
                            npcTarget = n.whoAmI;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    NPC nme = Main.npc[npcTarget];
                    float npcAngle = (float)Math.Atan2(nme.position.Y - projectile.position.Y, nme.position.X - projectile.position.X);

                    projectile.velocity = Distance(null, npcAngle, 16f);

                    int direction = 0;
                    if (projectile.velocity.X < 0)
                        direction = -1;
                    else direction = 1;
                    if(projectile.Hitbox.Intersects(nme.Hitbox))
                    {
                        nme.StrikeNPC(projectile.damage, projectile.knockBack, direction, false, false, false);
                        projectile.Kill();
                    }

                    if (!nme.active || nme.life <= 0)
                    {
                        target = false;
                    } 
                }
            }
            timer++;
            dustType = mod.DustType("cinnabar_dust");
            if (timer % 6 == 0)
            {
                int orbDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, Color.White, 1f);
                Main.dust[orbDust].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            dustType = mod.DustType("cinnabar_dust");
            for (int k = 0; k < 6; k++)
            {
                int Dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, Color.White, 2f);
                int Dust2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 2f);
            }
        }

        public Vector2 Distance(Player player, float Angle, float Radius)
        {
            float VelocityX = (float)(Radius * Math.Cos(Angle));
            float VelocityY = (float)(Radius * Math.Sin(Angle));

            return new Vector2(VelocityX, VelocityY);
        }
    }
}
