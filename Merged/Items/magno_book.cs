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

namespace ArchaeaMod.Merged.Items
{
    public class magno_book : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Book");
            Tooltip.SetDefault("Indicates magic effects");
        }
        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 24;
            item.scale = 1f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.damage = 15;
            item.mana = 8;
            item.useStyle = 1;
            item.value = 2500;
            item.rare = 2;
            item.noMelee = true;
            item.magic = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[mod.ProjectileType("magno_orb")] < 1)
            {
                return true;
            }
            else return false;
        }
        int Proj1;
        public override bool UseItem(Player player)
        {
            Main.PlaySound(2, player.Center, 20);
            Proj1 = Projectile.NewProjectile(player.position + new Vector2(player.width / 2, player.height / 2), Vector2.Zero, mod.ProjectileType("magno_orb"), (int)(15 * player.magicDamage), 4f, player.whoAmI, 0f, 0f);
            Main.projectile[Proj1].netUpdate = true;
            return true;
        }
    }
}
