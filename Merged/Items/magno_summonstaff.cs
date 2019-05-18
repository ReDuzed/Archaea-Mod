using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged.Buffs;

namespace ArchaeaMod.Merged.Items
{
    public class magno_summonstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rubidium Staff");
            Tooltip.SetDefault("Summons Magno minion"
                    +   "\nCauses shock waves "
                    +   "\nwhen attached to enemy");
        }
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.mana = 10;
            item.damage = 12;
            item.knockBack = 3f;
            item.value = 4000;
            item.rare = 2;
            item.autoReuse = false;
            item.consumable = false;
            item.noMelee = true;
            item.summon = true;
            item.buffType = mod.BuffType<magno_summon>();
            item.buffTime = 18000;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[mod.ProjectileType("magno_minion")] < player.maxMinions && player.numMinions < player.maxMinions)
            {
                return true;
            }
            else return false;
        }
        public override bool UseItem(Player player)
        {
            int projMinion = Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("magno_minion"), 5, 3f, player.whoAmI, 0f, 0f);
            Main.projectile[projMinion].netUpdate = true;
            return true;
        }
    }
}
