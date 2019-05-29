using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class magno_minion : ModProjectile
    {
        public override void SetDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

            projectile.width = 26;
            projectile.height = 26;
            projectile.scale = 1f;
            projectile.damage = 0;
            projectile.aiStyle = -1;
            projectile.timeLeft = 18000;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 1f;
            projectile.netImportant = true;
        }

        public void Initialize()
        {
            projectile.netUpdate = true;
            oldProj = projectile.whoAmI;
        }
        bool init;
        bool target = false, targeted = false;
        bool flag, flag2, flag3;
        int ticks = 0;
        int Proj1;
        int npcTarget = 0, oldNpcTarget = -1;
        int oldProj;
        int Random;
        float Angle, npcAngle;
        float WaveTimer;
        float degrees = 0;
        const float radians = 0.017f;
        Vector2 orbitPosition;
        Vector2 npcCenter;
        public override void AI()
        {
            if(!init)
            {
                Initialize();
                init = true;
            }

            Player player = Main.player[projectile.owner];

            ticks++;
            projectile.damage = 0;

            if (player.dead || !player.HasBuff(mod.BuffType("magno_summon")))
            {
                projectile.active = false;
            }
            if (player.HasBuff(mod.BuffType("magno_summon")))
            {
                projectile.timeLeft = 2;
            }
            if (player.ownedProjectileCounts[projectile.type] > player.maxMinions)
            {
                foreach(Projectile p in Main.projectile)
                {
                    if(p.type == projectile.type && p.active)
                    {
                        p.Kill();
                        break;
                    }
                }
            }

            if (!flag3)
            {
                Random = Main.rand.Next(-24, 24);
                projectile.netUpdate = true;
                flag3 = true;
            }
            orbitPosition = player.position + new Vector2(Random * 2f, -64f);
            Angle = (float)Math.Atan2(orbitPosition.Y - projectile.position.Y, orbitPosition.X - projectile.position.X);
            if (!target)
            {
                if (Vector2.Distance(orbitPosition - projectile.position, Vector2.Zero) > 32f && Vector2.Distance(orbitPosition - projectile.position, Vector2.Zero) <= 128f)
                {
                    projectile.position += Distance(null, Angle, 4f);
                    projectile.velocity = Vector2.Zero;
                }
                else if (Vector2.Distance(orbitPosition - projectile.position, Vector2.Zero) > 128f)
                {
                    projectile.velocity = Distance(null, Angle, 8f);
                }
                if(Vector2.Distance(orbitPosition - projectile.position, Vector2.Zero) > 1024)
                {
                    projectile.position = player.Center - new Vector2(0, player.height);
                }
                #region float
                float Revolution = 6.28308f;
                float WavesPerSecond = 1.0f;
                float Time = 1.0f / Main.frameRate;
                WaveTimer += Time * Revolution * WavesPerSecond;
                float Cos = (float)Math.Cos(180);
                float WaveOffset = (float)Math.Sin(WaveTimer) * 5f;

                projectile.position.Y += Cos * WaveOffset;
                #endregion
            }
            
            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly && !n.dontTakeDamage && !n.immortal && n.target == player.whoAmI && ((n.lifeMax >= 50 && (Main.expertMode || Main.hardMode)) || (n.lifeMax >= 15 && !Main.expertMode && !Main.hardMode)))
                {
                    npcCenter = new Vector2(n.position.X + n.width / 2, n.position.Y + n.height / 2);
                    if ((n.life <= 0 || !targeted || npcTarget != n.whoAmI) &&
                         Vector2.Distance(npcCenter - projectile.position, Vector2.Zero) < 384f)
                    {
                        oldNpcTarget = npcTarget;
                        npcTarget = n.whoAmI;
                        projectile.netUpdate = true;
                        targeted = true;
                    }
                }
                else
                {
                    projectile.spriteDirection = player.direction * -1;
                    projectile.rotation = 0;
                }
            }
            if(targeted)
            {
                NPC n = Main.npc[npcTarget];
                npcCenter = new Vector2(n.position.X + n.width / 2, n.position.Y + n.height / 2);
                npcAngle = (float)Math.Atan2(npcCenter.Y - projectile.position.Y, npcCenter.X - projectile.position.X);
                //  projectile.rotation = npcAngle;
                if (projectile.Hitbox.Intersects(n.Hitbox))
                    projectile.spriteDirection = n.spriteDirection;
                if (Vector2.Distance(npcCenter - projectile.position, Vector2.Zero) < 384f)
                {
                    if (!projectile.Hitbox.Intersects(n.Hitbox))
                    {
                        if (!flag2)
                        {
                            projectile.position += Distance(null, npcAngle, 16f);
                            projectile.netUpdate = true;
                        }
                    }
                    else 
                    {
                        /*  float radius = 32f;
                            degrees += radians * 9f;
                            projectile.position.X = n.Center.X + (float)(radius * Math.Cos(degrees));
                            projectile.position.Y = n.Center.Y + (float)(radius * Math.Sin(degrees));
                        */
                        flag2 = true;
                        projectile.position = n.position;
                        if (ticks % 120 == 0)
                        {
                            for (float k = 0; k < MathHelper.ToRadians(360); k += 0.017f * 9)
                            {
                                int d = Dust.NewDust(projectile.position + new Vector2(projectile.width / 2, projectile.height / 2), 4, 4, DustID.Fire, Distance(null, k, 2f).X, Distance(null, k, 8f).Y, 0, default(Color), 2f);
                                Main.dust[d].noGravity = true;
                            }
                            Main.npc[npcTarget].StrikeNPC((int)(12f * player.minionDamage), 4f, 0);
                            int Proj2 = Projectile.NewProjectile(projectile.position, Vector2.Zero, mod.ProjectileType<magno_minionexplosion>(), 0, 0f, projectile.owner, 0f, 0f);
                            Main.projectile[Proj2].position = projectile.position - new Vector2(15, 15);
                            Main.PlaySound(2, projectile.position, 14);
                            projectile.netUpdate = true;
                        }
                    }
                    target = true;
                }
                else target = false;
                if (!n.active || oldNpcTarget != n.whoAmI)
                {
                    flag2 = false;
                    target = false;
                    targeted = false;
                }
            }
            if (projectile.velocity.X <= projectile.oldVelocity.X || projectile.velocity.X > projectile.oldVelocity.X || projectile.velocity.Y <= projectile.oldVelocity.Y || projectile.velocity.Y > projectile.oldVelocity.Y)
                projectile.netUpdate = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public Vector2 Distance(Player player, float Angle, float Radius)
        {
            float VelocityX = (float)(Radius * Math.Cos(Angle));
            float VelocityY = (float)(Radius * Math.Sin(Angle));

            return new Vector2(VelocityX, VelocityY);
        }
    }
}
