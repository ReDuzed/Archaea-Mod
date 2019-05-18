using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class magnoheadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Headgear");
            Tooltip.SetDefault("10% increased arrow"
                +   "\ndamage");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 2;
            item.defense = 5;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 10);
            recipe.AddIngredient(mod.ItemType<magno_fragment>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("magnoplate") && legs.type == mod.ItemType("magnogreaves");
        }
        public override void UpdateEquip(Player player)
        {
            player.arrowDamage /= 0.90f;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "15% chance"
                +   "\nfor arrows to explode";
        }
    }
}
