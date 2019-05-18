using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Tiles
{
    public class magno_books : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Book");
        }
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Book);
        //  item.createTile = mod.TileType("m_book");
        }
    }
}
