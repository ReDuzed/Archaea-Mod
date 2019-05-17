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
    public class Tomohawk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tomohawk");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = 50;
            projectile.friendly = true;
            projectile.thrown = true;
        }

        private int ai = -1;
        private int direction;
        private float yOffset = 16f;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    projectile.position.Y -= yOffset;
                    direction = owner.direction;
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }

        public override void AI()
        {
            projectile.rotation -= -Draw.radian * 5f * direction;
            Dusts(3);
            if (projectile.timeLeft < 10)
            {
                if ((projectile.alpha += 20) < 200)
                    projectile.timeLeft = 10;
            }
        }
        public override void Kill(int timeLeft)
        {
            Dusts(8, true);
        }
        protected void Dusts(int amount, bool noGravity = false)
        {
            for (int i = 0; i < amount; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, projectile.velocity.X, projectile.velocity.Y);
                dust.noGravity = noGravity;
            }
        }
    }
}
