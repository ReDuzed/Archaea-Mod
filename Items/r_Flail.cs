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
    public class r_Flail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Thrasher");
            Tooltip.SetDefault("Scatters rust");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.value = 3500;
            item.rare = 2;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 8f;
            item.channel = true;
            item.noUseGraphic = true;
        }

        private Projectile proj;
        public override void HoldItem(Player player)
        {
            if (proj != null && proj.active)
                player.controlUseItem = true;
        }
        public override bool UseItem(Player player)
        {
            if (proj == null || !proj.active)
            {
                proj = Throw(player, Flail.Fling);
                return true;
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (proj == null || !proj.active)
            {
                proj = Throw(player, Flail.Swing);
                return true;
            }
            return false;
        }
        protected Projectile Throw(Player player, int ai)
        {
            float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, Main.MouseWorld);
            Vector2 velocity = NPCs.ArchaeaNPC.AngleToSpeed(angle, item.shootSpeed);
            return Projectile.NewProjectileDirect(new Vector2(ArchaeaItem.StartThrowX(player), player.Center.Y - 24f), velocity, mod.ProjectileType<Flail>(), item.damage, item.knockBack, player.whoAmI, ai);
        }
    }
}
