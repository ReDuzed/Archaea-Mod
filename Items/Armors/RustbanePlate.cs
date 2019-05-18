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
    [AutoloadEquip(EquipType.Body)]
    public class RustbanePlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rustbane Chestplate");
            Tooltip.SetDefault("20% increased throwing damage");
        }
        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 20;
            item.rare = 3;
            item.defense = 13;
            item.value = 4000;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return
            head == mod.GetItem<RustbaneHead>().item &&
            body == item &&
            legs == mod.GetItem<RustbaneLegs>().item;
        }
        public override void UpdateEquip(Player player)
        {
            player.thrownDamage *= 1.20f;
        }
    }
}
