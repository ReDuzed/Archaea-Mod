using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Tiles
{
    public class cinnabar_ore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Ore");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.scale = 1f;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.consumable = true;
            item.autoReuse = true;
            item.value = 1000;
            item.rare = 1;
            item.maxStack = 250;
            item.createTile = mod.TileType("c_ore");
        }
    }
}
