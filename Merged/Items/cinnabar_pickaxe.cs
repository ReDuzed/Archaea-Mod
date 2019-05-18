using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items
{
    public class cinnabar_pickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Pickaxe");
        }
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.scale = 1f;
            item.useTime = 16;
            item.useAnimation = 22;
            item.useStyle = 1;
            //  item % power
            item.pick = 95;
            item.damage = 13;
            item.knockBack = 7f;
            item.value = 1500;
            item.rare = 2;
            //  custom sound?
            //  item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/*");
            //  or vanilla sound?
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.melee = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 8);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
