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

namespace ArchaeaMod.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class ShockMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock Mask");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 12;
            item.defense = 5;
            item.value = 5000;
            item.rare = ItemRarityID.Orange;
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = false;
            drawAltHair = false;
        }
    }
}
