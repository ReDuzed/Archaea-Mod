using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class magno_mask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Mask");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 1000;
            item.rare = 2;
            item.defense = 0;
            item.vanity = true;
        }
    }
}
