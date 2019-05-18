using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Items.Alternate
{
    public class Staff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Staff");
            Tooltip.SetDefault("Casts fire wave");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 0;
            item.mana = 0;
            item.value = 5000;
            item.rare = ItemRarityID.Green;
            item.useTime = 60;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.channel = true;
            item.magic = true;
        }

        private int time;
        private int second = 60;
        private int elapsed
        {
            get { return (int)(second * 1.5f); }
        }
        private int maxTime
        {
            get { return elapsed * 5; }
        }
        private int manaCost
        {
            get { return second / item.useTime; }
        }
        private bool update = true;
        private int index;
        private int type = -1;
        public const int
            Reset = -1,
            Start = 0,
            Boost = 1,
            Hover = 2,
            Launch = 3;
        private float alpha;
        private Dust[] dust = new Dust[5];
        public Target[] targets;
        public override bool UseItem(Player player)
        {
            if (player.statMana <= 0)
                return false;
            if (update)
            {
                targets = Target.GetTargets(player, 135f).Where(t => t != null).ToArray();
                update = false;
            }
            return true;
        }
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (ArchaeaItem.Elapsed(30) && player.controlUseItem)
            {
                player.CheckMana(manaCost, true);
                player.manaRegenDelay = 120;
            }
            if (time++ % elapsed == 0 && time != 0)
            {
                update = true;
                index++;
            }
            for (int i = 0; i < index; i++)
            {
                if (i < 5)
                {
                    dust[i] = Dust.NewDustDirect(player.Center - new Vector2(25f, 32f) + new Vector2(i * 12f, 0f), 1, 1, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
                    dust[i].noGravity = true;
                }
            }
            if (targets == null)
                return;
            if (index == 5)
            {
                foreach (Target target in targets.Where(t => t != null))
                    target.AttackEffect(Target.ShockWave);
                BlastWave(player);
                index = 0;
            }
        }
        public override void UseStyle(Player player)
        {
            ArchaeaItem.ActiveChannelStyle(player);
        }
        public void BlastWave(Player player)
        {
            for (float r = 0f; r < Math.PI * 2f; r += 0.017f * (45f / 15f))
            {
                Vector2 velocity = NPCs.ArchaeaNPC.AngleToSpeed(r, 15f);
                Projectile pixel = Projectile.NewProjectileDirect(player.Center, velocity, mod.ProjectileType<Pixel>(), 0, 0f, player.whoAmI, Pixel.Fire, Pixel.Default);
                pixel.tileCollide = false;
                pixel.timeLeft = 15;
            }
        }
        protected void ResetItem()
        {
            dust = new Dust[5];
            time = 0;
            alpha = 0f;
        }
    }
    
}
