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
    public class n_Staff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necrosis' Staff");
            Tooltip.SetDefault("Emits orbitals around aiming direction" +
                "\nAlt use to disarm orbitals");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 0f;
            item.mana = 10;
            item.value = 3500;
            item.useTime = 100;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.magic = true;
        }

        private int index = -1;
        private float angle;
        private Projectile[] projs = new Projectile[6];
        public override bool UseItem(Player player)
        {
            foreach (Projectile proj in projs.Where(t => t != null))
                proj.Kill();
            index = 0;
            angle = 0f;
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (index != -1)
            {
                if (index == 6)
                {
                    index = -1;
                    return;
                }
                if (ArchaeaItem.Elapsed(10))
                {
                    Vector2 start = NPCs.ArchaeaNPC.AngleBased(player.Center, angle, 45f);
                    projs[index++] = Projectile.NewProjectileDirect(start, Vector2.Zero, mod.ProjectileType<Orbital>(), item.damage, item.knockBack, player.whoAmI, angle);
                    angle += (float)Math.PI / 3f;
                }
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < projs.Length; i++)
                if (projs[i] != null && projs[i].active)
                    projs[i].Kill();
            return true;
        }
    }
}
