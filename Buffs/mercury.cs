using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Buffs
{
    public class mercury : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mercury Sickness");
            canBeCleared = false;
            longerExpertDebuff = true;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "";
        }
        private bool init;
        private int oldDamage;
        private Color oldColor;
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!init)
            {
                oldDamage = npc.damage;
                npc.damage = (int)(npc.damage * 0.90f);
                oldColor = npc.color;
                npc.color = Color.LightSteelBlue * 0.50f;
                init = true;
            }
            if (npc.velocity.X != 0)
                npc.velocity.X /= 1.10f;
            npc.life -= (int)Main.time % 60 == 0 ? 2 : 0;
            npc.lifeRegen = 0;
            if (npc.buffTime[buffIndex] < 5)
            {
                npc.damage = oldDamage;
                npc.color = oldColor;
            }
            Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType<Merged.Dusts.c_silver_dust>());
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (!init)
            {
                oldColor = player.skinColor;
                player.skinColor = Color.LightSteelBlue * 0.50f;
                init = true;
            }
            if (player.velocity.X != 0)
                player.velocity.X /= 1.10f;
            player.statLife -= (int)Main.time % 60 == 0 ? 2 : 0;
            player.lifeRegenTime = 450;
            player.lifeRegen = 0;
            player.lifeRegenCount = 0;
            if (player.buffTime[buffIndex] < 5)
                player.skinColor = oldColor;
            Dust.NewDust(player.position, player.width, player.height, mod.DustType<Merged.Dusts.c_silver_dust>());
        }
    }
}
