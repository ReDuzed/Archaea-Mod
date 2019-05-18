﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Tiles
{
    public class m_door : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac door");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 34;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.value = 0;
            item.rare = 1;
            item.maxStack = 99;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.m_doorclosed>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Merged.Items.Tiles.magno_brick>(), 6);
            recipe.AddIngredient(mod.ItemType<Merged.Items.Materials.magno_bar>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(item.type);
            recipe.Create();
        }
    }
}
