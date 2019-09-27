using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.NPCs;

namespace ArchaeaMod.Entities
{
    public class MagnoShield : ArchaeaEntity
    {
        private float start
        {
            get { return ai[0]; }
        }
        private float radius = 64f;
        private float orbit;
        private const float radian = 0.017f;
        public override void SetDefaults()
        {
            width = 18;
            height = 40;
        }
        public override void Update()
        {
            netUpdate = true;
            Player player = Main.player[owner];
            rotation = ArchaeaNPC.AngleTo(player.Center, Center);
            orbit += radian * Math.Min(player.statLifeMax / Math.Max(player.statLife, 1) + 2f, 6f);
            if (orbit > Math.PI * 2f)
                orbit = 0f;
            float cos = player.Center.X + (float)(radius * Math.Cos(start + orbit));
            float sine = player.Center.Y + (float)(radius * Math.Sin(start + orbit));
            Center = new Vector2(cos, sine);
            if (!player.active || Items.ArchaeaItem.NotEquipped(player, ModLoader.GetMod("ArchaeaMod").GetItem<Items.m_shield>().item))
                Kill(true);
        }
        public override void Kill(bool effect)
        {
            if (effect && active)
                ArchaeaNPC.DustSpread(Center, 2, 2, ModLoader.GetMod("ArchaeaMod").DustType<Merged.Dusts.magno_dust>(), 8, 1.2f, Color.White, false, 2f);
            active = false;
        }
    }
}