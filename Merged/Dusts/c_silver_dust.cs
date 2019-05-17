using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Dusts
{
    public class c_silver_dust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.scale = 1.2f;
            dust.velocity /= 2f;
            dust.color = Color.White;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X;
            dust.scale -= 0.05f;
            Lighting.AddLight((int)dust.position.X / 16, (int)dust.position.Y / 16, 0.210f, 0.210f, 0.210f);
            if (dust.scale <= 0.50f)
            {
                dust.active = false;
            }

            return true;
        }
    }
}
