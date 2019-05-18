using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged;

namespace ArchaeaMod.Merged.Items.Materials
{
    public class magno_bar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rubidium Bar");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.value = 3500;
            item.maxStack = 99;
            item.rare = 2;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.consumable = true;
            item.autoReuse = true;
            item.createTile = mod.TileType("m_bar");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Tiles.magno_ore>(), 4);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
