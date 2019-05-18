using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items.Tiles
{
    public class magno_chest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Chest");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;
            item.value = 0;
            item.rare = 1;
            item.maxStack = 999;
            item.createTile = mod.TileType("m_chest");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 2);
            recipe.AddIngredient(9, 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
