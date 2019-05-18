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
    public class magnohelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Helmet");
            Tooltip.SetDefault("Increases maximum minion "
                +   "\ncount by 1");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 2;
            item.defense = 3;
        }
        public override void AddRecipes()
        {
            // -- //
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<magno_bar>(), 10);
            recipe.AddIngredient(mod.ItemType<magno_fragment>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("magnoplate") && legs.type == mod.ItemType("magnogreaves");
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions++;
        }
        bool flag;
        int projMinion;
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summons a Magno minion"
                +   "\nto your aid when"
                +   "\nin dire need";

            if (!flag && player.statLife <= player.statLifeMax / 2)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("magno_minion")] < player.maxMinions && player.numMinions < player.maxMinions)
                {
                    player.AddBuff(mod.BuffType("magno_summon"), 18000, false);
                    Main.PlaySound(2, player.Center, 20);
                    projMinion = Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("magno_minion"), 5, 3f, player.whoAmI, 0f, 0f);
                    player.maxMinions += 1;
                }
                flag = true;
            }
            if (flag && player.statLife > player.statLifeMax / 2)
            {
                if (Main.projectile[projMinion].type == mod.ProjectileType("magno_minion"))
                    Main.projectile[projMinion].active = false;
                flag = false;
            }
        }
    }
}
