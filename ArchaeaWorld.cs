using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

using ArchaeaMod.GenLegacy;
using ArchaeaMod.Items;
using ArchaeaMod.Items.Alternate;
using ArchaeaMod.Merged;
using ArchaeaMod.Merged.Items;
using ArchaeaMod.Merged.Tiles;
using ArchaeaMod.Merged.Walls;

namespace ArchaeaMod
{
    public class ArchaeaWorld : ModWorld
    {
        public static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public static ushort magnoStone
        {
            get { return (ushort)getMod.TileType<m_stone>(); }
        }
        public static ushort magnoBrick
        {
            get { return (ushort)getMod.TileType<m_brick>(); }
        }
        public static ushort magnoOre
        {
            get { return (ushort)getMod.TileType<m_ore>(); }
        }
        public static ushort magnoChest
        {
            get { return (ushort)getMod.TileType<m_chest>(); }
        }
        public static ushort cOre
        {
            get { return (ushort)getMod.TileType<c_ore>(); }
        }
        public static ushort crystal
        {
            get { return (ushort)getMod.TileType<c_crystalsmall>(); }
        }
        public static ushort crystal2x1
        {
            get { return (ushort)getMod.TileType<c_crystal2x1>(); }
        }
        public static ushort crystal2x2
        {
            get { return (ushort)getMod.TileType<c_crystal2x2>(); }
        }
        public static ushort crystalLarge
        {
            get { return (ushort)getMod.TileType<Tiles.c_crystal_large>(); }
        }
        public static ushort magnoBrickWall
        {
            get { return (ushort)getMod.WallType<magno_brick>(); }
        }
        public static ushort magnoStoneWall
        {
            get { return (ushort)getMod.WallType<magno_stone>(); }
        }
        public static ushort magnoCaveWall
        {
            get { return (ushort)getMod.WallType<Walls.magno_cavewall_unsafe>(); }
        }
        public static ushort skyBrick
        {
            get { return (ushort)getMod.TileType<Tiles.sky_brick>(); }
        }
        public static ushort skyBrickWall
        {
            get { return (ushort)getMod.WallType<Walls.sky_brickwall_unsafe>(); }
        }
        public bool downedMagno;
        public static Miner miner;
        public static List<Vector2> origins = new List<Vector2>();
        private Treasures t;
        public static Vector2[] genPosition;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int CavesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Granite")); // Granite
            if (CavesIndex != -1)
            {
                miner = new Miner();
                tasks.Insert(CavesIndex, new PassLegacy("Miner", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "MINER";
                    miner.active = true;
                    miner.Reset();
                    while (miner.active)
                        miner.Update();
                    genPosition = miner.genPos;
                    progress.End();
                }));
            }
            int shinies = tasks.FindIndex(pass => pass.Name.Equals("Shinies"));
            if (shinies != -1)
            {
                tasks.Insert(shinies, new PassLegacy("Mod Shinies", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    for (int k = 0; k < (int)((4200 * 1200) * 6E-05); k++)
                    {
                        //  WorldGen.TileRunner(WorldGen.genRand.Next((int)(genPosition[0].X / 16) - miner.edge / 2, (int)(genPosition[1].X / 16) + miner.edge / 2), WorldGen.genRand.Next((int)genPosition[0].Y / 16 - miner.edge / 2, (int)genPosition[1].Y / 16 + miner.edge / 2), WorldGen.genRand.Next(15, 18), WorldGen.genRand.Next(2, 6), magnoDirt, false, 0f, 0f, false, true);
                        int randX = WorldGen.genRand.Next((int)(genPosition[0].X / 16) - miner.edge / 2, (int)(genPosition[1].X / 16) + miner.edge / 2);
                        int randY = WorldGen.genRand.Next((int)genPosition[0].Y / 16 - miner.edge / 2, (int)genPosition[1].Y / 16 + miner.edge / 2);
                        if (Main.tile[randX, randY].type == magnoStone)
                        {
                            WorldGen.TileRunner(randX, randY, WorldGen.genRand.Next(9, 12), WorldGen.genRand.Next(2, 6), magnoOre, false, 0f, 0f, false, true);
                        }
                        progress.Value = k / (float)((4200 * 1200) * 6E-05);
                    }
                    progress.End();
                }));
            }
            int index2 = tasks.FindIndex(pass => pass.Name.Equals("Lakes"));
            if (index2 != -1)
            {
                tasks.Insert(index2, new PassLegacy("Sky Generation", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "Fort";
                    //SkyHall hall = new SkyHall();
                    //hall.SkyFortGen();
                    Vector2 position;
                    do
                    {
                        position = new Vector2(WorldGen.genRand.Next(200, Main.maxTilesX - 600), 50);
                    } while (position.X < Main.spawnTileX + 250 && position.X > Main.spawnTileX - 250);
                    var s = new Structures(position, skyBrick, skyBrickWall);
                    s.InitializeFort();
                    progress.Value = 1f;
                    progress.End();
                }));
            }
            int index3 = tasks.FindIndex(pass => pass.Name.Equals("Clean Up Dirt"));
            if (index3 != -1)
            {
                tasks.Insert(index3, new PassLegacy("Mod Generation", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "Magno extras";
                    t = new Treasures();
                    int place = 0;
                    int width = Main.maxTilesX - 100;
                    int height = Main.maxTilesY - 100;
                    Vector2[] any = Treasures.FindAll(new Vector2(100, 100), width, height, false, new ushort[] { magnoStone });
                    foreach (Vector2 floor in any)
                        if (floor != Vector2.Zero)
                        {
                            int i = (int)floor.X;
                            int j = (int)floor.Y;
                            int style = 0;
                            Tile top = Main.tile[i, j - 1];
                            Tile bottom = Main.tile[i, j + 1];
                            Tile left = Main.tile[i - 1, j];
                            Tile right = Main.tile[i + 1, j];
                            if (top.type == magnoStone && top.active())
                                style = 0;
                            if (left.type == magnoStone && left.active())
                                style = 1;
                            if (right.type == magnoStone && right.active())
                                style = 2;
                            if (bottom.type == magnoStone && bottom.active())
                                style = 3;
                            t.PlaceTile(i, j, crystal, true, false, 10, false, style);
                            if (!Main.tile[i + 1, j].active())
                            {
                                place++;
                                if (place % 3 == 0)
                                    t.PlaceTile(i, j, crystal2x2, true, false, 8);
                                else t.PlaceTile(i, j, crystal2x1, true, false, 8);
                            }
                        }
                    progress.Value = 1f;
                    progress.End();
                }));
            }
            int index4 = tasks.FindIndex(pass => pass.Name.Equals("Pyramids"));
            if (index4 != -1)
            {
                tasks.Insert(index4, new PassLegacy("Sorting Floating Tiles", delegate (GenerationProgress progress)
                {
                    progress.Message = "Magno Sorting";
                    for (int j = 100; j < Main.maxTilesY - 250; j++)
                        for (int i = 100; i < Main.maxTilesX - 100; i++)
                        {
                            Tile t = Main.tile[i, j];
                            Tile[] tiles = new Tile[]
                            {
                                Main.tile[i, j + 1],
                                Main.tile[i - 1, j],
                                Main.tile[i + 1, j]
                            };
                            int count = 0;
                            if (t.type == crystal)
                            {
                                foreach (Tile tile in tiles)
                                {
                                    if (!tile.active())
                                        count++;
                                    if (count == 3)
                                    {
                                        t.active(false);
                                        break;
                                    }
                                }
                            }
                        }
                }));
            }
            int index5 = tasks.FindIndex(pass => pass.Name.Equals("Dirt Rock Wall Runner"));
            tasks.Insert(index5, new PassLegacy("Structure Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "More Magno";
                var m = new Structures.Magno();
                m.tileID = magnoBrick;
                m.wallID = magnoBrickWall;
                Vector2 origin = new Vector2(100, 100);
                Vector2[] regions = Treasures.GetRegion(origin, Main.maxTilesX - 100, Main.maxTilesY - 250, false, true, new ushort[] { magnoStone });
                int count = 0;
                int total = (int)Math.Sqrt(regions.Length);
                int max = WorldGen.genRand.Next(5, 8);
                for (int i = 0; i < max; i++)
                {
                    m.Initialize();
                    while (!m.MagnoHouse(regions[WorldGen.genRand.Next(regions.Length)]))
                    {
                        if (count < total)
                            count++;
                        else break;
                    }
                    count = 0;
                    progress.Value = (float)i / max;
                }
                progress.Value = 1f;
                progress.End();
            }));
        
            #region Vector2 array
            /* int x = MagnoDen.bounds.X;
            int y = MagnoDen.bounds.Y;
            int right = MagnoDen.bounds.Right;
            int bottom = MagnoDen.bounds.Bottom;
            Vector2[] regions = new Vector2[MagnoDen.bounds.Width * MagnoDen.bounds.Height];
            for (int i = x; i < right; i++)
                for (int j = y; j < bottom; j++)
                {
                    if (Main.tile[i, j].type == TileID.PearlstoneBrick)
                        regions[count] = new Vector2(i, j);
                    count++;
                }*/
            #endregion
        }
        public override void PostWorldGen()
        {
            int[] t0 = new int[]
            {
                mod.ItemType<Broadsword>(),
                mod.ItemType<Calling>(),
                mod.ItemType<Deflector>(),
                mod.ItemType<Sabre>(),
                mod.ItemType<Staff>()
            };
            int[] t1 = new int[]
            {
                mod.ItemType<cinnabar_dagger>(),
                mod.ItemType<magno_book>(),
                mod.ItemType<magno_yoyo>(),
                mod.ItemType<m_fossil>()
            };
            int[] t2 = new int[]
            {
                mod.ItemType<ArchaeaMod.Merged.Items.Materials.magno_bar>(),
                ItemID.SilverBar,
                ItemID.GoldBar
            };
            int[] t3 = new int[]
            {
                ItemID.ArcheryPotion, 
                ItemID.BattlePotion, 
                ItemID.CalmingPotion, 
                ItemID.GravitationPotion, 
                ItemID.HunterPotion, 
                ItemID.LesserHealingPotion, 
                ItemID.IronskinPotion, 
                ItemID.MiningPotion, 
                ItemID.RecallPotion, 
                ItemID.TeleportationPotion
            };
            int[] s0 = t0;
            int[] s1 = t1;
            int[] s2 = new int[]
            {
                mod.ItemType<ArchaeaMod.Items.Materials.r_plate>()
            };
            int[] s3 = t3;
            
            for (int i = 0; i < 1000; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;
                if (Main.tile[chest.x, chest.y].type == magnoChest)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int type = 0;
                        int fossils = 0; 
                        switch (j)
                        {
                            case 0:
                                type = t0[Main.rand.Next(t0.Length)]; 
                                chest.item[j].SetDefaults(type);
                                break;
                            case 1:
                                type = t1[Main.rand.Next(t1.Length)]; 
                                if (type == t1[0])
                                {
                                    chest.item[j].SetDefaults(t1[0]);
                                    chest.item[j].stack = Main.rand.Next(8, 15);
                                    break;
                                }
                                if (fossils < 2)
                                {
                                    if (type == t1[3])
                                    {
                                        chest.item[j].SetDefaults(t1[3]);
                                        fossils++;
                                    }
                                }
                                break;
                            case 2:
                                type = t2[Main.rand.Next(t2.Length)]; 
                                chest.item[j].SetDefaults(type);
                                chest.item[j].stack = Main.rand.Next(6, 13);
                                break;
                            case 3:
                                type = t3[Main.rand.Next(t3.Length)]; 
                                chest.item[j].SetDefaults(type);
                                chest.item[j].stack = Main.rand.Next(1, 4);
                                break;
                        }
                    }
                }
                if (chest.y < Main.spawnTileY && Main.tile[chest.x, chest.y].frameX == 0)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int type = 0;
                        switch (j)
                        {
                            case 0:
                                type = s0[Main.rand.Next(s0.Length)]; 
                                chest.item[j].SetDefaults(type);
                                break;
                            case 1:
                                type = s1[Main.rand.Next(s1.Length)]; 
                                if (type == s1[0])
                                {
                                    chest.item[j].SetDefaults(t1[0]);
                                    chest.item[j].stack = Main.rand.Next(8, 15);
                                }
                                break;
                            case 2:
                                type = s2[Main.rand.Next(s2.Length)]; 
                                chest.item[j].SetDefaults(type);
                                chest.item[j].stack = Main.rand.Next(6, 13);
                                break;
                            case 3:
                                type = s3[Main.rand.Next(s3.Length)]; 
                                chest.item[j].SetDefaults(type);
                                chest.item[j].stack = Main.rand.Next(1, 4);
                                break;
                        }
                    }
                }
            }
        }
        public bool MagnoBiome;
        public bool SkyFort;
        public bool nearMusicBox;
        public bool SkyPortal;
        public override void TileCountsAvailable(int[] tileCounts)
        {
            MagnoBiome = tileCounts[magnoStone] >= 100;
            SkyFort = tileCounts[skyBrick] >= 80;
            SkyPortal = tileCounts[mod.TileType<Tiles.sky_portal>()] != 0;
            nearMusicBox = tileCounts[mod.TileType<Tiles.music_boxes>()] != 0;
        }
        public bool cordonBounds = false;
        private bool spawnedCrystals;
        public static int worldID;
        public static List<int> classes = new List<int>();
        public static List<int> playerIDs = new List<int>();
        public override TagCompound Save()
        {
            return new TagCompound {
                { "m_downed", downedMagno },
                { "First", first },
                { "Classes", classes },
                { "IDs", playerIDs },
                { "Crystals", spawnedCrystals }
            };
        }
        public override void Load(TagCompound tag)
        {
            downedMagno = tag.GetBool("m_downed");
            first = tag.GetBool("First");
            classes = tag.Get<List<int>>("Classes");
            playerIDs = tag.Get<List<int>>("IDs");
            spawnedCrystals = tag.GetBool("Crystals");
        }
        private bool begin;
        private bool first;
        public static Player firstPlayer;
        public override void PreUpdate()
        {
            if (!first)
            {
                if (firstPlayer == null)
                {
                    foreach (Player p in Main.player.Where(t => t != null))
                    {
                        if (p.active)
                            firstPlayer = p;
                    }
                }
                else if (firstPlayer.active)
                {
                    first = true;
                }
            }
            if (ArchaeaPlayer.KeyPress(Keys.E))
                begin = false;
            if (ArchaeaPlayer.KeyPress(Keys.Q))
            {
                if (!begin)
                {
                    //s = new Structures.Magno();
                    //t = new Treasures();
                    /*
                    Vector2 position;
                    do
                    {
                        position = new Vector2(WorldGen.genRand.Next(200, Main.maxTilesX - 200), 50);
                    } while (position.X < Main.spawnTileX + 150 && position.X > Main.spawnTileX - 150);
                    var s = new Structures(position, skyBrick, skyBrickWall);
                    s.InitializeFort();
                    begin = true;*/
                }
                //s.Initialize();
                //s.tileID = magnoBrick;
                //s.wallID = magnoBrickWall;
                //Player player = Main.LocalPlayer;
                //s.MagnoHouse(new Vector2(player.position.X / 16, player.position.Y / 16));
            }
            if (Main.hardMode && !spawnedCrystals)
            {
                t = new Treasures();
                int place = 0;
                int width = Main.maxTilesX - 100;
                int height = Main.maxTilesY - 100;
                Vector2[] any = Treasures.FindAll(new Vector2(100, 100), width, height, false, new ushort[] { magnoStone });
                foreach (Vector2 floor in any)
                {
                    if (floor != Vector2.Zero)
                    {
                        int i = (int)floor.X;
                        int j = (int)floor.Y;
                        t.PlaceTile(i, j, crystalLarge, true, false, 8);
                    }
                }
                spawnedCrystals = true;
            }
        }
        public static bool Inbounds(int i, int j)
        {
            return i < Main.maxTilesX - 50 && i > 50 && j < Main.maxTilesY - 200 && j > 50;
        }
        public static void Clamp(ref int input, int min, int max, out int result)
        {
            if (input < min)
                input = min;
            if (input > max)
                input = max;
            result = input;
        }
    }
    
    public class Treasures
    {
        public int offset;
        private ushort floorID;
        private ushort newTileID;
        private ushort wallID;
        private List<Vector2> list;
        public void Initialize(int offset, ushort newTileID, ushort floorID, ushort wallID)
        {
            this.offset = offset;
            this.newTileID = newTileID;
            this.floorID = floorID;
            this.wallID = wallID;
            list = ArchaeaWorld.origins;
        }
        public void PlaceChests(int total, int retries)
        {
            int index = 1;
            int count = 0;
            int loop = 0;
            var getFloor = GetFloor();
            int length = list.Count;
            bool[] added = new bool[length];
            while (count < total)
            {
                if (loop < total * retries)
                    loop++;
                else
                {
                    index++;
                    loop = 0;
                }
                foreach (Vector2 ground in getFloor[index - 1])
                {
                    int x = (int)ground.X;
                    int y = (int)ground.Y;
                    if (!ArchaeaWorld.Inbounds(x, y)) continue;
                    if (Main.tile[x, y].wall == wallID && WorldGen.genRand.Next(8) == 0)
                        WorldGen.PlaceTile(x, y, newTileID, true, true);
                    if (Main.tile[x, y].type == newTileID)
                    {
                        added[index] = true;
                        count++;
                        break;
                    }
                }
                if (added[index])
                {
                    index++;
                    loop = 0;
                }
                if (index == length)
                    break;
            }
        }
        public void PlaceTile(Vector2[] region, int total, int retries, ushort newTileID, bool genPlace = true, bool force = false, bool random = false, int odds = 5, bool proximity = false, int radius = 30, bool iterate = false, bool onlyOnWall = false)
        {
            int loop = 0;
            int index = 0;
            var getFloor = region;
            while (index < getFloor.Length)
            {
                if (loop < total * retries)
                    loop++;
                else break;
                if (getFloor[index] == Vector2.Zero)
                {
                    index++;
                    continue;
                }
                int x = (int)getFloor[index].X;
                int y = (int)getFloor[index].Y;
                Tile tile = Main.tile[x, y];
                if (random && WorldGen.genRand.Next(odds) != 0) continue;
                if (onlyOnWall && Main.tile[x, y].wall != wallID)
                {
                    index++;
                    continue;
                }
                if (proximity && Vicinity(getFloor[index], radius, newTileID))
                {
                    index++;
                    continue;
                }
                if (genPlace)
                    WorldGen.PlaceTile(x, y, newTileID, true, force);
                else
                {
                    tile.active(true);
                    tile.type = newTileID;
                }
                if (total == 1 && tile.type == newTileID && tile.active())
                    break;
                if (iterate && index == getFloor.Length - 1)
                    index = 0;
                index++;
            }
        }
        public bool PlaceTile(int i, int j, ushort tileType, bool genPlace = false, bool force = false, int proximity = -1, bool wall = false, int style = 0)
        {
            Tile tile = Main.tile[i, j];
            if (proximity != -1 && Vicinity(new Vector2(i, j), proximity, tileType))
                return false;
            if (!genPlace)
            {
                tile.active(true);
                tile.type = tileType;
            }
            else WorldGen.PlaceTile(i, j, tileType, true, force, -1, style);
            if (tile.type == tileType)
                return true;
            return false;
        }
        public Vector2[][] GetFloor()
        {
            int index = 0;
            int count = 0;
            int length = list.Count;
            var tiles = new Vector2[length][];
            for (int k = 0; k < length; k++)
                tiles[k] = new Vector2[length * length];
            foreach (Vector2 v2 in list)
            {
                for (int i = (int)v2.X - offset; i < (int)v2.X + offset; i++)
                    for (int j = (int)v2.Y - offset; j < (int)v2.Y + offset; j++)
                    {
                        Tile floor = Main.tile[i, j];
                        Tile ground = Main.tile[i, j + 1];
                        if ((!floor.active() || !Main.tileSolid[floor.type]) &&
                            ground.active() && Main.tileSolid[ground.type] && ground.type == floorID)
                        {
                            if (count < tiles[index].Length)
                            {
                                tiles[index][count] = new Vector2(i, j);
                                count++;
                            }
                        }
                    }
                count = 0;
                if (index < length)
                    index++;
                else
                    break;
            }
            return tiles;
        }
        public static Vector2[] FindAll(Vector2 region, int width, int height, bool overflow = false, ushort[] floorIDs = null)
        {
             int index = width * height * floorIDs.Length;
            int amount = (int)Math.Sqrt(index) / 10;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort floorType in floorIDs)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        Tile origin = Main.tile[i, j];
                        Tile ceiling = Main.tile[i, j - 1];
                        Tile ground = Main.tile[i, j + 1];
                        Tile right = Main.tile[i + 1, j];
                        Tile ieft = Main.tile[i - 1, j];
                        if (origin.active() && Main.tileSolid[origin.type]) continue;
                        if (ceiling.active() && Main.tileSolid[ceiling.type] && ceiling.type == floorType || 
                            ground.active() && Main.tileSolid[ground.type] && ground.type == floorType || 
                            right.active() && Main.tileSolid[right.type] && right.type == floorType || 
                            ieft.active() && Main.tileSolid[ieft.type] && ieft.type == floorType)
                        {
                            if (count < tiles.Length)
                            {
                                tiles[count] = new Vector2(i, j);
                                count++;
                            }
                        }
                    }
            return tiles;
        }
        public static Vector2[] GetFloor(Vector2 region, int width, int height, bool overflow = false, ushort[] floorIDs = null)
        {
            int index = width * height * floorIDs.Length;
            int amount = (int)Math.Sqrt(index) / 10;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort floorType in floorIDs)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        Tile floor = Main.tile[i, j];
                        Tile ground = Main.tile[i, j + 1];
                        if (floor.active() && Main.tileSolid[floor.type]) continue;
                        if (ground.active() && Main.tileSolid[ground.type] && ground.type == floorType)
                        {
                            if (count < tiles.Length)
                            {
                                tiles[count] = new Vector2(i, j);
                                count++;
                            }
                        }
                    }
            return tiles;
        }
        public static Vector2[] GetCeiling(Vector2 region, int radius, bool overflow = false, ushort tileType = 0)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int count = 0;
            var tiles = new Vector2[index];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile roof = Main.tile[i, j];
                    Tile ceiling = Main.tile[i, j + 1];
                    if (ceiling.active() && Main.tileSolid[ceiling.type]) continue;
                    if (roof.active() && Main.tileSolid[roof.type] && roof.type == tileType)
                    {
                        if (count < tiles.Length)
                        {
                            tiles[count] = new Vector2(i, j);
                            count++;
                        }
                    }
                }
            return tiles;
        }
        public static Vector2[] GetCeiling(Vector2 region, int width, int height, bool overflow = false, ushort tileType = 0)
        {
            var tiles = new List<Vector2>();
            for (int i = (int)region.X; i < width; i++)
                for (int j = (int)region.Y; j < height; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile roof = Main.tile[i, j];
                    Tile ceiling = Main.tile[i, j + 1];
                    if (ceiling.active() && Main.tileSolid[ceiling.type]) continue;
                    if (roof.active() && Main.tileSolid[roof.type] && roof.type == tileType)
                        tiles.Add(new Vector2(i, j + 1));
                }
            return tiles.ToArray();
        }
        public static Vector2[] GetRegion(Vector2 region, int width, int height, bool overflow = false, bool attach = false, ushort[] tileTypes = null)
        {
            int index = width * height * tileTypes.Length;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort tileType in tileTypes)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (count >= tiles.Length) continue;
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (attach && Main.tile[i, j].type != tileType) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        tiles[count] = new Vector2(i, j);
                        count++;
                    }
            return tiles;
        }
        public static Vector2[] GetWall(Vector2 region, int width, int height, bool overflow = false, bool attach = false, ushort[] attachTypes = null)
        {
            int index = width * height * attachTypes.Length;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort tileType in attachTypes)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (count >= tiles.Length) continue;
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        Tile tile = Main.tile[i, j];
                        Tile wallL = Main.tile[i - 1, j];
                        Tile wallR = Main.tile[i + 1, j];
                        if (wallL.active() && Main.tileSolid[wallL.type])
                            if (!tile.active() || !Main.tileSolid[tile.type])
                            {
                                if (attach && wallL.type != tileType) continue;
                                tiles[count] = new Vector2(i, j);
                            }
                        if (wallR.active() && Main.tileSolid[wallR.type])
                            if (!tile.active() || !Main.tileSolid[tile.type])
                            {
                                if (attach && wallR.type != tileType) continue;
                                tiles[count] = new Vector2(i, j);
                            }
                        count++;
                    }
            return tiles;
        }
        public static Vector2[] GetWall(int x, int y, int width, int height, ushort[] tileTypes = null, int radius = -1)
        {
            int count = 0;
            List<Vector2> list = new List<Vector2>();
            foreach (ushort tileType in tileTypes)
                for (int i = x; i < width; i++)
                    for (int j = y; j < width; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j))
                            continue;
                        if (radius != -1 && Vicinity(new Vector2(i, j), radius, tileType))
                            continue;
                        Tile up = Main.tile[i, j - 1];
                        Tile left = Main.tile[i - 1, j];
                        Tile right = Main.tile[i + 1, j];
                        if ((left.type == tileType || right.type == tileType) && !up.active())
                        {
                            list.Add(new Vector2(i, j));
                            count++;
                        }
                    }
            return list.ToArray();
        }
        public static bool Vicinity(Vector2 region, int radius, ushort tileType)
        {
            int x = (int)region.X;
            int y = (int)region.Y;
            for (int i = x - radius; i < x + radius; i++)
                for (int j = y - radius; j < y + radius; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    if (Main.tile[i, j].type == tileType)
                        return true;
                }
            return false;
        }
        public static int Vicinity(Vector2 region, int radius, ushort[] tileType)
        {
            Func<int> count = delegate ()
            {
                int x = (int)region.X;
                int y = (int)region.Y;
                int tiles = 0;
                for (int i = x - radius; i < x + radius; i++)
                    for (int j = y - radius; j < y + radius; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        foreach (ushort type in tileType)
                            if (Main.tile[i, j].type == type && Main.tile[i, j].active())
                            {
                                tiles++;
                                break;
                            }
                    }
                return tiles;
            };
            return count();
        }
        public static int ProximityCount(Vector2 region, int radius, ushort tileType)
        {
            int x = (int)region.X;
            int y = (int)region.Y;
            int count = 0;
            for (int i = x - radius; i < x + radius; i++)
                for (int j = y - radius; j < y + radius; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    Tile tile = Main.tile[i, j];
                    if (tile.type == tileType)
                        count++;
                }
            return count;
        }
        public static bool ActiveAndSolid(int i, int j)
        {
            return Main.tile[i, j].active() && Main.tileSolid[Main.tile[i, j].type];
        }
    }
    public class Structures
    {
        internal bool[] direction;
        public static int index;
        private int count;
        private const int max = 3;
        public int[][,] house;
        public int[][,] rooms;
        public int[][,] fort;
        public int[,] island;
        public int[,] tower;
        public ushort tileID;
        public ushort wallID;
        private ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
        private ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
        private ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
        private Vector2 origin;
        private List<Vector2> path;
        class ID
        {
            public const int
                Dirt = -2,
                Grass = -1,
                Empty = 0,
                Tile = 1,
                Wall = 2,
                Platform = 3,
                Stairs = 4,
                Floor = 5,
                Door = 6,
                Decoration = 7,
                Furniture = 8,
                Useful = 9,
                Lamp = 10,
                Chest = 11,
                Cloud = 12,
                Trap = 13,
                Danger = 14,
                Wire = 15,
                Window = 16,
                Light = 17,
                Dark = 18,
                WallHanging = 19,
                Portal = 20;
        }
        class RoomID
        {
            public const int
                Empty = 0,
                FilledIn = 1,
                Safe = 2,
                Danger = 3,
                Chest = 4,
                Platform = 5,
                Start = 6,
                End = 7,
                Lighted = 8,
                Decorated = 9;
        }
        class FortID
        {
            public const int
                Light = 0,
                Dark = 1;
        }
        public Structures(Vector2 origin = default(Vector2), ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
        {
            this.tileID = tileID;
            this.wallID = wallID;
            this.origin = origin;
        }
        public void InitializeFort()
        {
            int radius = 105;
            int lX = radius - 45;
            int lY = radius - 6;
            int cloud = radius + 75;
            int center = cloud / 2;
            int roomX = 15;
            int roomY = 9;
            index = -1;
            for (int i = 600; i < 600 + cloud; i++)
                for (int j = 50; j < 50 + lY + cloud / 8; j++)
                    WorldGen.KillTile(i, j, false, false, true);

            //island = new int[cloud, cloud / 2];
            //InitIsland();
            FortPathing(radius, roomX, roomY, lY);

            int lengthX = fort[0].GetLength(0);
            int lengthY = fort[0].GetLength(1);
            int[] roomTypes = new int[] { RoomID.Chest, RoomID.Danger, RoomID.Decorated, RoomID.Lighted };

            int x1, x2, x3;
            int y1 = 50, y2 = y1, y3;
            int[,] cRoom;
            //GenerateStructure(600, 50 + fort[0].GetLength(1), island);
            GenerateStructure(x1 = (int)origin.X + center - lengthX / 2, 50, fort[0], tileID, wallID);
            GenerateStructure(x2 = (int)origin.X + center + lengthX / 2, 50, fort[1], tileID, wallID);
            GenerateStructure(x3 = (int)origin.X + center + lengthX / 2 + lengthX, y3 = (int)(50 + lengthY * 0.33f), cRoom = Chamber((int)(lengthY * 0.67f)), tileID, wallID);
            ChamberRoof(x3, y1, cRoom.GetLength(0), (int)(lengthY * 0.33f));

            PlaceInteriorRooms(new Vector2((int)origin.X + center - lengthX / 2, 50), roomX, roomY, fort[0], roomTypes);
            PlaceInteriorRooms(new Vector2((int)origin.X + center + lengthX / 2, 50), roomX, roomY, fort[1], roomTypes);
            house = new int[max][,];
            rooms = new int[max - 1][,];
            
            house[0] = fort[0];
            SkyRoom();

            GenConnect(x1, 50 + lengthY - 10, 20, 8);
            GenConnect(x1 + lengthX - 15, 50 + 5, 20, 8);
            GenConnect(x2, 50 + 5, 10, 8);
            GenConnect(x2 + lengthX - 50, 50 + lengthY - 11, 40 + cRoom.GetLength(0) / 2, 6);
            GenConnect(x3 + cRoom.GetLength(0) / 2 - 10, 50 + lengthY - 20, 8, 14);
            Main.NewText("Gen complete");
        }
        internal void ChamberRoof(int x, int y, int width, int height)
        {
            float n = y;
            Main.NewText(x + " " + y + " " + width + " " + height);
            for (int i = x; i < x + width; i++)
            {
                n += (float)height / width;
                for (int j = (int)n; j < y + height; j++)
                {
                    Main.tile[i, j].active(true);
                    Main.tile[i, j].type = tileID;
                }
            }
        }
        internal void GenConnect(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
                for (int j = y; j < y + height; j++)
                {
                    Main.tile[i, j].active(false);
                    Main.tile[i, j].wall = wallID;
                }
        }
        internal int[,] Chamber(int height)
        {
            int width = 100;
            int[,] room = new int[width, height];
            for (int i = 0; i < room.GetLength(0); i++)
            for (int j = 0; j < height; j++)
            {
                room[i,j] = ID.Tile;
            }
            for (int i = 0; i < width / 4; i++)
            for (int j = 0; j < height / 2 - 5; j++)
            {
                for (float r = 0f; r < 360f; r++)
                {
                    int x = (int)(width / 2 + i * Math.Cos(r));
                    int y = (int)((height / 2 - 5) + j * Math.Sin(r));
                    room[x, y] = ID.Empty;
                }
            }
            room[width / 2 - 1, height / 2 - 1] = ID.Portal;
            return room;
        }
        internal void InitIsland()
        {
            int lengthX = island.GetLength(0);
            int lengthY = island.GetLength(1);
            int margin = 10;
            for (int k = 0; k < lengthX; k++)
            {
                int j = (int)Math.Min(lengthY - 1, Math.Abs(lengthY * Math.Sin(k * Draw.radian)));
                for (int l = 0; l < j; l++)
                    island[k, l] = ID.Cloud;
                for (int m = 0; m < j / 2; m++)
                    island[k, m] = ID.Dirt;
            }
            for (int k = margin; k < lengthX - margin; k++)
                island[k, 0] = ID.Grass;
        }
        internal void InitTower()
        {
            int lengthX = tower.GetLength(0) - 1;
            int lengthY = tower.GetLength(1) - 1;
            int margin = 5;
            int width = lengthX - margin * 2;
            int top = 15;
            for (int m = margin; m < width; m++)
                for (int n = lengthY - 1; n > top; n--)
                    tower[m, n] = ID.Tile;
            List<Vector2> move = new List<Vector2>();
            for (int j = lengthY; j > top; j--)
                move.Add(new Vector2(margin + (float)(width - width * Math.Abs(Math.Cos(j * Draw.radian))), j));
            foreach (Vector2 v in move)
            {
                for (int k = 0; k < 4; k++)
                    for (int l = 0; l < 6; l++)
                    {
                        int i = (int)v.X + k;
                        int j = (int)v.Y + l;
                        if (i > margin && i < width && l < lengthY && l > top)
                            tower[i, j] = ID.Empty;
                    }
            }
            for (int m = 0; m <= lengthX; m++)
                for (int n = 0; n <= top; n++)
                {
                    if (m == 0 || m == lengthX - 1 || n == 0 || n == top)
                        tower[m, n] = ID.Tile;
                    else tower[m, n] = ID.Wall;
                }
        }
        public void GenerateStructure(int x, int y, int[,] structure, ushort tileType = 0, ushort wallType = 0)
        {
            int lengthX = structure.GetLength(0);
            int lengthY = structure.GetLength(1);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                {
                    int m = i + x;
                    int n = j + y;
                    Tile tile = Main.tile[m, n];
                    switch (structure[i, j])
                    {
                        case ID.Empty:
                            WorldGen.KillTile(m, n, false, false, true);
                            goto case ID.Wall;
                        case ID.Tile:
                            WorldGen.PlaceTile(m, n, tileType, true, true);
                            break;
                        case ID.Wall:
                            WorldGen.PlaceWall(m, n, wallType, true);
                            break;
                        case ID.Dirt:
                            tile.active(true);
                            tile.type = TileID.Dirt;
                            break;
                        case ID.Grass:
                            tile.active(true);
                            tile.type = TileID.Grass;
                            break;
                        case ID.Cloud:
                            tile.active(true);
                            tile.type = TileID.Cloud;
                            break;
                        case ID.Portal:
                            WorldGen.Place3x3Wall(m, n, (ushort)ArchaeaMain.getMod.TileType<ArchaeaMod.Tiles.sky_portal>(), 0);
                            goto case ID.Wall;
                    }
                }
        }
        internal void FortPathing(int radius, int roomX, int roomY, int lY)
        {
            fort = new int[2][,];
            int Width = (radius - 45) * 2;
            for (int k = 0; k < 2; k++)
            {
                fort[k] = new int[Width, lY];
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < lY; j++)
                        fort[k][i, j] = ID.Tile;
                bool direction = k == 0;
                bool genRoom = false;
                int m = 0;
                int n = 0;
                int width = Width / 5 - 3;
                int margin = roomY * 2;
                int space = roomX / 3;
                int moved = 0;
                int start;
                int room = 0;
                int roomHeight = 4;
                Vector2 move = new Vector2(k == 0 ? 5 : Width - 30, lY - roomY);
                for (int total = 0; total < lY / margin; total++)
                {
                    int spacing = WorldGen.genRand.Next(8, 12);
                    for (moved = 0; moved < Width - 20; moved += space)
                    {
                        if ((total == 0 && moved > (Width / 1.5f)) || (total == 1 && move.X < 10 && !direction))
                            break;
                        move.X += direction ? space : space * -1;
                        space = roomX / (WorldGen.genRand.NextBool() ? 3 : 5);
                        int height = WorldGen.genRand.Next(5, roomY - 1);
                        start = WorldGen.genRand.Next(-3, 1);
                        for (int j = genRoom ? start - roomHeight : start; j < height; j++)
                            for (int i = 0; i < 5; i++)
                            {
                                m = (int)move.X + i;
                                n = (int)move.Y + j;
                                ArchaeaWorld.Clamp(ref m, 3, Width - 3, out m);
                                ArchaeaWorld.Clamp(ref n, 3, lY - 3, out n);
                                fort[k][m, n] = ID.Empty;
                            }
                        room++;
                        if (room > (genRoom ? spacing + 4 : spacing))
                        {
                            genRoom = !genRoom;
                            room = 0;
                        }
                    }
                    int destination = (int)move.Y - margin;
                    int oldY = (int)move.Y;
                    for (move.Y = oldY; move.Y > destination; move.Y--)
                    {
                        start = WorldGen.genRand.Next(-2, 2);
                        for (int i = start; i < 5 + start; i++)
                            for (int j = 0; j < 5; j++)
                            {
                                m = (int)move.X + i;
                                n = (int)move.Y + j;
                                ArchaeaWorld.Clamp(ref m, 3, Width - 3, out m);
                                ArchaeaWorld.Clamp(ref n, 3, lY - 3, out n);
                                fort[k][m, n] = ID.Empty;
                            }
                    }
                    direction = !direction;
                }
            }
        }
        internal void PlaceInteriorRooms(Vector2 origin, int roomX, int roomY, int[,] structure, int[] types)
        {
            int lengthX = structure.GetLength(0);
            int lengthY = structure.GetLength(1);
            int[,] rooms = new int[lengthX / roomX, lengthY / roomY];
            bool[,] placed = new bool[lengthX / roomX, lengthY / roomY];
            for (int i = 0; i < lengthX / roomX; i++)
                for (int j = 0; j < lengthY / roomY; j++)
                    rooms[i, j] = WorldGen.genRand.Next(types);
            for (int j = 0; j < lengthY; j++)
                for (int i = 0; i < lengthX; i++)
                {
                    int x = (int)origin.X + i;
                    int y = (int)origin.Y + j;
                    int m = i / roomX;
                    int n = j / roomY;
                    switch (rooms[m, n])
                    {
                        case RoomID.Chest:
                            if (!placed[m, n])
                                WorldGen.PlaceChest(x, y);
                            if (IsPlaced(x, y, TileID.Containers))
                                placed[m, n] = true;
                            break;
                        case RoomID.Danger:
                            if (structure[i, Math.Max(j - 1, 0)] == ID.Empty && structure[i, j] == ID.Tile)
                                WorldGen.PlaceTile(x, y, TileID.Spikes);
                            break;
                        case RoomID.Decorated:
                            if (WorldGen.genRand.NextBool())
                                WorldGen.PlaceTile(x, y, WorldGen.genRand.Next(decoration));
                            break;
                        case RoomID.Lighted:
                            if (!Treasures.Vicinity(new Vector2(x, y), 15, TileID.HangingLanterns))
                                WorldGen.PlaceTile(x, y, TileID.HangingLanterns);
                            break;
                    }
                    WorldGen.PlaceWall(i, j, wallID);
                }
        }
        internal bool IsPlaced(int i, int j, ushort tile)
        {
            return Main.tile[i, j].type == tile && Main.tile[i, j].active();
        }
        public void DesignateRoom(int i, int j, int roomX, int roomY, int index)
        {
            index = 0;
            bool lamp = false, 
                useful = false, 
                placed = false;
            int q = i;
            int r = j;
            int m = 0;
            int n = 0;
            int width = q + roomX;
            int height = r + roomY;
            for (int y = r; y < r + roomY; y++)
            {
                if (y < roomY || y >= fort.GetLength(1) - roomY)
                    continue;
                for (int x = q; x < q + roomX; x++)
                {
                    if (x < roomX || x >= fort.GetLength(0) - roomX)
                        continue;
                    int[] choice = new int[] { RoomID.Safe, RoomID.Decorated, RoomID.Lighted, RoomID.Chest, RoomID.Danger };
                    int rand = WorldGen.genRand.Next(choice);
                    m = x;
                    n = y;
                    switch (rand)
                    {
                        case -2:
                            break;
                        case -1:
                            break;
                        case RoomID.Safe:
                            for (int t = 0; t < 5; t++)
                            {
                                int ground = q + WorldGen.genRand.Next(1, 11);
                                if (ground % 2 == 0)
                                {
                                    while (fort[index][ground, n] != ID.Wall)
                                        n++;
                                    n--;
                                    if (!useful)
                                    {
                                        fort[index][ground, n] = ID.Useful;
                                        useful = true;
                                    }
                                    fort[index][ground, n] = WorldGen.genRand.Next(new int[] { ID.Decoration, ID.Furniture });
                                }
                            }
                            goto case RoomID.Lighted;
                        case RoomID.Decorated:
                            if (WorldGen.genRand.NextFloat() > 0.67f)
                                fort[index][m, n] = ID.WallHanging;
                            for (int t = 0; t < 4; t++)
                            {
                                int ground = q + WorldGen.genRand.Next(1, 11);
                                while (fort[index][ground, n] != ID.Wall)
                                    n++;
                                n--;
                                if (ground % 2 == 0)
                                    fort[index][ground, n] = ID.Decoration;
                            }
                            goto case -1;
                        case RoomID.Lighted:
                            if (!lamp)
                            {
                                int roof = q + WorldGen.genRand.Next(2, 10);
                                fort[index][roof, y] = ID.Lamp;
                                lamp = true;
                            }
                            goto case -1;
                        case RoomID.Chest:
                            for (int t = 0; t < 4; t++)
                            {
                                int ground = i + WorldGen.genRand.Next(1, 11);
                                while (fort[index][ground, n] != ID.Wall)
                                    n++;
                                n--;
                                if (ground % 2 == 1)
                                {
                                    if (fort[index][ground, n] == ID.Empty)
                                        fort[index][ground, n] = ID.Decoration;
                                    if (!placed)
                                    {
                                        fort[index][ground, n] = ID.Chest;
                                        placed = true;
                                    }
                                }
                            }
                            goto case -1;
                        case RoomID.Danger:
                            goto case -1;
                    }
                }
            }
        }
        public Vector2 PlaceRoom(ref int i, ref int j, int index)
        {
            int width = WorldGen.genRand.Next(10, 22);
            int height = WorldGen.genRand.Next(10, 12);
            Vector2 origin = Vector2.Zero;
            Main.NewText(i + " " + j);
            for (int k = i; k < i + width; k++)
            {
                if (origin != Vector2.Zero)
                    break;
                for (int l = j; l < j + height; l++)
                    if (k < fort.GetLength(0) && l < fort.GetLength(1))
                        if (fort[index][k, l] == ID.Empty && origin == Vector2.Zero)
                        {
                            origin = new Vector2(k, l);
                            i = k;
                            j = l;
                            break;
                        }
            }
            Draw draw = new Draw();
            for (int k = i; k < i + width; k++)
                for (int l = j; l < j + height; l++)
                    if (k < fort.GetLength(0) && l < fort.GetLength(1))
                        fort[index][k, l] = ID.Empty;
            for (float radius = 0; radius < height; radius++)
                for (float r = -(float)Math.PI; r <= 0; r += draw.radians(radius))
                {
                    Vector2 c = NPCs.ArchaeaNPC.AngleBased(new Vector2(i + width / 2, j + height / 2), r, radius);
                    if (c.X < fort.GetLength(0) && c.Y < fort.GetLength(1))
                        fort[index][(int)c.X, (int)c.Y] = ID.Empty;
                }
            return origin;
        }
        public void CloudForm(int i, int j)
        {
            int width = 90;
            var d = new Draw();
            for (int m = 0; m < width; m++)
                for (int n = 0; n < width / 2; n++)
                {
                    if (WorldGen.genRand.Next(25) == 0)
                        for (float k = 0; k < 12; k += 12 * Draw.radian)
                            for (float r = 0f; r < Math.PI * 2f; r += d.radians(k))
                            {
                                int cos = (int)(i + m + k * Math.Cos(r));
                                int sine = (int)(j + n + k / 2f * Math.Sin(r));
                                Main.tile[cos, sine].active(true);
                                Main.tile[cos, sine].type = TileID.Cloud;
                            }
                }
        }
        public void RoomItems(int k, int index = 0, int roomX = 15, int roomY = 9, bool genWalls = false)
        {
            for (int i = 0; i < rooms[k].GetLength(0); i++)
                for (int j = 0; j < rooms[k].GetLength(1); j++)
                {
                    int m = i * roomX;
                    int n = j * roomY;
                    int width = m + roomX;
                    int height = n + roomY;
                    int added = 0;
                    int floor;
                    bool placed;
                    for (int q = m; q < width; q++)
                        for (int r = n; r < height; r++)
                            switch (rooms[k][i, j])
                            {
                                case -2:
                                    if (genWalls)
                                    {
                                        int door = 0;
                                        if (rooms[k][Math.Max(i - 1, 0), j] != RoomID.FilledIn)
                                            if (i != 0 && q == m && r < height)
                                            {
                                                if (j != rooms[k].GetLength(1) - 1 && r >= height - 3)
                                                    house[index][q, r] = ID.Empty;
                                                else if (j == rooms[k].GetLength(1) - 1)
                                                {
                                                    door = 1;
                                                    if (r >= height - 4 && r < height - 1)
                                                        house[index][q, r] = ID.Empty;
                                                }
                                                else door = 0;
                                                if (r == height - 1 - door)
                                                    house[index][q, r] = ID.Door;
                                            }
                                    }
                                    if (rooms[k][i, j] == RoomID.Start)
                                    {
                                        if (q == (k == FortID.Light ? m : width - 1) && r >= height - 4 && r < height - 1)
                                        {
                                            house[index][q, r] = ID.Empty;
                                            if (r == height - 3)
                                                house[index][q, r] = ID.Door;
                                        }
                                        if (q > m + 5 && q < m + 10 && r == n)
                                            house[index][q, r] = ID.Platform;
                                    }
                                    if (rooms[k][i, j] == RoomID.End)
                                        if (q == (k == FortID.Light ? width - 1 : m) && r >= height - 3 && r < height)
                                        {
                                            if (r == height - 2)
                                                house[index][q, r] = ID.Door;
                                            else house[index][q, r] = ID.Empty;
                                        }
                                    break;
                                case -1:
                                    if (genWalls)
                                    {
                                        if ((i == 0 && q == m) || (i == rooms[k].GetLength(0) - 1 && q == width - 1))
                                            house[index][q, r] = ID.Wall;
                                        if (rooms[k][Math.Max(i - 1, 0), j] != RoomID.FilledIn)
                                            if (i != 0 && q == m)
                                            {
                                                house[index][q, r] = ID.Wall;
                                                goto case -2;
                                            }
                                    }
                                    break;
                                case RoomID.Empty:
                                    goto case RoomID.Decorated;
                                case RoomID.Start:
                                    goto case -2;
                                case RoomID.End:
                                    goto case -2;
                                case RoomID.FilledIn:
                                    house[index][q, r] = ID.Wall;
                                    break;
                                case RoomID.Platform:
                                    if (q > m + 5 && q < m + 10 && r == n)
                                        house[index][q, r] = ID.Platform;
                                    goto case -1;
                                case RoomID.Safe:
                                    added = 0;
                                    bool useful = false;
                                    if (j == rooms[k].GetLength(1) - 1)
                                        floor = 2;
                                    else floor = 1;
                                    if (q == m + 1 && r == height - floor)
                                    {
                                        while (added < 5)
                                        {
                                            int ground = q + WorldGen.genRand.Next(1, 11);
                                            if (ground % 2 == 0)
                                            {
                                                if (!useful)
                                                {
                                                    house[index][ground, r] = ID.Useful;
                                                    useful = true;
                                                }
                                                house[index][ground, r] = WorldGen.genRand.Next(new int[] { ID.Decoration, ID.Furniture });
                                                added++;
                                            }
                                        }
                                    }
                                    goto case RoomID.Lighted;
                                case RoomID.Decorated:
                                    added = 0;
                                    if (j == rooms[k].GetLength(1) - 1)
                                        floor = 2;
                                    else floor = 1;
                                    if (q == m + 1 && r == height - floor)
                                    {
                                        while (added < 4)
                                        {
                                            int ground = q + WorldGen.genRand.Next(1, 11);
                                            if (ground % 2 == 0)
                                            {
                                                house[index][ground, r] = ID.Decoration;
                                                added++;
                                            }
                                        }
                                    }
                                    goto case -1;
                                case RoomID.Lighted:
                                    bool lamp = false;
                                    if (q == m && r == n + 1)
                                    {
                                        if (!lamp)
                                        {
                                            int roof = q + WorldGen.genRand.Next(2, 10);
                                            house[index][roof, r] = ID.Lamp;
                                            lamp = true;
                                        }
                                    }
                                    goto case -1;
                                case RoomID.Chest:
                                    added = 0;
                                    placed = false;
                                    if (j == rooms[k].GetLength(1) - 1)
                                        floor = 2;
                                    else floor = 1;
                                    if (q == m + 3 && q < width - 3 && r == height - floor)
                                    {
                                        while (added < 4)
                                        {
                                            int ground = q + WorldGen.genRand.Next(1, 11);
                                            if (ground % 2 == 1)
                                            {
                                                if (house[index][ground, r] == ID.Empty)
                                                {
                                                    house[index][ground, r] = ID.Decoration;
                                                    added++;
                                                }
                                                if (!placed)
                                                {
                                                    house[index][ground, r] = ID.Chest;
                                                    placed = true;
                                                }
                                            }
                                        }
                                    }
                                    if (q > m + 3 && q < width - 2 && r >= 3 && r <= 3)
                                        house[index][q, r] = ID.Window;
                                    goto case -1;
                                case RoomID.Danger:
                                    goto case -1;
                            }
                }
        }
        public void SkyRoom(bool killRegion = false)
        {
            int m;
            int n;
            int w = 0;
            int k = 0;
            int width = house[0].GetLength(0);
            int lengthX = house[k].GetLength(0);
            int lengthY = house[k].GetLength(1);
            ushort type;
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                {
                    m = (int)origin.X + i;
                    n = (int)origin.Y + j;
                    if (killRegion)
                        WorldGen.KillTile(m, n, false, false, true);
                    Tile tile = Main.tile[m, n];
                    if (house[k][i, j] == ID.Wall)
                    {
                        tile.active(true);
                        tile.type = tileID;
                    }
                    if (house[k][i, j] != ID.Wall && house[k][i, j] != ID.Door && house[k][i, j] != ID.Window)
                            WorldGen.PlaceWall(m, n, wallID);
                }
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                {
                    m = (int)origin.X + i;
                    n = (int)origin.Y + j;
                    Tile tile = Main.tile[m, n];
                    switch (house[k][i, j])
                    {
                        case -2:
                            tile.active(false);
                            break;
                        case -1:
                            tile.active(true);
                            break;
                        case ID.Cloud:
                            tile.type = TileID.Cloud;
                            goto case -1;
                        case ID.Floor:
                            tile.type = tileID;
                            goto case -1;
                        case ID.Platform:
                            tile.type = TileID.Platforms;
                            goto case -1;
                        case ID.Chest:
                            WorldGen.PlaceChest(m, n);
                            break;
                        case ID.Furniture:
                            type = furniture[WorldGen.genRand.Next(furniture.Length)];
                            WorldGen.PlaceTile(m, n, type);
                            break;
                        case ID.Useful:
                            type = useful[WorldGen.genRand.Next(useful.Length)];
                            if (!Treasures.Vicinity(origin, 20, type))
                                WorldGen.PlaceTile(m, n, type);
                            break;
                        case ID.Decoration:
                            type = decoration[WorldGen.genRand.Next(decoration.Length)];
                            WorldGen.PlaceTile(m, n, type);
                            break;
                        case ID.Lamp:
                            WorldGen.PlaceWall(m, n, wallID);
                            WorldGen.PlaceTile(m, n, TileID.HangingLanterns);
                            break;
                        case ID.Door:
                            tile.active(false);
                            tile.type = 0;
                            WorldGen.PlaceWall(m, n, wallID);
                            WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                            break;
                        case ID.Window:
                            WorldGen.PlaceWall(m, n, (ushort)WorldGen.genRand.Next(88, 93));
                            break;
                        case ID.Light:
                            tile.type = ArchaeaWorld.skyBrick;
                            goto case -1;
                        case ID.Dark:
                            tile.type = ArchaeaWorld.skyBrick;
                            goto case -1;
                        case ID.WallHanging:
                            WorldGen.Place3x2Wall(m, n, TileID.WeaponsRack, 0);
                            break;
                    }
                }
        }
        public class Magno : Structures
        {
            public void Initialize()
            {
                house = new int[max][,];
                bool rand = WorldGen.genRand.Next(2) == 0;
                direction = new bool[] { !rand, rand, !rand };
                int randFloor = WorldGen.genRand.Next(max);
                bool craft = false;
                bool chest = false;
                for (int k = 0; k < max; k++)
                {
                    int randX = k != 1 ? WorldGen.genRand.Next(15, 28) : WorldGen.genRand.Next(15, 20);
                    int randY = WorldGen.genRand.Next(7, 9);
                    int numLights = 0;
                    int numObjects = 0;
                    bool furniture = false;
                    bool stairs = false;
                    house[k] = new int[randX, randY];
                    for (int i = 0; i < randX; i++)
                        for (int j = 0; j < randY; j++)
                        {
                            if (i == 0 || i == randX - 1 || (k != 1 && (j == 0 || j == randY - 1)))
                                house[k][i, j] = tileID;
                            if (k != 1 && (i == 0 || i == randX - 1) && j == randY - 3)
                                house[k][i, j] = ID.Door;
                            if (i > 0 && i < randX - 1)
                            {
                                int x;
                                int y;
                                int count = 0;
                                int top = direction[k] ? 8 : 12;
                                if (i == top && j == 1)
                                {
                                    while (!stairs)
                                    {
                                        if (count == randY - 1)
                                            stairs = true;
                                        if (direction[k] && i + count < randX && j + count < randY)
                                            house[k][i + count, j + count] = ID.Stairs;
                                        if (!direction[k] && i - count >= 0 && j + count < randY)
                                            house[k][i - count, j + count] = ID.Stairs;
                                        count++;
                                    }
                                }
                                while (i == 1 && j == 1 && numLights < 2)
                                {
                                    x = i + WorldGen.genRand.Next(randX - 2);
                                    house[k][x, j] = ID.Lamp;
                                    numLights++;
                                }
                                if (i == 1 && j == randY - 2)
                                {
                                    while (numObjects < 3)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (x % 2 == 1)
                                            if (house[k][x, j] == 0)
                                            {
                                                house[k][x, j] = ID.Decoration;
                                                house[k][x + 1, j] = ID.Empty;
                                                numObjects++;
                                            }
                                    }
                                    while (!craft && k == randFloor)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (house[k][x, j] == 0)
                                        {
                                            house[k][x, j] = ID.Useful;
                                            house[k][x + 1, j] = ID.Empty;
                                            craft = true;
                                        }
                                    }
                                    while (!furniture && k != 1)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (x % 2 == 0)
                                            if (house[k][x, j] == 0)
                                            {
                                                house[k][x, j] = ID.Furniture;
                                                house[k][x + 1, j] = ID.Empty;
                                                furniture = true;
                                            }
                                    }
                                    while (!chest && k == randFloor)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (house[k][x, j] == 0)
                                        {
                                            house[k][x, j] = ID.Chest;
                                            house[k][x + 1, j] = ID.Empty;
                                            chest = true;
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            public bool MagnoHouse(Vector2 origin, bool fail = false)
            {
                if (fail || origin == Vector2.Zero)
                    return false;
                bool success = false;
                int x = (int)origin.X;
                int y = (int)origin.Y;
                if (!ArchaeaWorld.Inbounds(x, y))
                    return false;
                int m = 0;
                int n = 0;
                int height = 0;
                int randFloor = WorldGen.genRand.Next(max);
                for (int k = 0; k < max; k++)
                {
                    int lengthX = house[k].GetLength(0);
                    int lengthY = house[k].GetLength(1);
                    for (int i = 0; i < lengthX; i++)
                        for (int j = 0; j < lengthY; j++)
                        {
                            m = i + x;
                            n = j + y + height;
                            Tile tile = Main.tile[m, n];
                            if (tile.wall == wallID || tile.type == tileID)
                                return false;
                            if (i >= 0 && i < lengthX && j >= 0 && j < lengthY)
                                if (WorldGen.genRand.NextFloat() < 0.50f)
                                {
                                    tile.wall = wallID;
                                    WorldGen.PlaceWall(m, n, wallID, true);
                                }
                            if (house[k][i, j] == tileID)
                            {
                                tile.active(true);
                                tile.type = tileID;
                            }
                            else tile.active(false);
                            if (tile.type == tileID && tile.active())
                                success = true;
                            if (i > 6 && i < 14)
                            {
                                if (k == 0 && (j == 0 || j == lengthY - 1))
                                {
                                    tile.type = TileID.Platforms;
                                    WorldGen.SquareTileFrame(m, n, true);
                                }
                                if (k == max - 1 && j == 0)
                                {
                                    tile.type = TileID.Platforms;
                                    WorldGen.SquareTileFrame(m, n, true);
                                }
                                if (direction[k] && i == 7)
                                    tile.slope(1);
                                else if (!direction[k] && i == 13)
                                    tile.slope(2);
                            }
                        }
                    for (int i = 0; i < lengthX; i++)
                        for (int j = 0; j < lengthY; j++)
                        {
                            m = i + x;
                            n = j + y + height;
                            ushort type;
                            switch (house[k][i, j])
                            {
                                case ID.Door:
                                    if (!Treasures.ActiveAndSolid(m - 1, n) && !Treasures.ActiveAndSolid(m + 1, n))
                                        WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                                    else
                                    {
                                        for (int t = -1; t <= 1; t++)
                                        {
                                            Main.tile[m, n + t].active(true);
                                            Main.tile[m, n + t].type = tileID;
                                        }
                                    }
                                    break;
                                case ID.Stairs:
                                    WorldGen.KillTile(m, n, false, false, true);
                                    WorldGen.PlaceTile(m, n, TileID.Platforms);
                                    Main.tile[m, n].slope((byte)(direction[k] ? 1 : 2));
                                    WorldGen.SquareTileFrame(m, n, true);
                                    break;
                                case ID.Lamp:
                                    WorldGen.PlaceTile(m, n, TileID.HangingLanterns);
                                    break;
                                case ID.Chest:
                                    WorldGen.PlaceChest(m, n, ArchaeaWorld.magnoChest);
                                    break;
                                case ID.Furniture:
                                    type = furniture[WorldGen.genRand.Next(furniture.Length)];
                                    if (!Treasures.Vicinity(origin, 50, type))
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Useful:
                                    type = useful[WorldGen.genRand.Next(useful.Length)];
                                    if (!Treasures.Vicinity(origin, 50, type))
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Decoration:
                                    type = decoration[WorldGen.genRand.Next(decoration.Length)];
                                    if (Treasures.ProximityCount(origin, 50, type) < 5)
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                default:
                                    break;
                            }
                        }
                    height += lengthY;
                }
                index++;
                return success;
            }
        }
        internal void PlaceChest(int i, int j, int width, ushort groundID)
        {
            i -= width;
            j -= 1;
            bool chest = false;
            int count = 0;
            int total = 100;
            while (!chest)
            {
                int m = i + WorldGen.genRand.Next(width - 1);
                Tile floor = Main.tile[m, j];
                Tile ground = Main.tile[m, j + 1];
                if (!floor.active() && ground.type == groundID)
                {
                    WorldGen.PlaceChest(m, j);
                    if (floor.type == ArchaeaWorld.magnoChest)
                        chest = true;
                }
                if (count < total)
                    count++;
                else break;
            }
        }
        internal void Decorate(int i, int j, int radius, ushort tileType)
        {
            var t = new Treasures();
            Vector2 v2 = new Vector2(i, j);
            var floor = Treasures.GetFloor(v2, radius, radius, false, new ushort[] { tileType, TileID.Platforms });
            var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
            ushort[] decoration = new ushort[]  { TileID.Statues, TileID.Pots };
            ushort[] furniture = new ushort[]   { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
            ushort[] useful = new ushort[]      { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
            int length = 0;
            foreach (ushort tile in furniture)
                t.PlaceTile(floor, 2, 30, tile, true, false, false, 0, true, 40);
            length = WorldGen.genRand.Next(useful.Length);
            t.PlaceTile(floor, 1, 30, useful[length], true, false, false, 0, true, 40);
            foreach (ushort tile in decoration)
                t.PlaceTile(floor, 4, 20, tile, true);
            t.PlaceTile(ceiling, 3, 1, TileID.HangingLanterns, true, true);
            t = null;
        }
        public void Reset()
        {
            index = 0;
        }
    }
    public class Digger
    {
        private int max;
        private ushort tileID;
        private ushort wallID;
        private readonly float radians = 0.017f;
        private Vector2[] centers;
        public Digger(int size, ushort tileID, ushort wallID)
        {
            max = size;
            this.tileID = tileID;
            this.wallID = wallID;
        }
        public void DigSequence(Vector2 center)
        {
            int num = 0;
            int border = 0;
            int size = 10;
            float radius = 0f;
            Relocate(ref center, out centers);
            size = WorldGen.genRand.Next(8, 15);
            border = size + 4;
            List<Vector2> list = new List<Vector2>();
            foreach (Vector2 path in centers)
            {
                num++;
                float weight = 0f;
                while (weight < 1f)
                {
                    list.Add(Vector2.Lerp(centers[num - 1], centers[Math.Min(num, centers.Length - 1)], weight));
                    weight += 0.2f;
                }
            }
            foreach (Vector2 lerp in list)
            {
                radius = size;
                while (radius < border)
                {
                    int offset = border / 2;
                    for (int i = (int)lerp.X - offset; i < (int)lerp.X + offset; i++)
                        for (int j = (int)lerp.Y - offset; j < (int)lerp.Y + offset; j++)
                        {
                            Main.tile[i, j].type = tileID;
                            Main.tile[i, j].active(true);
                            //  WorldGen.PlaceTile(i, j, tileID, true, true);
                        }
                    for (int i = (int)lerp.X - offset + 2; i < (int)lerp.X + offset - 1; i++)
                        for (int j = (int)lerp.Y - offset + 2; j < (int)lerp.Y + offset - 1; j++)
                            Main.tile[i, j].wall = wallID;
                    radius += 0.5f;
                }
            }
            radius = 0f;
            foreach (Vector2 lerp in list)
            {
                while (radius < size)
                {
                    int offset = size / 2;
                    for (int i = (int)lerp.X - offset; i < (int)lerp.X + offset; i++)
                        for (int j = (int)lerp.Y - offset; j < (int)lerp.Y + offset; j++)
                        {
                            Main.tile[i, j].type = 0;
                            Main.tile[i, j].active(false);
                            //  WorldGen.KillTile(i, j, false, false, true);
                        }
                    radius += 0.5f;
                }
                radius = 0f;
            }
            list.Clear();
        }
        public void Relocate(ref Vector2 position, out Vector2[] path)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int count = 0;
            int max = this.max;
            Vector2[] paths = new Vector2[max];
            while (count < max)
            {
                x = (int)position.X;
                y = (int)position.Y;
                int[] direction = new int[]
                {
                    -2, 5, -8, 10, 3, -1,
                    3, 1, -6, -3, 9, 2
                };
                int randX = direction[WorldGen.genRand.Next(direction.Length)];
                int randY = direction[WorldGen.genRand.Next(direction.Length)];
                position.X += randX;
                position.Y += randY;
                paths[count] = position;
                count++;
            }
            path = paths;
        }
    }

    public class Miner : ModWorld
    {
        public Mod moda = ModLoader.GetMod("ArchaeaMod");
        public static string progressText = "";
        static int numMiners = 0, randomX, randomY, bottomBounds = Main.maxTilesY, rightBounds = Main.maxTilesX, circumference, ticks;
        public int edge = 128;
        float mineBlockX = 256, mineBlockY = 256;
        float RightBounds;
        static bool runner = false, grassRunner = false, fillerRunner = false, russianRoulette = false;
        public Vector2 center = new Vector2((Main.maxTilesX / 2) * 16, (Main.maxTilesY / 2) * 16);
        public int buffer = 1, offset = 200;
        int whoAmI = 0, type = 0;
        int XOffset = 512, YOffset = 384;
        public int jobCount = 0;
        public int jobCountMax = 32;
        static int moveID, lookFurther, size = 1;
        public Vector2 minerPos;
        public Vector2 finalVector;
        static Vector2 oldMinerPos, deadZone = Vector2.Zero;
        Vector2 position;
        Vector2 mineBlock;
        public Vector2 baseCenter;
        bool init = false;
        bool fail;
        bool switchMode = false;
        public bool active = true;
        public Vector2[] genPos = new Vector2[2];
        Vector2[] minePath = new Vector2[800 * 800];
        //  for loop takes care of need to generate new miners
        //  Miner[] ID = new Miner[400];
        public void Init()
        {
            if (whoAmI == 0)
            {
                //  remove these comments for public version
                float offset = XOffset * WorldGen.genRand.Next(-1, 1);
                if (offset == 0)
                {
                    offset = XOffset;
                }
                minerPos = center + new Vector2(offset * 16f, Main.maxTilesY - YOffset);
                center = minerPos;
                baseCenter = minerPos;
            }
            else
            {
                int RandomX = WorldGen.genRand.Next(-2, 2);
                int RandomY = WorldGen.genRand.Next(-2, 2);
                if (RandomX != 0 && RandomY != 0)
                {
                    mineBlock = new Vector2(mineBlockX * RandomX, mineBlockY * RandomY);
                    minerPos += mineBlock;
                }
                else
                {
                    mineBlock = new Vector2(mineBlockX, mineBlockY);
                    minerPos += mineBlock;
                    return;
                }
            }
            minePath[0] = center;
            init = true;
            //  Main.spawnTileX = (int)center.X / 16;
            //  Main.spawnTileY = (int)center.Y / 16;
            progressText = jobCount + " initiated, " + Math.Round((double)((float)jobCount / jobCountMax) * 10, 0) + "%";
        }
        public void Update()
        {
            if (!init) Init();
            if (init && whoAmI == 0)
            {
                for (int k = 0; whoAmI < 800; k++)
                {
                    Mine();
                }
            }
            else if (whoAmI > 0 && whoAmI <= 800)
            {
                for (int k = 0; whoAmI < 800; k++)
                {
                    Mine();
                }
                if (whoAmI == 800)
                {
                    jobCount++;
                    Init();
                    whoAmI = 1;
                }
            }

            if (minerPos.X < center.X)
                center.X = minerPos.X;
            if (minerPos.Y < center.Y)
                center.Y = minerPos.Y;
            if (minerPos.X > oldMinerPos.X)
                oldMinerPos.X = minerPos.X;
            if (minerPos.Y > oldMinerPos.Y)
                oldMinerPos.Y = minerPos.Y;

            if (jobCount > jobCountMax)
            {
                progressText = "Process complete";
                int layer = (int)Main.worldSurface;
                int offset = Main.maxTilesY / 2;
                if (minerPos.X < center.X)
                {
                    genPos[0] = new Vector2(minerPos.X, center.Y);
                    genPos[1] = oldMinerPos;
                }
                if (minerPos.X > center.X)
                {
                    genPos[0] = center;
                    genPos[1] = oldMinerPos;
                }
                if (!switchMode)
                {
                    switchMode = true;
                    Dig();
                }
            }
            if (switchMode)
            {
                //  jobCount--;
                Terminate();
                //  Reset();
            }
        }
        public void AverageMove() // most average path, sometimes most interesting
        {
            size = WorldGen.genRand.Next(1, 3);
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.X += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.Y -= 16;
                Dig();
            }
            GenerateNewMiner();
        }
        public void DirectionalMove() // tends to stick to a path
        {
            size = WorldGen.genRand.Next(1, 3);
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                minerPos.X += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                minerPos.X -= 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active())
            {
                minerPos.Y += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (!Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active() &&
                !Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active() &&
                !Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active() &&
                !Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                lookFurther++;
                if (lookFurther % 2 == 0) progressText = "Looking " + lookFurther + " tiles further";
                PlaceWater();
            }
            else lookFurther = 0;
            GenerateNewMiner();
        }
        public void ToTheSurfaceMove() // it likes randomizer = 3
        {
            moveID = 0;
            if (Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                moveID++;
            }
            int randomizer = WorldGen.genRand.Next(0, moveID);
            size = WorldGen.genRand.Next(1, 3);
            if (randomizer == 0)
            {
                lookFurther++;
                int adjust = WorldGen.genRand.Next(1, 4);
                if (adjust == 1)
                {
                    minerPos.X -= 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 2)
                {
                    minerPos.X += 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 3)
                {
                    minerPos.Y -= 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 4)
                {
                    minerPos.Y += 16;
                    PlaceWater();
                    Dig();
                }
                return;
            }
            if (randomizer == 1)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (randomizer == 2)
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (randomizer == 3)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (randomizer == 4)
            {
                minerPos.X += 16;
                Dig();
            }
            GenerateNewMiner();
            lookFurther = 0;
        }
        public void StiltedMove()    // stilted, might work if more iterations of movement, sometimes longest tunnel
        {                                   // best water placer, there's another move that could be extracted from this if the ID segments were removed
            moveID = 0;
            if (Main.tileSolid[Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].type])
            {
                moveID++;
            }
            int randomizer = WorldGen.genRand.Next(0, moveID);
            size = WorldGen.genRand.Next(1, 3);
            if (randomizer == 0)
            {
                lookFurther++;
                int adjust = WorldGen.genRand.Next(1, 4);
                if (adjust == 1)
                {
                    minerPos.X -= 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 2)
                {
                    minerPos.X += 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 3)
                {
                    minerPos.Y -= 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 4)
                {
                    minerPos.Y += 16 * 2;
                    PlaceWater();
                }
                return;
            }
            if (randomizer == 1 && WorldGen.genRand.Next(6) == 2)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (randomizer == 2 && WorldGen.genRand.Next(10) == 4)
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (randomizer == 3)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (randomizer == 4 && WorldGen.genRand.Next(5) == 4)
            {
                minerPos.X += 16;
                Dig();
            }
            GenerateNewMiner();
            lookFurther = 0;
        }
        public void GenerateNewMiner()
        {
            int randomizer = WorldGen.genRand.Next(0, 100);
            if (randomizer < 20 && whoAmI < 800)
            {
                //  Codable.RunGlobalMethod("ModWorld", "miner.Init", new object[] { 0 });
                //  progressText = "Miner " + whoAmI + " created";
                whoAmI++;

                //  unecessary, jobCount takes care of new mining tasks
                //  miner.whoAmI = whoAmI;
                /*  int newMiner = NewMiner(minerPos.X, minerPos.Y, 0, whoAmI);
                    ID[newMiner].init = false;
                    ID[newMiner].Dig(); */
                //  miner.ID[newID].minerPos = Miner.minerPos;
            }
        }
        public void Dig()
        {
            if (type < 800 * 800)
            {
                type++;
                minePath[type] = minerPos;
            }

            if (!switchMode)
            {
                for (int k = 2; k < 24; k++)
                {
                    int i = (int)minerPos.X / 16;
                    int j = (int)minerPos.Y / 16;
                    Tile[] tiles = new Tile[]
                    {
                        Main.tile[i + k, j + k],
                        Main.tile[i - k, j - k],
                        Main.tile[i + k, j - k],
                        Main.tile[i - k, j + k]
                    };
                    foreach (Tile tile in tiles)
                    {
                        tile.type = ArchaeaWorld.magnoStone;
                        tile.active(true);
                    }
                    WorldGen.KillWall((int)minerPos.X / 16 + k, (int)minerPos.Y / 16 + k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 - k, (int)minerPos.Y / 16 - k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 + k, (int)minerPos.Y / 16 - k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 - k, (int)minerPos.Y / 16 + k, false);
                }
            }
            if (switchMode)
            {
                for (int k = 0; k < type; k++)
                {
                    minerPos = minePath[k];
                    if (WorldGen.genRand.Next(60) == 0) PlaceWater();
                    if (size == 1)
                    {
                        int i = (int)minerPos.X / 16;
                        int j = (int)minerPos.Y / 16;
                        Tile[] tiles = new Tile[]
                        {
                            Main.tile[i + circumference, j + circumference],
                            Main.tile[i + circumference, j],
                            Main.tile[i, j + circumference],
                            Main.tile[i, j],
                            Main.tile[i + 1, j],
                            Main.tile[i - 1, j],
                            Main.tile[i, j + 1],
                            Main.tile[i, j - 1]
                        };
                        foreach (Tile tile in tiles)
                        {
                            tile.type = 0;
                            tile.active(false);
                        }
                    }
                    else if (size == 2)
                    {
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 2].active(false);
                    }
                    else if (size == 3)
                    {
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 3, (int)(minerPos.Y / 16) + circumference * 3].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 3].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 3].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 3].active(false);
                    }
                    if (WorldGen.genRand.NextFloat() > 0.5f)
                        Main.tile[(int)minerPos.X / 16, (int)minerPos.Y / 16].wall = ArchaeaWorld.magnoCaveWall;
                }
            }
        }
        private void CaveWalls(int i, int j)
        {
            if (WorldGen.genRand.NextFloat() > 0.50f)
            {
                int radius = WorldGen.genRand.Next(8, 24);
                for (int n = 0; n < radius; n++)
                {
                    for (float r = 0f; r < Math.PI * 2; r += new Draw().radians(n))
                    {
                        float cos = i + (float)(n * (Math.Cos(r)));
                        float sine = j + (float)(n * (Math.Sin(r)));
                        Main.tile[(int)cos, (int)sine].wall = ArchaeaWorld.magnoCaveWall;
                    }
                }
            }
        }
        public void PlaceWater()
        {
            int randomizer = WorldGen.genRand.Next(0, 100);
            if (randomizer < 8)
            { // old randomizer%12 == 0
                Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].liquid = 255;
                Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].liquid = 255;
                WorldGen.SquareTileFrame((int)(minerPos.X / 16), (int)(minerPos.Y / 16));
            }
            if (circumference != 1) return;
        }
        public void Mine()
        {
            //	AverageMove();
            DirectionalMove();
            //	ToTheSurfaceMove();
            //	StiltedMove();
            //  used only in the removal of newly generated miners
            /*          if(russianRoulette){
                            int life = -5;
                            int death = 60;
                            int roulette = WorldGen.genRand.Next(life, death);
                            if(roulette == death && whoAmI > 0){
                                ID[whoAmI] = null;
                                whoAmI++;
                            }
                        }   */
            if (!switchMode && minerPos != Vector2.Zero && jobCount < jobCountMax/* && (minerPos.X < edge * 16 && minerPos.X > (rightBounds - edge) * 16 && minerPos.Y < edge * 16 && minerPos.Y > (bottomBounds - edge) * 16)*/)
            {
                int RandomX = WorldGen.genRand.Next(-2, 2);
                int RandomY = WorldGen.genRand.Next(-2, 2);
                if (RandomX != 0 && RandomY != 0)
                {
                    if (minerPos.Y / 16 > Main.maxTilesY / 3 && minerPos.Y < bottomBounds - edge)
                    {
                        mineBlock = new Vector2(mineBlockX * RandomX, mineBlockY * RandomY);
                        minerPos += mineBlock;
                    }
                    if (minerPos.Y / 16 < Main.maxTilesY / 3)
                    {
                        minerPos.Y += mineBlockY;
                    }
                    if (minerPos.Y / 16 > bottomBounds - edge)
                    {
                        minerPos.Y -= mineBlockY;
                    }
                }
                else return;
            }
        }
        public void Randomizer()
        {
            randomX = WorldGen.genRand.Next(edge, rightBounds - edge);
            randomY = WorldGen.genRand.Next((int)Main.rockLayer, bottomBounds - edge);
            for (int j = -1; j < 1; j++)
            {
                circumference = j;
            }
        }
        public void Terminate()
        {
            jobCount = jobCountMax;
            whoAmI = 800;
            ArchaeaWorld.miner.active = false;
        }
        public void Reset()
        {
            progressText = "";
            type = 0;
            whoAmI = 0;
            jobCount = 0;
            switchMode = false;
            init = false;
            center = new Vector2((Main.maxTilesX / 2) * 16, (Main.maxTilesY / 2) * 16);
            minerPos = center;
            oldMinerPos = default(Vector2);
            genPos[0] = default(Vector2);
            genPos[1] = default(Vector2);
            for (int i = 0; i < minePath.Length - 1; i++)
            {
                minePath[i] = default(Vector2);
            }
            if (Main.maxTilesX == 4200)
                jobCountMax = 32;
            if (Main.maxTilesX == 6400)
                jobCountMax = 48;
            if (Main.maxTilesX == 8400)
                jobCountMax = 64;
        }
    }
}
