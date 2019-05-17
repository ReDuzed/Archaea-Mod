using System;
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
    public class flask_mercury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flask of Mercury");
            Tooltip.SetDefault("Adds mercury sickness on melee hit");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 26;
            item.useTime = 15;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.maxStack = 20;
            item.value = 800;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            Main.PlaySound(SoundID.Pixie, player.Center);
            player.AddBuff(mod.BuffType<Buffs.flask_mercury>(), 72000);
            if (item.stack > 0)
            {
                item.stack--;
                return true;
            }
            return false;
        }
    }
}
