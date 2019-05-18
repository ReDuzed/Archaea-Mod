using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Materials
{
    public class magno_fragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Fragment");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 30;
            item.scale = 0.85f;
            item.value = 2500;
            item.maxStack = 99;
            item.rare = 1;
        }
    }
}
