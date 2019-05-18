using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Tiles
{
    public class magno_dirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Dirt");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.scale = 1f;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;
            item.value = 0;
            item.rare = 1;
            item.maxStack = 999;
            item.createTile = mod.TileType("m_dirt");
        }
    }
}
