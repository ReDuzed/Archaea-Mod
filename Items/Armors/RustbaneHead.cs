using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class RustbaneHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rustbane Visor");
        }
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.rare = 3;
            item.defense = 8;
            item.value = 2500;
        }
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = false;
            drawAltHair = false;
        }
    }
}
