using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Legs)]
    public class magnogreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Greaves");
            Tooltip.SetDefault("7% increased movement"
                +   "\nspeed");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 100;
            item.rare = 2;
            item.defense = 6;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 15);
            recipe.AddIngredient(mod.ItemType<magno_fragment>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed /= 0.93f;
        }
    }
}
