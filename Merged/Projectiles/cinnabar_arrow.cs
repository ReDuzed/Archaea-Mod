using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_arrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercury Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.damage = 12;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.scale = 1f;
            projectile.ranged = true;
            projectile.arrow = true;
        }
        bool init = false;
        int ticks = 0;
        float Angle;
        float degrees = 0;
        const float radians = 0.017f;
        Player player;
        public override void AI()
        {
            if (!init)
            {
                Player player = Main.player[projectile.owner];
                Angle = (float)Math.Atan2(player.Center.Y - Main.MouseWorld.Y, player.Center.X - Main.MouseWorld.X);

                if (projectile.velocity.X < 0f)
                {
                    projectile.spriteDirection = -1;
                    Angle += radians * -90f;
                }
                else Angle += radians * -90f;
                projectile.netUpdate = true;
                init = true;
            }

            ticks++;
            if (ticks >= 20)
                projectile.velocity.Y += 0.10f;

            projectile.rotation = projectile.velocity.ToRotation() + Draw.radian * 90f;

            if (projectile.velocity.X < 0f && projectile.oldVelocity.X >= 0f || projectile.velocity.X > 0f && projectile.oldVelocity.X <= 0f || projectile.velocity.Y < 0f && projectile.oldVelocity.Y >= 0f || projectile.velocity.Y > 0f && projectile.oldVelocity.Y <= 0f)
                projectile.netUpdate = true;

            int dustType = mod.DustType("c_silver_dust");
            int Dust1 = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 1, 1, dustType, 0f, 0f, 0, Color.White, 1.4f); // old dust: 159, Color.OrangeRed
            Main.dust[Dust1].noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
            {
                int killDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 4, 0f, 0f, 0, default(Color), 1f);
            }
            Main.PlaySound(SoundID.Dig, projectile.position);
        }

        public void SyncProj(int netID)
        {
            if (Main.netMode == netID)
            {
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI, projectile.position.X, projectile.position.Y, projectile.rotation);
                projectile.netUpdate = true;
            }
        }

    /*  public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Angle);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.rotation = reader.ReadSingle();
        }   */
    }
}
