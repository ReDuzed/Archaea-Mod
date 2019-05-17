using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items
{
    public class acc_cluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Cluster");
            Tooltip.SetDefault("Adds a damage boost per successful enemy hit" +
                "\nResets after taking damage");
        }
        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.rare = -12;
            item.value = 20000;
            item.accessory = true;
            item.expert = true;
        }
        public override void UpdateEquip(Player player)
        {
            if (player.armor.Contains(item))
                player.AddBuff(mod.BuffType<Buffs.buff_cluster>(), int.MaxValue, true);
            else player.DelBuff(player.FindBuffIndex(mod.BuffType<Buffs.buff_cluster>()));
        }
    }
    public class AccPlayer : ModPlayer
    {
        public int stack;
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            stack = 0;
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            stack = 0;
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (player.armor.ToList().Contains(mod.GetItem<acc_cluster>().item));
                stack++;
            damage += stack;
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (player.armor.ToList().Contains<Item>(mod.GetItem<acc_cluster>().item))
                stack++;
            damage += stack;
        }
    }
}
