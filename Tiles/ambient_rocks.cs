using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Tiles
{
    public class ambient_rocks : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            //TileObjectData.newTile.AnchorValidTiles = new int[] { ArchaeaWorld.magnoStone };
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidBottom | AnchorType.SolidTile, 0, 0);
            #region Stalagmites 1x2
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(1);
            #endregion
            #region Stalagmites 1x1
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Height = 1;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.CoordinateHeights = new int[] { 16 };
            TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(2);
            #endregion
            #region Stalagtites 1x1
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Height = 1;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.CoordinateHeights = new int[] { 16 };
            TileObjectData.addAlternate(3);
            #endregion
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(210, 110, 110));
            disableSmartCursor = true;
            mineResist = 1.2f;
            minPick = 45;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Main.tile[i, j].frameX = (short)(18 * WorldGen.genRand.Next(3));
        }
    }
}
