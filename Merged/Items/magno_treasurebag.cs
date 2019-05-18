using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Items
{
    public class magno_treasurebag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.rare = 11;
            item.maxStack = 250;
            item.consumable = true;
            item.expert = true;
            bossBagNPC = mod.NPCType("boss_magnohead");
        }

        public override bool CanRightClick()
        {
            return true;
        }
        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(ItemID.GoldCoin, 5);
            //  player.QuickSpawnItem(mod.ItemType<magno_shieldacc>(), 1);
            player.QuickSpawnItem(mod.ItemType<Vanity.magno_mask>(), 1);
            player.QuickSpawnItem(mod.ItemType<Tiles.magno_ore>(), Main.rand.Next(34, 68));
            //  player.QuickSpawnItem(mod.ItemType<magno_fragment>(), Main.rand.Next(12, 24));
        }
    }
}
