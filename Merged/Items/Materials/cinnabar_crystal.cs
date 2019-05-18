using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Materials
{
    public class cinnabar_crystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Crystal");
            Tooltip.SetDefault("Glows with a crimson aura");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.value = 1000;
            item.maxStack = 99;
            item.rare = 2;
        }
    }
}
