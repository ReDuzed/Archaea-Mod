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

namespace ArchaeaMod.Tiles
{
    public class ArchaeaTiles : GlobalTile
    {
        public override bool CanExplode(int i, int j, int type)
        {
            for (int k = -1; k <= 1; k++)
                for (int l = -1; l <= 1; l++)
                {
                    Tile tile = Main.tile[i + k, j + l];
                    if (tile.type == ArchaeaWorld.crystal ||
                        tile.type == ArchaeaWorld.crystal2x1 ||
                        tile.type == ArchaeaWorld.crystal2x2)
                        return false;
                }
            return base.CanExplode(i, j, type);
        }
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            for (int k = -1; k <= 1; k++)
                for (int l = -1; l <= 1; l++)
                {
                    Tile tile = Main.tile[i + k, j + l];
                    if (tile.type == ArchaeaWorld.crystal ||
                        tile.type == ArchaeaWorld.crystal2x1 ||
                        tile.type == ArchaeaWorld.crystal2x2)
                        return false;
                }
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }
    }
}
