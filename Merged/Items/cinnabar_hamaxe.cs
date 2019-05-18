using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items
{
    public class cinnabar_hamaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Hamaxe");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.scale = 1f;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = 1;

            item.axe = 20;
            item.hammer = 50;
            item.damage = 16;
            item.knockBack = 7.5f;
            item.value = 1500;
            item.rare = 2;

            //  custom sound?
            //  item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/*");
            //  or vanilla sound?
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.melee = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 8);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
