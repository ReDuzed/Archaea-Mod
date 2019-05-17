using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class magno_minionexplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Minion Explosion");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.scale = 1f;
            projectile.damage = 0;
            projectile.aiStyle = -1;
            projectile.timeLeft = 60;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(new Vector2(projectile.position.X / 16, projectile.position.Y / 16), new Vector3(0.4f, 0.5f, 0.25f));

            projectile.frameCounter++;
            if(projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if(projectile.frame > 3)
            {
                projectile.Kill();
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor = Color.PaleGoldenrod;
        }
    }
}
