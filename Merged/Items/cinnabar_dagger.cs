using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items
{
    public class cinnabar_dagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Dagger");
            Tooltip.SetDefault("Mercury-tipped blade");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 1;
            item.damage = 20;
            item.knockBack = 4f;
            item.shootSpeed = 11f;
            item.value = 1500;
            item.rare = 1;
            item.maxStack = 999;
            item.noUseGraphic = true;
            item.autoReuse = false;
            item.consumable = true;
            item.noMelee = true;
            item.thrown = true;
            item.shoot = mod.ProjectileType("cinnabar_dagger");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 2);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
    }
}
