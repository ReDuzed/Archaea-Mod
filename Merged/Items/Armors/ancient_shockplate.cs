using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Projectiles;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Body)]
    public class ancient_shockplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Shock Plate");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 3;
            item.defense = 5;
        }

        int Proj1;
        int ticks = 0;
        int d = 0;
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == mod.ItemType("ancient_shockhelmet") && legs.type == mod.ItemType("ancient_shockgreaves");
        }
    }
}
