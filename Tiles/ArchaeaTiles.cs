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
                        return mod.GetModWorld<ArchaeaWorld>().downedMagno;
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
                        return mod.GetModWorld<ArchaeaWorld>().downedMagno;
                }
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }
        private int counter;
        public override void RandomUpdate(int i, int j, int type)
        {
            if (!ArchaeaWorld.Inbounds(i, j))
                return;
            if (Main.rand.NextFloat() > 0.95f && type == ArchaeaWorld.magnoStone)
            {
                int count = 0;
                ushort[] types = new ushort[]
                {
                    ArchaeaWorld.crystal,
                    ArchaeaWorld.crystal2x1,
                    ArchaeaWorld.crystal2x2
                };
                for (int k = i - 8; k < i + 8; k++)
                for (int l = j - 8; l < j + 8; l++)
                {
                    foreach (ushort t in types)
                    {
                        if (Main.tile[k, l].type == t)
                        {
                            count++;
                        }
                    }
                }
                if (count == 0)
                {
                    Tile top = Main.tile[i, j + 1];
                    Tile right = Main.tile[i + 1, j];
                    Tile bottom = Main.tile[i, j - 1];
                    Tile left = Main.tile[i - 1, j];
                    if (type == ArchaeaWorld.magnoStone)
                    {
                        if (!top.active())
                            WorldGen.PlaceTile(i, j - 1, (int)types[0], true, false, -1, 3);
                        else if (!right.active())
                            WorldGen.PlaceTile(i, j - 1, (int)types[0], true, false, -1, 1);
                        else if (!bottom.active())
                            WorldGen.PlaceTile(i, j - 1, Main.rand.Next(new int[] { (int)types[0], (int)types[1], (int)types[3] }), true, false, -1, 0);
                        else if (!left.active())
                            WorldGen.PlaceTile(i, j - 1, (int)types[0], true, false, -1, 2);
                    }
                }
            }
        }
    }
}
