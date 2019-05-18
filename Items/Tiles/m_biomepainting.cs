using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Tiles
{
    public class m_biomepainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Biome");
            Tooltip.SetDefault("R.A.");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 32;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.value = 5000;
            item.maxStack = 99;
            item.rare = 3;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.paintings>();
        }
    }
}
