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
    public class banner_hatchling : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hatchling banner");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.value = 1000;
            item.maxStack = 99;
            item.rare = 2;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.banners>();
        }
    }
}
