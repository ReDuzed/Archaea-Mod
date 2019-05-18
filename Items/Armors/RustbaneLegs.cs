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
    [AutoloadEquip(EquipType.Legs)]
    public class RustbaneLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rustbane Greaves");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.rare = 3;
            item.defense = 7;
            item.value = 2000;
        }
    }
}
