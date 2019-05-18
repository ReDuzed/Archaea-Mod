using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Merged;

namespace ArchaeaMod.Items.Tiles
{
    public class m_chandelier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Chandelier");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.value = 1000;
            item.rare = 1;
            item.maxStack = 99;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.m_chandelier>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Merged.Items.Materials.magno_bar>(), 3);
            recipe.AddIngredient(mod.ItemType<Merged.Items.Tiles.magno_brick>(), 4);
            recipe.AddIngredient(TileID.Torches, 4);
            recipe.AddIngredient(TileID.Chain, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(item.type);
            recipe.Create();
        }
    }
}
