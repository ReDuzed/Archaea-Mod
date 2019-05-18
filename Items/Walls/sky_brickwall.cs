using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Walls
{
    public class sky_brickwall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fortress Brick Wall");
        }
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.maxStack = 999;
            item.rare = 1;
            item.value = 0;
            item.useTime = 7;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createWall = mod.WallType<ArchaeaMod.Walls.sky_brickwall>();
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(mod.ItemType<Tiles.sky_brick>());
            recipe.Create();
        }
    }
}
