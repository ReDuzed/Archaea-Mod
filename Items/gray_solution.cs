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
    public class gray_solution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorched Solution");
            Tooltip.SetDefault("Transforms stone into Magno stone");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.rare = 2;
            item.value = 100;
            item.maxStack = 999;
            item.ammo = AmmoID.Solution;
            item.shoot = mod.ProjectileType<GraySolution>() - ProjectileID.PureSpray;
            item.consumable = true;
        }
    }
    sealed class GraySolution : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gray solution");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.scale = 0.67f;
            projectile.timeLeft = 80;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            for (int n = 0; n < projectile.width; n++)
            {
                int i = (int)projectile.position.X + n;
                int j = (int)projectile.position.Y + n;
                Tile tile = Framing.GetTileSafely(new Vector2(i, j));
                if (tile.type == TileID.Stone && tile.active())
                {
                    tile.type = (ushort)mod.TileType<Merged.Tiles.m_stone>();
                    WorldGen.SquareTileFrame(i / 16, j / 16, true);
                }
            }
            int t = Dust.NewDust(projectile.position, projectile.width, projectile.height, mod.DustType<Merged.Dusts.magno_dust>(), 0f, 0f, 0, default(Color), 1.2f);
            Main.dust[t].noGravity = true;
            Main.dust[t].noLight = true;
        }
    }
}
