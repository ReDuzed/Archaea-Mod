using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;
using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Merged.Projectiles
{
    public class magno_orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Orb");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.2f;
            projectile.aiStyle = -1;
            projectile.timeLeft = 3600;
            projectile.damage = 10;
            projectile.knockBack = 7.5f;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.netImportant = true;
        }

        public void Initialize()
        {
            oldAngle = (float)Math.Atan2(Main.MouseWorld.Y - projectile.position.Y, Main.MouseWorld.X - projectile.position.X);
            projectile.velocity *= 2f;
        }
        bool init = false;
        int ticks = 0;
        int dustType;
        float oldAngle;
        Player player;
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            Player player = Main.player[projectile.owner];

            float Angle = (float)Math.Atan2(Main.MouseWorld.Y - projectile.position.Y, Main.MouseWorld.X - projectile.position.X);
            if (Main.mouseLeft && Vector2.Distance(Main.MouseWorld - projectile.position, Vector2.Zero) < 128)
            {
                projectile.rotation = Angle;
                Rectangle mouseBox = new Rectangle((int)Main.MouseWorld.X - 8, (int)Main.MouseWorld.Y - 8, 16, 16);
                if (!mouseBox.Intersects(projectile.Hitbox))
                    projectile.velocity = Distance(null, Angle, 8f);
                else projectile.velocity = Vector2.Zero;

                oldAngle = (float)Math.Atan2(Main.MouseWorld.Y - projectile.Center.Y, Main.MouseWorld.X - projectile.Center.X);
                projectile.netUpdate = true;
            }
            else
            {
                projectile.velocity = Distance(null, oldAngle, 16f);
                projectile.rotation = oldAngle;
            }
            if (Vector2.Distance(player.position - projectile.position, Vector2.Zero) > 1024)
            {
                projectile.Kill();
            }
            dustType = mod.DustType("magno_dust");
            for (int k = 0; k < 1; k++)
            {
                int orbDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default(Color), 1f);
                Main.dust[orbDust].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (float k = 0; k < MathHelper.ToRadians(360); k += 0.017f * 9)
            {
                int Proj1 = Projectile.NewProjectile(projectile.position + new Vector2(projectile.width / 2, projectile.height / 2), Distance(null, k, 16f), mod.ProjectileType("dust_diffusion"), projectile.damage, 7.5f, projectile.owner, Distance(null, k, 16f).X, Distance(null, k, 16f).Y);
                Main.projectile[Proj1].netUpdate = true;
                //  if (Main.netMode == 1) NetMessage.SendData(27, -1, -1, null, Proj1);
            }
            Main.PlaySound(2, projectile.position, 14);
        }

        public void SyncProj(int netID)
        {
            if (Main.netMode == netID)
            {
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI, projectile.position.X, projectile.position.Y);
                projectile.netUpdate = true;
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
