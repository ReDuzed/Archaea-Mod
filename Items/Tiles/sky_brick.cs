using System;
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
    public class sky_brick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fortress Brick");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.value = 0;
            item.rare = 1;
            item.maxStack = 999;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.sky_brick>();
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(mod.ItemType<Walls.sky_brickwall>(), 4);
            recipe.AddRecipe();
        }
    }
}
