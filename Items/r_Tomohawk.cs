using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Items
{
    public class r_Tomohawk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Tomohawk");
            Tooltip.SetDefault("Was once shiny");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.value = 100;
            item.rare = 2;
            item.maxStack = 250;
            item.useTime = 30;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shoot = mod.ProjectileType<Tomohawk>();
            item.shootSpeed = 7f;
            item.thrown = true;
            item.consumable = true;
            item.noUseGraphic = true;
        }
    }
}
