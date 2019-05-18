using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Legs)]
    public class ancient_shockgreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Shock Greaves");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 100;
            item.rare = 3;
            item.defense = 3;
        }
    }
}
