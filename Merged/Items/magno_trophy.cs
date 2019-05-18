using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items
{
    public class magno_trophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Trophy");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.scale = 1f;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;
            item.value = 5000;
            item.rare = 3;
            item.maxStack = 99;
            item.createTile = mod.TileType("m_trophy");
        }
    }
}
