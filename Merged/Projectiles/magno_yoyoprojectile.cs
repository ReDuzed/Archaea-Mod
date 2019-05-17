using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class magno_yoyoprojectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mango Yoyo");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 8.5f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 13.5f;
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = false;
            projectile.scale = 1f;
            projectile.melee = true;
            projectile.extraUpdates = 0;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            bool random = Main.rand.Next(5) == 0;
            if (random)
            {
                for (float k = 0; k < MathHelper.ToRadians(360); k += 0.017f * 9)
                {
                    int Proj1 = Projectile.NewProjectile(projectile.position + new Vector2(projectile.width / 2, projectile.height / 2), Distance(null, k, 16f), mod.ProjectileType("dust_diffusion"), projectile.damage, 4f, projectile.owner, Distance(null, k, 16f).X, Distance(null, k, 16f).Y);
                    if (Main.netMode == 1) NetMessage.SendData(27, -1, -1, null, Proj1);
                    //custom sound
                    //Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/IceBeamChargeShot"), projectile.position);
                    //vanilla sound
                    Main.PlaySound(2, projectile.position, 14);
                }
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