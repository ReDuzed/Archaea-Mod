using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_spore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Spore");
            Main.projFrames[projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.scale = 1f;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.damage = 10;
            projectile.knockBack = 0f;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        bool spawnOK = false;
        int buffer = 256;
        const int startHeight = 640;
        int x, y;
        public void Initialize()
        {
            maxTime = projectile.timeLeft;

            Player player = Main.player[projectile.owner];

            foreach (NPC n in Main.npc)
            {
                if (!target && n.active && !n.friendly && !n.dontTakeDamage && !n.immortal && n.target == player.whoAmI && ((n.lifeMax >= 10 && !Main.expertMode) || (n.lifeMax >= 30 && (Main.expertMode || Main.hardMode))))
                {
                    npcTarget = n.whoAmI;
                    target = true;
                }
            }
            if (target && Vector2.Distance(player.position - Main.npc[npcTarget].position, Vector2.Zero) <= 512)
            {
                projectile.Center = Main.npc[npcTarget].Center;
            }
            else NewPosition(player, 128);
        }
        bool init = false;
        bool target = false;
        private bool overworld;
        int ticks = 0;
        int maxTime;
        int npcTarget;
        float rot = 0;
        private const int
            AI_RAIN = 1,
            AI_STATIC = 2;
        private int projAI
        {
            get { return (int)projectile.ai[0]; }
        }
        const float radians = 0.017f;
        Vector2 npcCenter;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType<ArchaeaMod.Buffs.mercury>(), 450);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.SendNPCBuffs, -1, -1, null, target.whoAmI);
        }
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }

            Player player = Main.player[projectile.owner];

            NPC nme = Main.npc[npcTarget];

            int direction = 0;
            if (projectile.velocity.X < 0)
                direction = -1;
            else direction = 1;

            if (projAI == AI_RAIN)
            {
                if (projectile.velocity.Y < 9.17f) 
                    projectile.velocity.Y += 0.0917f;
                int dust = Dust.NewDust(projectile.Center, 2, 2, mod.DustType<Dusts.cinnabar_dust>());
                Main.dust[dust].alpha = projectile.alpha;
            }
            else if (projAI == AI_STATIC)
            {
                if (projectile.alpha <= 0)
                    projectile.Kill();
            }

            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly && !n.dontTakeDamage && !n.immortal)
                {
                    if (projectile.Hitbox.Intersects(n.Hitbox))
                    {
                        if (ticks % 60 == 0)
                        {
                            n.StrikeNPC(projectile.damage, projectile.knockBack, direction, false, false, false);
                        }
                    }
                }
            }

            ticks++;

            if (!IsTile(projectile.position.X, projectile.position.Y))
            {
                int alpha = projectile.alpha = 255 * (maxTime - ticks) / maxTime;
                float brightness = (float)ticks / maxTime;
                Lighting.AddLight(projectile.Center, new Vector3(0.804f * brightness, 0.361f * brightness, 0.361f * brightness));
            }

            projectile.frameCounter++;
            if (projectile.frameCounter == 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 7)
            {
                projectile.frame = 0;
            }

            Tile tile = Main.tile[(int)projectile.Center.X / 16, (int)projectile.Center.Y / 16];
            if (!Main.tileSolid[tile.type] || !tile.active())
                projectile.timeLeft = 5;
        }
        public override void Kill(int timeLeft)
        {
            int dustType = mod.DustType("cinnabar_dust");
            int dustType2 = mod.DustType("c_silver_dust");
            for (int k = 0; k < 3; k++)
            {
                int killDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default(Color), 1.2f);
                Main.dust[killDust].noGravity = false;
            }
            for (int k = 0; k < 2; k++)
            {
                int killDust2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType2, 0f, 0f, 0, default(Color), 1.2f);
                Main.dust[killDust2].noGravity = false;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public void SyncProj(float x, float y)
        {
            if (Main.netMode == 2)
            {
                projectile.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI, projectile.position.X, projectile.position.Y);
                projectile.netUpdate = true;
            }
            else if (Main.netMode == 0)
            {
                projectile.position = new Vector2(x, y);
            }
        }

        public void NewPosition(Player player, int maxTries)
        {
            float PosX = 0f, PosY = 0f;
            projectile.alpha = 250;
            if (projAI == AI_RAIN)
            {
                projectile.scale = 0.75f;
                PosX = Main.rand.Next((int)player.position.X - buffer, (int)player.position.X + buffer);
                PosY = Main.rand.Next((int)player.position.Y - (int)(startHeight * 1.20f), (int)player.position.Y - startHeight);
                for (int i = 0; i < maxTries; i++)
                {
                    if(!IsTile(PosX, PosY))
                    {
                        SyncProj(PosX, PosY);
                        break;
                    }
                }
            }
            else if (projAI == AI_STATIC)
            {
                PosX = Main.rand.Next((int)player.position.X - buffer, (int)player.position.X + buffer);
                PosY = Main.rand.Next((int)player.position.Y - (int)(buffer * 1.67f), (int)player.position.Y + buffer);
                for (int i = 0; i < maxTries; i++)
                {
                    if(!IsTile(PosX, PosY))
                    {
                        SyncProj(PosX, PosY);
                        break;
                    }
                }
            }
        }
        public bool IsTile(float x, float y)
        {
            int i = (int)x / 16;
            int j = (int)y / 16;
            bool Active = Main.tile[i, j].active() == true;
            bool Solid = Main.tileSolid[Main.tile[i, j].type] == true;

            if (Solid && Active) return true;
            else return false;
        }
    }
}
