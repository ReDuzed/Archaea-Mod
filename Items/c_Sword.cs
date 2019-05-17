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

using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Items
{
    public class c_Sword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Sword");
            Tooltip.SetDefault("Sends ground tremmors");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.value = 3500;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useTurn = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.melee = true;
        }

        private int time;
        private int index;
        private Vector2[] ground;
        public override bool UseItem(Player player)
        {
            index = 0;
            ground = GetGround(player, 10);
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (ground == null)
                return;
            if (index < ground.Length)
            {
                if (ArchaeaItem.Elapsed(5))
                    Projectile.NewProjectileDirect(ground[index++], Vector2.Zero, mod.ProjectileType<Mercury>(), item.damage, item.knockBack, player.whoAmI, Mercury.Ground);
            }
        }

        protected Vector2[] GetGround(Player start, int length)
        {
            List<Vector2> ground = new List<Vector2>();
            int x = (int)start.Center.X;
            int y = (int)start.position.Y + start.height;
            bool direction = start.direction == 1;
            for (int k = 0; direction ? k < length : k >= 0 - length; k -= direction ? -1 : 1)
            {
                bool add = true;
                int total = 0;
                int i = k + x / 16;
                int j = y / 16;
                while (!Main.tile[i, j].active())
                {
                    j++;
                    if (total++ > 10)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                    ground.Add(new Vector2(i * 16, j * 16));
            }
            return ground.ToArray();
        }
    }
}
