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

using ArchaeaMod.Buffs;
using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Items
{
    public class r_Catcher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Soul Catcher");
            Tooltip.SetDefault("Metallic minion");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.value = 3500;
            item.rare = 2;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }
        private int count;
        private int minions;
        private int buffType
        {
            get { return mod.BuffType<buff_catcher>(); }
        }
        private Projectile minion;
        public override bool UseItem(Player player)
        {
            minions = count + player.numMinions;
            if (minions == player.maxMinions || player.HasBuff(buffType))
            {
                if (minion != null)
                    minion.active = false;
                minion = Projectile.NewProjectileDirect(player.position - new Vector2(0, player.height), Vector2.Zero, mod.ProjectileType<CatcherMinion>(), item.damage, item.knockBack, player.whoAmI);
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, minion.whoAmI);
            }
            if (!player.HasBuff(buffType))
            {
                player.AddBuff(buffType, 36000);
                minion = Projectile.NewProjectileDirect(player.position - new Vector2(0, player.height), Vector2.Zero, mod.ProjectileType<CatcherMinion>(), item.damage, item.knockBack, player.whoAmI);
                count = player.ownedProjectileCounts[minion.type];
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, minion.whoAmI);
            }
            return true;
        }
    }
}
