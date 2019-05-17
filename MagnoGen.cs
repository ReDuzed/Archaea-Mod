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

namespace ArchaeaMod
{
    public class MagnoCave : ArchaeaWorld
    {
        private static int x
        {
            get { return WorldGen.genRand.Next(100, (int)Main.rightWorld / 16 - 100); }
        }
        private static int y
        {
            get { return WorldGen.genRand.Next((int)Main.worldSurface / 16 + 300, (int)Main.bottomWorld / 16 - 250); }
        }
        public int rand;
        public float progress;
        public Digger[] diggers;
        public static Vector2 Center()
        {
            return new Vector2(x, y);
        }
        public void Initialize(ushort tileID, ushort wallID)
        {
            rand = WorldGen.genRand.Next(7, 12);
            diggers = new Digger[rand];
            for (int i = 0; i < rand; i++)
                diggers[i] = new Digger(25, tileID, wallID);
        }
    }
    public class MagnoDen
    {
        public static bool active = true;
        public bool complete;
        public static bool miniBiome;
        private int width
        {
            get { return (int)Main.rightWorld / 16 / 3; }
        }
        private int leftBounds
        {
            get { return WorldGen.genRand.Next(width, width * 2); }
        }
        private int upperBounds
        {
            get { return Main.maxTilesY / 3 + 50; }
        }
        public int maxY
        {
            get { return (int)Main.bottomWorld / 16 - 200; }
        }
        private int centerX
        {
            get { return leftBounds + width / 2; }
        }
        private int centerY
        {
            get { return (int)(Main.bottomWorld / 16 / 1.5f); }
        }
        public int lookFurther;
        public static int whoAmI = 0;
        public static readonly int max = 200;
        private readonly int border = 3;
        private readonly int cave = 1;
        private int x
        {
            get { return (int)center.X; }
        }
        private int y
        {
            get { return (int)center.Y; }
        }
        public int points;
        private int cycle;
        private int id;
        private int X = Main.maxTilesX;
        private int Y = Main.maxTilesY;
        private int Width;
        private int Height;
        private int iterate;
        public Vector2 center;
        public static Vector2 origin;
        public static Rectangle[] bounds;
        public static Dictionary<Vector2, int> plots = new Dictionary<Vector2, int>();
        public static MagnoDen[] mDen;
        public void Start(MagnoDen den, bool miniBiome = true, int iterate = 8)
        {
            active = true;
            this.iterate = iterate;
            mDen = new MagnoDen[max];
            center = new Vector2(centerX / 2, centerY);
            //  bounds = new Rectangle(leftBounds, centerY, leftBounds + width / 3, centerY + width / 4);
            origin = center;
            bounds = new Rectangle[max / iterate];
            for (int i = 0; i < bounds.Length; i++)
                bounds[i] = Rectangle.Empty;
            mDen[whoAmI] = den;
            mDen[whoAmI].id = whoAmI;
            mDen[whoAmI].center = center;
        }
        public void Update()
        {
            //  GenerateNewMiner();
        }
        public bool StandardMove()
        {
            int size = WorldGen.genRand.Next(1, 4);
            int rand = WorldGen.genRand.Next(1, 5);
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x + 1 + lookFurther, y].active())
            {
                center.X += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x - 1 - lookFurther, y].active())
            {
                center.X -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y + 1 + lookFurther].active() && center.Y < maxY)
            {
                center.Y += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y - 1 - lookFurther].active() && center.Y > upperBounds)
            {
                center.Y -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (!Main.tile[x + 1 + lookFurther, y].active() &&
                !Main.tile[x - 1 - lookFurther, y].active() &&
                !Main.tile[x, y + 1 + lookFurther].active() &&
                !Main.tile[x, y - 1 - lookFurther].active())
                lookFurther++;
            if (!plots.ContainsKey(center))
                plots.Add(center, size);
            if (points > 10)
                return true;
            return false;
        }
        public void VerticalMove()
        {
            Vector2 old = center;
            while (center == old)
            {
                int rand = WorldGen.genRand.Next(1, 5);
                int x = (int)center.X;
                int y = (int)center.Y;
                switch (rand)
                {
                    case 1:
                        do
                        {
                            center.X += 1f;
                            lookFurther++;
                        } while (!Main.tile[x + 1 + lookFurther, y].active()
                                && x < Main.rightWorld / 16);
                        break;
                    case 2:
                        do
                        {
                            center.X -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x - 1 - lookFurther, y].active()
                                && x > 50);
                        break;
                    case 3:
                        do
                        {
                            center.Y += 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y + 1 + lookFurther].active()
                                && y < Main.bottomWorld / 16);
                        break;
                    case 4:
                        do
                        {
                            center.Y -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y - 1 - lookFurther].active()
                                && y > maxY);
                        break;
                    default:
                        break;
                }
                if (lookFurther % 2 == 0)
                    PlaceWater(center);
                lookFurther = 0;
            }
        }
        public bool AverageMove()
        {
            Vector2 old = center;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X -= 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y -= 1f;
            return center != old;
        }
        public MagnoDen GenerateNewMiner()
        {
            if (this == mDen[0])
            {
                whoAmI++;
                if (whoAmI == max)
                    FinalDig();
                if (whoAmI < max)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                }
                else
                    Terminate();
            }
            return mDen[Math.Min(whoAmI, max - 1)];
        }
        public Vector2 NewPosition(Vector2 previous)
        {
            return new Vector2(previous.X, origin.Y);
        }
        public static bool Inbounds(int x, int y)
        {
            return x < Main.maxTilesX - 50 && x > 50 && y < Main.maxTilesY - 200 && y > 50;
        }
        public void DigPlot(int size)
        {
            for (int i = (int)center.X - size; i < (int)center.X + size; i++)
                for (int j = (int)center.Y - size; j < (int)center.Y + size; j++)
                {
                    if (Inbounds(i, j))
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, false, true);
                    }
                }
        }
        public void FinalDig()
        {
            var v2 = plots.Keys.ToArray();
            var s = plots.Values.ToArray();
            for (int k = 1; k < v2.Length; k++)
            {
                int x = (int)v2[k].X;
                int y = (int)v2[k].Y;
                for (int i = x - s[k] * border; i < x + s[k] * border; i++)
                {
                    for (int j = y - s[k] * border; j < y + s[k] * border; j++)
                    {
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, true, true);
                        //  WorldGen.KillWall(i, j);
                    }
                }
            }
            for (int l = 1; l < v2.Length; l++)
            {
                int x = (int)v2[l].X;
                int y = (int)v2[l].Y;
                for (int i = (int)x - s[l]; i < (int)x + s[l]; i++)
                    for (int j = (int)y - s[l]; j < (int)y + s[l]; j++)
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = 0;
                        Main.tile[i, j].active(false);
                        //  WorldGen.KillTile(i, j, false, false, true);
                    }
            }
        }
        public void PlaceWater(Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (Inbounds(x, y))
                Main.tile[x, y].liquid = 60;
        }
        public void CheckComplete(int divisor = 2)
        {
            cycle++;
            if (cycle == max / divisor)
            {
                whoAmI++;
                if (whoAmI < mDen.Length)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].id = whoAmI;
                    if (miniBiome)
                    {
                        if (whoAmI % iterate == 0)
                            mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                        else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                    }
                    else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                }
            }
        }
        public void GetBounds()
        {
            int count = 0;
            foreach (MagnoDen m in mDen)
                if (m != null && m.center != Vector2.Zero)
                {
                    if (m.center.X < X)
                        X = (int)m.center.X;
                    if (m.center.Y < Y)
                        Y = (int)m.center.Y;
                    if (m.center.X - X > Width)
                        Width = (int)m.center.X - X;
                    if (m.center.Y - Y > Height)
                        Height = (int)m.center.Y - Y;
                    count++;
                    if (miniBiome)
                    {
                        if (count % iterate == 0)
                        {
                            bounds[count] = (new Rectangle(X, Y, Width, Height));
                            X = 0;
                            Y = 0;
                            Width = 0;
                            Height = 0;
                        }
                    }
                    else bounds[0] = new Rectangle(X, Y, Width, Height);
                }
        }
        public void Terminate()
        {
            Unload();
            active = false;
            for (int i = 0; i < mDen.Length; i++)
                mDen[i] = null;
        }
        protected void Unload()
        {
            origin = Vector2.Zero;
            plots.Clear();
        }
    }
}
