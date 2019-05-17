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
    public class Mercury : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercury Shards");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 10;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = true;
        }

        private int ai = -1;
        public const int
            Ground = 0,
            Falling = 1;
        public float velX;
        public float velY;
        public Vector2 start;
        private Dust dust;
        public Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    Initialize();
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }
        public override void AI()
        {
            switch ((int)projectile.ai[0])
            {
                case Ground:
                    dust.velocity = projectile.velocity;
                    projectile.velocity.Y = velY;
                    break;
                case Falling:
                    projectile.rotation += 0.017f * 5f;
                    projectile.velocity = new Vector2(velX, velY);
                    break;
            }
        }
        public override void Kill(int timeLeft)
        {
            if ((int)projectile.ai[0] != Ground)
                NPCs.ArchaeaNPC.DustSpread(projectile.Center, 1, 1, 6, 3, 2f);
        }
        protected void Initialize()
        {
            switch ((int)projectile.ai[0])
            {
                case Ground:
                    velY = -8f;
                    dust = defaultDust;
                    projectile.timeLeft = 30;
                    projectile.tileCollide = false;
                    projectile.friendly = true;
                    NPCs.ArchaeaNPC.DustSpread(projectile.Center, 1, 1, 6, 4, 2f);
                    break;
                case Falling:
                    start = new Vector2(projectile.ai[1], owner.position.Y - 600f);
                    projectile.position = start;
                    velX = Main.rand.NextFloat(-2f, 2f);
                    velY = 12f;
                    break;
            }
        }
        private Dust defaultDust
        {
            get { return Dust.NewDustDirect(projectile.Center, 1, 1, 6); }
        }
    }
}
