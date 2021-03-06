﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Body)]
    public class magnoplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Breastplate");
            Tooltip.SetDefault("7% increased damage");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 2;
            item.defense = 7;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 20);
            recipe.AddIngredient(mod.ItemType<magno_fragment>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage /= 0.93f;
            player.rangedDamage /= 0.93f;
            player.magicDamage /= 0.93f;
            player.thrownDamage /= 0.93f;
            player.minionDamage /= 0.93f;
        }
    }
}
