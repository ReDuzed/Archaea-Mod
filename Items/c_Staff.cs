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

using ArchaeaMod.NPCs;
using ArchaeaMod.Projectiles;


namespace ArchaeaMod.Items
{
    public class c_Staff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnabar Staff");
            Tooltip.SetDefault("Emits mercury dust");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.mana = 10;
            item.value = 3500;
            item.rare = 2;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.magic = true;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (ArchaeaItem.Elapsed(10))
                Projectile.NewProjectileDirect(hitbox.Center(), NPCs.ArchaeaNPC.AngleToSpeed(player.AngleTo(Main.MouseWorld), VelocityWeight(player, 4f)), mod.ProjectileType<Pixel>(), item.damage, item.knockBack, player.whoAmI, Pixel.Mercury, Pixel.Sword);
        }
        public static float VelocityWeight(Player player, float strength, float multiplier = 0.01f)
        {
            return player.Distance(Main.MouseWorld) * multiplier * strength;
        }
    }
}
