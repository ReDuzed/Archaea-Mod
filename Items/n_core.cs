using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.NPCs.Bosses;

namespace ArchaeaMod.Items
{
    public class n_core : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Aura");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.rare = 2;
            item.value = 2000;
            item.useTime = 45;
            item.useAnimation = 45;
            item.useStyle = 4;
            item.autoReuse = false;
            item.consumable = true;
            item.noMelee = true;
            bossType = mod.NPCType<Sky_boss>();
        }
        private int bossType;
        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == bossType && (npc.active || npc.life > 0))
                    return false;
            }
            ArchaeaPlayer modPlayer = player.GetModPlayer<ArchaeaPlayer>();
            if (!modPlayer.SkyFort && !modPlayer.SkyPortal)
                return false;
            return true;
        }
        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, bossType);
            Main.PlaySound(SoundID.Roar, player.Center, 0);
            return true;
        }
    }
}