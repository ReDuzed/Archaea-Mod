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
    public class cinnabarhelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Helmet");
            Tooltip.SetDefault("Increases rate of" 
                +   "\nhealing");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 3;
            item.defense = 7;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 10);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType<cinnabarplate>() && legs.type == mod.ItemType<cinnabargreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 2;
            player.lifeRegenTime++;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased"
                +   "\nmelee speed";
            player.meleeSpeed /= 0.80f;
        }
    }
}
