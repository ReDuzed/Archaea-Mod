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
    public class ModNPCID
    {
        protected static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public static int ItchySlime
        {
            get { return getMod.NPCType<NPCs.Slime_Itchy>(); }
        }
        public static int MercurialSlime
        {
            get { return getMod.NPCType<NPCs.Slime_Mercurial>(); }
        }
        public static int Mimic
        {
            get { return getMod.NPCType<NPCs.Mimic>(); }
        }
        public static int Fanatic
        {
            get { return getMod.NPCType<NPCs.Fanatic>(); }
        }
        public static int Hatchling
        {
            get { return getMod.NPCType<NPCs.Hatchling_head>(); }
        }
        public static int Observer
        {
            get { return getMod.NPCType<NPCs.Sky_1>(); }
        }
        public static int Marauder
        {
            get { return getMod.NPCType<NPCs.Sky_2>(); }
        }
        public static int MagnoliacHead
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_head>(); }
        }
        public static int MagnoliacBody
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_body>(); }
        }
        public static int MagnoliacTail
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_tail>(); }
        }
        public static int SkyBoss
        {
            get { return getMod.NPCType<NPCs.Bosses.Sky_boss>(); }
        }
        public static int Gargoyle
        {
            get { return getMod.NPCType<NPCs.Sky_3>(); }
        }
    }
}

namespace ArchaeaMod.NPCs
{
    public enum Pattern
    {
        JustSpawned,
        Idle,
        Active,
        Attack,
        Teleport,
        FadeIn,
        FadeOut
    }
    public class PatternID
    {
        public const int
            JustSpawned = 0,
            Idle = 1,
            Active = 2,
            Attack = 3,
            Teleport = 4,
            FadeIn = 5,
            FadeOut = 6;
    }
    public class _GlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            bool MagnoBiome = spawnInfo.player.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome;
            bool downedMagno = mod.GetModWorld<ArchaeaWorld>().downedMagno;
            pool.Add(ModNPCID.Fanatic,          MagnoBiome ? 0.2f : 0f);
            pool.Add(ModNPCID.Hatchling,        MagnoBiome ? 0.2f : 0f);
            pool.Add(ModNPCID.ItchySlime,       MagnoBiome ? 0.4f : 0f);
            pool.Add(ModNPCID.MercurialSlime,   MagnoBiome && downedMagno ? 0.4f : 0f);
            pool.Add(ModNPCID.Mimic,            MagnoBiome && Main.hardMode ? 0.1f : 0f);
            bool SkyFort = spawnInfo.player.GetModPlayer<ArchaeaPlayer>(mod).SkyFort;
            pool.Add(ModNPCID.Observer,         SkyFort ? 0.4f : 0f);
            pool.Add(ModNPCID.Marauder,         SkyFort ? 0.4f : 0f);
            pool.Add(ModNPCID.Gargoyle,         SkyFort ? 0.2f : 0f);
        }
        public override bool CheckActive(NPC npc)
        {
            if (npc.TypeName.Contains("Sky"))
                return true;
            return base.CheckActive(npc);
        }
    }
    public class ArchaeaNPC : GlobalNPC
    {
        public static int defaultWidth = 800;
        public static int defaultHeight = 600;
        public static Rectangle defaultBounds(NPC npc)
        {
            return new Rectangle((int)npc.position.X - defaultWidth / 2, (int)npc.position.Y - defaultHeight / 2, defaultWidth, defaultHeight);
        }
        public static Rectangle defaultBounds(Player player)
        {
            return new Rectangle((int)player.position.X - defaultWidth / 2, (int)player.position.Y - defaultHeight / 2, defaultWidth, defaultHeight);
        }
        public static Vector2 FastMove(NPC npc)
        {
            return FindGround(npc, defaultBounds(npc));
        }
        public static Vector2 FastMove(Player player)
        {
            return FindGround(player, defaultBounds(player));
        }
        public static bool TargetBasedMove(NPC npc, Player target, bool playerRange = false)
        {
            int width = Main.screenWidth - 100;
            int height = Main.screenHeight - 100;
            Vector2 old = npc.position;
            Vector2 vector;
            if (target == null)
                return false;
            vector = FindGround(npc, new Rectangle((int)target.position.X - width / 2, (int)target.position.Y - height / 2, width, height));
            if (!ArchaeaWorld.Inbounds((int)vector.X / 16, (int)vector.Y / 16))
                return false;
            if (vector != Vector2.Zero)
                npc.position = vector;
            if (old != npc.position)
                return true;
            return false;
        }
        public static bool NoSolidTileCollision(Tile tile)
        {
            return (tile.active() && !Main.tileSolid[tile.type]) || !tile.active();
        }
        public static Vector2 FindGround(NPC npc, Rectangle bounds)
        {
            var vector = FindEmptyRegion(npc, bounds);
            if (vector != Vector2.Zero)
            {
                int i = (int)vector.X / 16;
                int j = (int)(vector.Y + npc.height - 8) / 16;
                if (!ArchaeaWorld.Inbounds(i, j))
                    return Vector2.Zero;
                int max = npc.width / 16;
                int move = 0;
                Tile ground = Main.tile[i + (npc.width / 16 / 2), j + move];
                while (NoSolidTileCollision(ground))
                {
                    move++;
                    ground = Main.tile[i + (npc.width / 16 / 2), j + move];
                }
                vector.Y += move * 16 - 16;
                return vector;
            }
            else return Vector2.Zero;
        }
        public static Vector2 FindGround(Player player, Rectangle bounds)
        {
            var vector = FindEmptyRegion(player, bounds);
            for (int k = 0; k < 5; k++)
            {
                int i = (int)vector.X / 16;
                int j = (int)(vector.Y + player.height + 8) / 16;
                if (!ArchaeaWorld.Inbounds(i, j))
                    continue;
                int count = 0;
                int max = player.width / 16;
                for (int l = 0; l < player.width / 16; l++)
                {
                    Tile ground = Main.tile[i + l, j + 1];
                    if (ground.active() && Main.tileSolid[ground.type])
                        count++;
                }
                while (vector.Y + player.height < j * 16)
                    vector.Y++;
                if (Collision.SolidCollision(vector, player.width - 4, player.height - 4))
                    return Vector2.Zero;
                if (count == max)
                    return vector;
            }
            return Vector2.Zero;
        }
        public static Vector2 FindEmptyRegion(NPC npc, Rectangle check)
        {
            int tile = 16;
            int x = Main.rand.Next(check.X, check.Right);
            int y = Main.rand.Next(check.Y, check.Bottom);
            if (Main.netMode == 0)
            {
                x /= tile;
                y /= tile;
                for (int n = npc.height / 16; n >= 0; n--)
                    for (int m = 0; m < npc.width / 16; m++)
                        if (!NoSolidTileCollision(Main.tile[x + m, y + n]))
                            return Vector2.Zero;
                return new Vector2(x * tile, y * tile);
            }
            else
            {
                if (Collision.SolidTiles(x, x + npc.width, y, y + npc.height))
                    return Vector2.Zero;
            }
            return new Vector2(x, y);
        }
        public static Vector2 FindEmptyRegion(Player player, Rectangle check)
        {
            int x = Main.rand.Next(check.X, check.Right);
            int y = Main.rand.Next(check.Y, check.Bottom);
            int tile = 18;
            for (int n = player.height + tile; n >= 0; n--)
                for (int m = 0; m < player.width + tile; m++)
                {
                    int i = (x + m) / 16;
                    int j = (y + n) / 16;
                    if (Collision.SolidCollision(new Vector2(x + m, y + n), player.width, player.height))
                        return Vector2.Zero;
                    return new Vector2(x, y);
                }
            return Vector2.Zero;
        }
        public static Vector2 FindAny(NPC npc, Player target, bool findGround = true, int range = 400)
        {
            int x = 0, y = 0;
            x = Main.rand.Next((int)target.Center.X - range, (int)target.Center.X + range);
            y = Main.rand.Next((int)target.Center.Y - (int)(range * 0.67f), (int)target.Center.Y + (int)(range * 0.67f));
            x = (x - (x % 16)) / 16;
            y = (y - (y % 16)) / 16;
            if (!ArchaeaWorld.Inbounds(x, y))
                return Vector2.Zero;
            if (findGround)
            {
                if (!Main.tile[x, y + npc.height / 16 + 1].active() || !Main.tileSolid[Main.tile[x, y + npc.height / 16 + 1].type] || !Main.tileSolid[Main.tile[x + 1, y + npc.height / 16 + 1].type] || Main.tile[x, y + (npc.height - 4) / 16].active())
                    return Vector2.Zero;
            }
            return new Vector2(x * 16, y * 16);
        }
        public static Vector2 FindAny(NPC npc, int range = 400)
        {
            int tries = 0;
            int x = 0, y = 0;
            x = Main.rand.Next((int)npc.Center.X - range, (int)npc.Center.X + range);
            y = Main.rand.Next((int)npc.Center.Y - range, (int)npc.Center.Y + range);
            x = (x - (x % 16));
            y = (y - (y % 16));
            return new Vector2(x, y);
        }
        public static Vector2 AllSolidFloors(Player target)
        {
            int range = 400;
            int x = (int)target.Center.X;
            int y = (int)target.Center.Y;
            int right = x + range;
            int bottom = y + range;
            List<Vector2> floor = new List<Vector2>();
            for(int i = x - range; i < right; i++)
            {
                for (int j = y - range; j < bottom; j++)
                {
                    int tile = 16;
                    if (!Collision.SolidTiles(i, i + tile, j, j + tile) && Collision.SolidTiles(i, i + tile, j + tile, j + tile * 2))
                        floor.Add(new Vector2(i, j));
                }
            }
            if (floor.Count() > 0)
                return floor[Main.rand.Next(floor.Count())];
            else return Vector2.Zero;
        }
        public static bool WithinRange(Vector2 position, Rectangle range)
        {
            return range.Contains(position.ToPoint());
        }
        protected static Rectangle Range(Vector2 position, int width, int height)
        {
            return new Rectangle((int)position.X - width / 2, (int)position.Y - width / 2, width, height);
        }
        public static Player FindClosest(NPC npc, bool unlimited = false, float range = 300f)
        {
            int[] indices = new int[Main.player.Length];
            if (!unlimited)
            {
                foreach (Player target in Main.player)
                    if (target.active)
                        if (npc.Distance(target.position) < range)
                            return target;
            }
            else
            {
                int count = 0;
                for (int i = 0; i < Main.player.Length; i++)
                    if (Main.player[i].active)
                        indices[count] = Main.player[i].whoAmI;
                float[] distance = new float[indices.Length];
                for (int k = 0; k < indices.Length; k++)
                    distance[k] = Vector2.Distance(Main.player[k].position, npc.position);
                return Main.player[indices[distance.ToList().IndexOf(distance.Min())]];
            }
            return null;
        }

        public static Vector2 AngleToSpeed(float angle, float amount = 2f)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static Vector2 AngleBased(Vector2 position, float angle, float radius)
        {
            float cos = position.X + (float)(radius * Math.Cos(angle));
            float sine = position.Y + (float)(radius * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static float RandAngle()
        {
            return Main.rand.NextFloat((float)(Math.PI * 2d));
        }
        public static float AngleTo(NPC from, Player to)
        {
            return (float)Math.Atan2(to.position.Y - from.position.Y, to.position.X - from.position.X);
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static Dust[] DustSpread(Vector2 v, int width = 1, int height = 1, int dustType = 6, int total = 10, float scale = 1f, Color color = default(Color), bool noGravity = false)
        {
            Dust[] dusts = new Dust[total];
            for (int k = 0; k < total; k++)
            {
                Vector2 speed = ArchaeaNPC.AngleToSpeed(ArchaeaNPC.RandAngle(), 8f);
                dusts[k] = Dust.NewDustDirect(v + speed, width, height, dustType, speed.X, speed.Y, 0, color, scale);
                dusts[k].noGravity = noGravity;
            }
            return dusts;
        }
        public static bool OnHurt(int life, int oldLife, out int newLife)
        {
            if (life < oldLife)
            {
                newLife = life;
                return true;
            }
            newLife = life;
            return false;
        }
        public static void RotateIncrement(bool direction, ref float from, float to, float speed, out float result)
        {
            if (!direction)
            {
                if (from > to * -1)
                    from -= speed;
                if (from < to * -1)
                    from -= speed;
            }
            else
            {
                if (from > to)
                    from -= speed;
                if (from < to)
                    from += speed;
            }
            result = from;
        }
        public static void SlowDown(ref Vector2 velocity)
        {
            velocity.X = velocity.X > 0.1f ? velocity.X -= 0.05f : 0f;
            velocity.X = velocity.X < -0.1f ? velocity.X += 0.05f : 0f;
        }
        public static void SlowDown(ref Vector2 velocity, float rate = 0.05f)
        {
            if (velocity.X > 0.1f)
                velocity.X -= rate;
            if (velocity.X < -0.1f) 
                velocity.X += rate;
            if (velocity.Y > 0.1f)
                velocity.Y -= rate;
            if (velocity.Y < -0.1f)
                velocity.Y += rate;
        }
        public static void PositionToVel(NPC npc, Vector2 change, float speedX, float speedY, bool clamp = false, float min = -2.5f, float max = 2.5f, bool wobble = false, double degree = 0f)
        {
            float cos = wobble ? (float)(0.05f * Math.Cos(degree)) : 0f;
            float sine = wobble ? (float)(0.05f * Math.Sin(degree)) : 0f;
            if (clamp)
                VelocityClamp(npc, min, max);
            if (npc.position.X < change.X)
                npc.velocity.X += speedX + cos;
            if (npc.position.X > change.X)
                npc.velocity.X -= speedX + cos;
            if (npc.position.Y < change.Y)
                npc.velocity.Y += speedY + sine;
            if (npc.position.Y > change.Y)
                npc.velocity.Y -= speedY + sine;
        }
        public static void VelocityClamp(NPC npc, float min, float max)
        {
            Vector2 _min = new Vector2(min, min);
            Vector2 _max = new Vector2(max, max);
            Vector2.Clamp(ref npc.velocity, ref _min, ref _max, out npc.velocity);
        }
        public static void VelocityClamp(Projectile proj, float min, float max)
        {
            Vector2 _min = new Vector2(min, min);
            Vector2 _max = new Vector2(max, max);
            Vector2.Clamp(ref proj.velocity, ref _min, ref _max, out proj.velocity);
        }
        public static void VelocityClamp(ref Vector2 velocity, float min, float max)
        {
            Vector2 _min = new Vector2(min, min);
            Vector2 _max = new Vector2(max, max);
            Vector2.Clamp(ref velocity, ref _min, ref _max, out velocity);
        }
        public static void VelClampX(NPC npc, float min, float max)
        {
            if (npc.velocity.X < min)
                npc.velocity.X = min;
            if (npc.velocity.X > max)
                npc.velocity.X = max;
        }

        protected static bool SolidGround(Tile[] tiles)
        {
            int count = 0;
            foreach (Tile ground in tiles)
                if (!NotActiveOrSolid(ground))
                {
                    count++;
                    if (count == tiles.Length)
                        return true;
                }
            return false;
        }
        protected static bool NotActiveOrSolid(int i, int j)
        {
            return (!Main.tile[i, j].active() && Main.tileSolid[Main.tile[i, j].type]) || (Main.tile[i, j].active() && !Main.tileSolid[Main.tile[i, j].type]);
        }
        protected static bool NotActiveOrSolid(Tile tile)
        {
            return (!tile.active() && Main.tileSolid[tile.type]) || (tile.active() && !Main.tileSolid[tile.type]);
        }

        #region out of view
        /*
        int add = 30;
        int count = 0;
        int max = 3;
        List<Vector2> vectors = new List<Vector2>();
        Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        while (count < max)
        {
            check.Inflate(add, add);
            if (check.Width > screen.Width + 30)
                break;
            count++;
        }
        for (int i = check.Left; i < check.Right; i++)
            for (int j = check.Left; j < check.Right; i++)
                if (!WithinRange(new Vector2(i, j), screen) && ArchaeaWorld.Inbounds(i, j))
                    vectors.Add(new Vector2(i, j));
        if (vectors.Count > 0)
            return vectors.ToArray();
        return new Vector2[] { Vector2.Zero };*/
        #endregion
        #region depracated
        /*if (!ArchaeaWorld.Inbounds(x, y))
        {
            npc.active = false;
            return Vector2.Zero;
        }
        int count = 0;
        int max = npc.width / 16 * (npc.height / 16);
        for (int l = 0; l < npc.height; l++)
            for (int k = 0; k < npc.width; k++)
            {
                int i = (x + k) / 16;
                int j = (y + l - npc.height) / 16;
                if (!ArchaeaWorld.Inbounds(i, j))
                    continue;
                Tile tile = Main.tile[i, j];
                if (NotActiveOrSolid(tile))
                    count++;
                else
                {
                    count = 0;
                    break;
                }
                if (k == 0 && l == npc.height - 1)
                    for (int m = 0; m < npc.width / 16; m++)
                    {
                        Tile ground = Main.tile[i + m, j + 1];
                        if (ground.active() && Main.tileSolid[ground.type])
                            count++;
                    }
                if (count == max + npc.width / 16)
                    return new Vector2(x, y);
            }
        return new Vector2(rangeOut, rangeOut);*/
        #endregion
        #region Depracated SpawnOnGround
        /*
        Tile[] ground = new Tile[npc.width / 16];
        Vector2 spawn = FindNewPosition(npc, bounds);
        int i = (int)(spawn.X / 16);
        int j = (int)(spawn.Y + npc.height) / 16;
        int count = 0;
        for (int k = 0; k < ground.Length; k++)
        {
            ground[k] = Main.tile[i + k, j + 1];
            if (ground[k].active() && Main.tileSolid[ground[k].type])
            {
                count++;
                if (count == ground.Length)
                    return spawn;
            }
        }
        return Vector2.Zero;*/
        #endregion
    }
}
