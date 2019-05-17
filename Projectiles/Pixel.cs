using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;

namespace ArchaeaMod.Projectiles
{
    public class Pixel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.damage = 0;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * alpha;
        }
        private bool direction;
        private int ai;
        public const int
            None = -1,
            Default = 0,
            Sword = 1,
            Active = 2,
            Gravity = 3;
        private float rotate;
        private float alpha;
        private Dust dust;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case 0:
                    direction = owner.direction == 1 ? true : false;
                    rotate = direction ? 0f : (float)Math.PI;
                    dust = SetDust();
                    goto case 1;
                case 1:
                    ai = 1;
                    break;
            }
            return true;
        }
        public void AIType()
        {
            switch ((int)projectile.ai[1])
            {
                case None:
                    projectile.alpha = 0;
                    alpha = 1f;
                    projectile.timeLeft = 100;
                    break;
                case Default:
                    dust.position = projectile.position;
                    break;
                case Sword:
                    NPCs.ArchaeaNPC.RotateIncrement(true, ref rotate, (float)Math.PI / 2f, 0.15f, out rotate);
                    projectile.velocity += NPCs.ArchaeaNPC.AngleToSpeed(rotate, 0.25f);
                    dust.position = projectile.position;
                    break;
                case Active:
                    dust = SetDust();
                    break;
            }
        }
        public override void AI()
        {
            AIType();
        }
        public override void Kill(int timeLeft)
        {
            switch ((int)projectile.ai[1])
            {
                case Default:
                    break;
                case Sword:
                    NPCs.ArchaeaNPC.DustSpread(projectile.Center, 1, 1, 6, 4, 2f);
                    if (projectile.ai[0] == Mercury)
                        Projectile.NewProjectileDirect(new Vector2(owner.position.X, owner.position.Y - 600f), Vector2.Zero, mod.ProjectileType<Mercury>(), 20, 4f, owner.whoAmI, Projectiles.Mercury.Falling, projectile.position.X);
                    break;
            }
        }
        public const int
            Fire = 1,
            Dark = 2,
            Mercury = 3,
            Electric = 4;
        private Dust defaultDust
        {
            get { return Dust.NewDustDirect(Vector2.Zero, 1, 1, 0); }
        }
        public Dust SetDust()
        {
            switch ((int)projectile.ai[0])
            {
                case 0:
                    break;
                case Fire:
                    return Dust.NewDustDirect(projectile.Center, 2, 2, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
                case Mercury:
                    return Dust.NewDustDirect(projectile.Center, 2, 2, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
                case Electric:
                    Dust dust = Dust.NewDustDirect(projectile.Center, 2, 2, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
                    dust.noGravity = true;
                    return dust;
            }
            return defaultDust;
        }
    }

}
