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

namespace ArchaeaMod.Items.Alternate
{
    public class Broadsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Broadsword");
            Tooltip.SetDefault("Shocks weakened enemies");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 20;
            item.crit = 15;
            item.value = 3500;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Green;
        }

        public override bool UseItem(Player player)
        {
            if (Main.mouseLeftRelease && Main.mouseLeft)
                if (!Main.mouseRight)
                    SetMainFunction();
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (item.mana == 0)
                SetAltFunction();
            return true;
        }
        private Target target;
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (item.mana > 0)
            {
                target = Target.GetClosest(player, Target.GetTargets(player, 240f).Where(t => t != null).ToArray());
                if (target != null && ArchaeaItem.Elapsed(90))
                    Shield.ShockTarget(hitbox.Center(), target.npc, item.damage);
            }
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (crit)
            {
                for (float r = 0; r < Math.PI * 2f + Math.PI / 4f; r += (float)Math.PI / 8f)
                {
                    Projectile proj = Projectile.NewProjectileDirect(target.Center, NPCs.ArchaeaNPC.AngleToSpeed(r, 6f), mod.ProjectileType<Pixel>(), item.damage, item.knockBack, player.whoAmI, Pixel.Fire, Pixel.Active);
                    proj.timeLeft = 20;
                    proj.tileCollide = false;
                }
            }
        }
        public void SetMainFunction()
        {
            item.damage = 20;
            item.mana = 0;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = false;
            item.channel = false;
            item.melee = true;
        }
        public void SetAltFunction()
        {
            item.damage = 10;
            item.mana = 1;
            item.useTime = 30;
            item.useAnimation = 40;
            item.useTurn = true;
            item.autoReuse = true;
            item.channel = true;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }
    }
}
