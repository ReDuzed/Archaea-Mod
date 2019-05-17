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
    public class Javelin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Javelin");
        }
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.damage = 10;
            projectile.knockBack = 0f;
            projectile.penetrate = 10;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.thrown = true;
        }
        private int ai = -1;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    projectile.rotation = NPCs.ArchaeaNPC.AngleTo(owner.Center, Main.MouseWorld) + (float)(Math.PI / 4f);
                    rotate = projectile.rotation;
                    rotate += (float)Math.PI / 4f;
                    projectile.position = new Vector2(ArchaeaItem.StartThrowX(owner), projectile.position.Y - 16f);
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }
        private int time = 90;
        private float rotate;
        private float x;
        private float y;
        private Vector2 hit;
        private Target target;
        private NPC npc;
        public override void AI()
        {
            if (ArchaeaItem.Elapsed(5))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
        }
    }
}
