using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

using System.Collections.Generic;
using ArchaeaMod;

namespace ArchaeaMod.Gen
{
    public class FortGen : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(pass => pass.Name.Equals("Lakes"));
            if (index != -1)
            {
                tasks.Insert(index, new PassLegacy("Sky Generation", delegate (GenerationProgress progress)
                {






                }));
            }
        }
    }
    public class Sector
    {
        internal class RoomID
        {
            public const int
                Hall = 0,
                Single = 1,
                Large = 2,
                Circle = 3,
                Junction = 4;
        }
        public Sector NewSector(int roomType, ushort tileType)
        {
            Sector sector = new Sector();
            sector.roomType = roomType;
            sector.tileType = tileType;
            return sector;
        }
        private int scale = 10;
        private int roomSize = 20;
        private int size 
        {
            get { return scale * roomSize; }
        } 
        private int[,] frame;
        public int roomType;
        public ushort tileType;
        public static int defaultTile = TileID.Stone;
        public static Sector[,] sectors = new Sector[10, 10];
        public void Initialize()
        {
            int size = 10;
            sectors = new Sector[size, size];
            sectors.Initialize();
            for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                sectors[i, j].roomType = WorldGen.genRand.Next(5);
            }
            for (int i = size; i >= 0; i--)
            for (int j = size; j >= 0; j--)
            {
                if (i == size || j == size)
                {
                    if (sectors[i,j].roomType == RoomID.Circle || sectors[i,j].roomType == RoomID.Large)
                        sectors[i,j].roomType = RoomID.Junction;
                }
                if (sectors[i,j].roomType == RoomID.Circle)
                {
                    sectors[i+1,j].roomType = RoomID.Circle;
                    sectors[i,j+1].roomType = RoomID.Circle;
                    sectors[i+1,j+1].roomType = RoomID.Circle;
                }
                if (sectors[i,j].roomType == RoomID.Large)
                {
                    sectors[i+1,j].roomType = RoomID.Large;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        sectors[i,j+1].roomType = RoomID.Large;
                        sectors[i+1,j+1].roomType = RoomID.Large;
                    }
                }
            }
        }
        public void EstablishSectors()
        {
            var origin = GenerateFrame();
            for (int i = 0; i < scale; i++)
            for (int j = 0; j < scale; j++)
            {
                int x = (int)(origin.X + i * roomSize);
                int y = (int)(origin.Y + j * roomSize);
                GenerateSector(x, y, i, j, sectors[i, j].roomType);
            }
        }
        internal void GenerateSector(int x, int y, int idX, int idY, int type)
        {
            for (int i = 0; i < roomSize; i++)
            for (int j = 0; j < roomSize; j++)
            {
                int m = x + i;
                int n = y + j;
                Main.tile[m,n].type = tileType;
                Main.tile[m,n].active(false);
                switch (type)
                {
                    case -1:
                        break;
                    case RoomID.Junction:
                        if ((idY != 0 && idY != scale - 1 && (i < 5 || i > 15)) && (idX != 0 && idX != scale - 1 &&(j < 5 || j > 15)))
                        {
                            Main.tile[m,n].active(true);
                            Main.tile[m,n].type = tileType;
                        }
                        break;
                    case RoomID.Large:
                        for (int k = -1; k <= 1; k++)
                        {
                            if (sectors[idX + k, idY].roomType == RoomID.Large || sectors[idX + k, idY].roomType == RoomID.Junction)
                            {
                                if (((k == -1 && i < 5) || (k == 1 && i > 15)) && (j < 5 || j > 15))
                                {
                                    Main.tile[m,n].active(true);
                                    Main.tile[m,n].type = tileType;
                                }
                            }
                            if (sectors[idX, idY + k].roomType == RoomID.Large || sectors[idX, idY + k].roomType == RoomID.Junction)
                            {
                                if ((i < 5 || i > 15) && ((k == -1 && j < 5) || (k == 1 && j > 15)))
                                {
                                    Main.tile[m,n].active(true);
                                    Main.tile[m,n].type = tileType;
                                }
                            }
                        }
                        break;
                    case RoomID.Circle:
                        if (idX != scale - 1 && idY != scale -1)
                        {
                            if (sectors[idX + 1, idY].roomType == RoomID.Circle)
                            {
                                
                            }
                        }
                        break;
                }
            }
        }

        internal Vector2 GenerateFrame()
        {
            var origin = SetOrigin();
            int x = (int)origin.X;
            int y = (int)origin.Y;
            for (int i = x; i < x + size; i++)
            for (int j = y; j < y + size; j++)
            {
                Main.tile[i, j].type = (ushort)tileType;
                Main.tile[i, j].active(true);
            }
            return origin;
        }

        internal Vector2 SetOrigin()
        {
            int point = WorldGen.genRand.Next(Main.maxTilesX - 300);
            if (point < 300)
                point += 300;
            return new Vector2(point, 100);
        }
    }
}