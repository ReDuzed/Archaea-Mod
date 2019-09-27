using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Entities;

namespace ArchaeaMod.Items
{
    public class m_shield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno Shield");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.value = 10000;
            item.rare = -12;
            item.accessory = true;
            item.expert = true;
        }
        private bool generate = true;
        private int time;
        private const int regen = 900;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int count =  ArchaeaEntity.entity.Where(e => e != null && e.owner == player.whoAmI && e.active).Count();
            if (count < 4)
            {
                if (time++ > regen)
                {
                    foreach (var e in ArchaeaEntity.entity.Where(e => e != null && e.owner == player.whoAmI))
                    {
                        e.Kill(false);
                    }
                }
            }
            if ((generate && count == 0) || time > regen)
            {
                for (int i = 0; i < 4; i++)
                {
                    ArchaeaEntity.NewEntity(player.Center, Vector2.Zero, 0, player.whoAmI, i * 90f * 0.017f);
                }
                time = 0;
                generate = false;
            }
        }
    }
}