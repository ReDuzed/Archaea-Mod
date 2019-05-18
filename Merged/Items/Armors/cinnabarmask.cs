using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Items.Materials;

namespace ArchaeaMod.Merged.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class cinnabarmask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Mask");
            Tooltip.SetDefault("10% increased magic"
                +   "\ndamage");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 3;
            item.defense = 4;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 10);
            recipe.AddIngredient(mod.ItemType<cinnabar_crystal>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType<cinnabarplate>() && legs.type == mod.ItemType<cinnabargreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage /= 0.9f;
        }
        bool spawnOK = false;
        int ticks = 0;
        int buffer = 256;
        int x, y;
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Generates lethal" 
                +   "\nspores";

            if (ticks++ % 60 == 0)
            {
                int newProj = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("cinnabar_spore"), 14, 0f, player.whoAmI, x, y);
            }
        }

        public bool TileCheck(int i, int j)
        {
        //  bool Dirt = Main.tile[i, j].type == TileID.Dirt;
            bool Active = Main.tile[i, j].active() == true;
            bool Solid = Main.tileSolid[Main.tile[i, j].type] == true;

            if (Solid && Active) return true;
            else return false;
        }
    }
}
