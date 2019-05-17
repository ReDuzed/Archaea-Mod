using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_dagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Dagger");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 300;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.scale = 1f;
            projectile.melee = true;
        }

        public void Initialize()
        {
            Player player = Main.player[projectile.owner];
            Angle = (float)Math.Atan2(projectile.position.Y - Main.MouseWorld.Y, projectile.position.X - Main.MouseWorld.X);

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                Angle += radians * -90f;
                projectile.rotation = Angle + radians;
            }
            else
            {
                Angle += radians * -90f;
                projectile.rotation = Angle + radians;
            }
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
                Initialize();
                init = true;
            }
            
            ticks++;
            if (ticks >= 20)
            {
                projectile.velocity.X *= 0.98f;
                projectile.velocity.Y += 0.35f;

                if (projectile.velocity.X < 0f)
                {
                    degrees = radians * 15f;
                    projectile.rotation -= degrees;
                }
                else
                {
                    degrees = radians * 15f;
                    projectile.rotation += degrees;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextFloat() >= 0.75f)
            {
                int daggerDrop = Item.NewItem(projectile.Center, mod.ItemType("cinnabar_dagger"), 1, true, 0, false, false);
            }
            for (int k = 0; k < 8; k++)
            {
                int killDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 4, 0f, 0f, 0, default(Color), 1f);
            }
            Main.PlaySound(SoundID.Dig, projectile.position);
        }
    }
}
