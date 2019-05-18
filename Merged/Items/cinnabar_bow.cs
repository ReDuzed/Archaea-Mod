using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;
using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items
{
    public class cinnabar_bow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Bow");
            Tooltip.SetDefault("Transforms wooden"
                + "\narrows into mercury arrows");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 50;
            item.scale = 1f;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.damage = 24;
            item.knockBack = 1.5f;
            item.shootSpeed = 7.5f;
            item.value = 2500;
            item.rare = 1;
            item.UseSound = SoundID.Item5;
            item.autoReuse = false;
            item.consumable = false;
            item.noMelee = true;
            item.ranged = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 10);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = mod.ProjectileType<Projectiles.cinnabar_arrow>();
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
