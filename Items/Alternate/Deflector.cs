using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Alternate
{
    public class Deflector : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deflector");
            Tooltip.SetDefault("Creates barriers from harmful attacks");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.mana = 10;
            item.damage = 10;
            item.value = 10000;
            item.expert = true;
            item.rare = ItemRarityID.Expert;
            item.accessory = true;
        }

        private int ai = -1;
        public const int
            Start = -1,
            Reset = 0,
            Orbit = 1,
            Throw = 2,
            OnBreak = 3;
        private int index;
        private int x;
        private const int max = 4;
        private Vector2 move;
        private Vector2 start;
        private Shield[] shield = new Shield[4];
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ShieldStyle();
            switch (ai)
            {
                case Start:
                    if (player.CheckMana(10, true))
                    {
                        for (float r = 0; r < Math.PI * 2f; r += (float)Math.PI / (max / 2f))
                        {
                            if (index < max)
                            {
                                shield[index] = new Shield(player, r);
                                index++;
                            }
                        }
                        index = 0;
                        goto case Orbit;
                    }
                    break;
                case Reset:
                    break;
                case Orbit:
                    ai = Orbit;
                    type = Orbit;
                    if (ArchaeaPlayer.KeyPress(Keys.E))
                    {
                        move = Main.MouseWorld;
                        goto case Throw;
                    }
                    break;
                case Throw:
                    ai = Throw;
                    type = Throw;
                    break;
            }
        }
        public override void UpdateEquip(Player player)
        {
            if (ArchaeaItem.NotEquipped(player, item, ref x))
            {
                ai = -1;
                foreach (Shield s in shield)
                    if (s != null)
                        s.Break();
            }
        }

        private int type;
        public void ShieldStyle()
        {
            foreach (Shield s in shield)
            {
                if (s != null)
                {
                    s.Update();
                    s.OrbitPoint();
                    switch (type)
                    {
                        case Orbit:
                            s.thrown = false;
                            break;
                        case Throw:
                            s.thrown = true;
                            s.ShockTarget(400f);
                            s.point += NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(s.point, move), Shield.speed);
                            ai = s.SetType();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public class Shield
    {
        public bool thrown;
        public int life;
        public int maxLife;
        private int timeLeft = 180;
        private int hitsLeft = 3;
        private int damage = 15;
        public float rotation;
        private float rotate;
        private float angle;
        private float radius;
        public static float speed = 10f;
        public Vector2 center;
        public Vector2 point;
        public Projectile proj;
        private Projectile shock;
        public NPC npc;
        public Player target;
        private object t;
        public Player player;
        private NPC[] targets = new NPC[256];
        public Shield(Player player, float rotation)
        {
            this.player = player;
            this.rotation = rotation;
            radius = 64f;
            proj = Projectile.NewProjectileDirect(Vector2.Zero, Vector2.Zero, ProjectileID.Fireball, damage, 8f, player.whoAmI);
            proj.Center = GetPosition(player.Center, radius);
            proj.tileCollide = false;
            proj.ignoreWater = true;
            proj.friendly = true;
            proj.hostile = false;
            proj.timeLeft = 600;
            point = proj.Center;
            t = player;
        }
        public void Update()
        {
            proj.Center = GetPosition(point, radius);
            if (!thrown && Main.mouseLeftRelease && Main.mouseLeft)
            {
                NPC n = TargetNPC();
                if (n != null)
                {
                    npc = n;
                    t = n;
                }
                Player p = TargetPlayer();
                if (p != null)
                {
                    target = p;
                    t = p;
                }
            }
        }
        public void OrbitPoint()
        {
            angle = NPCs.ArchaeaNPC.AngleTo(point, proj.Center) + rotation;
            rotate = 0.017f * 5f;
            if (t != null && !thrown)
            {
                if (t == player)
                    point = player.Center;
                else point += center;
                if (target == t)
                {
                    center = NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(point, target.Center), speed);
                    if (target.Hitbox.Contains(point.ToPoint()))
                        point = target.Center;
                }
                if (npc == t)
                {
                    center = NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(point, npc.Center), speed);
                    if (npc.Hitbox.Contains(point.ToPoint()))
                        point = npc.Center;
                }
            }
            proj.timeLeft = 100;
        }
        public void ShockTarget(float range)
        {
            if (Math.Round(Main.time, 0) % 180 == 0)
            {
                targets = GetTargets(range);
                foreach (NPC n in targets)
                {
                    if (n != null && GetOnCondition(n))
                    {
                        float distance = n.Distance(proj.Center);
                        if (distance < range)
                        {
                            for (int r = 0; r < distance; r++)
                            {
                                float angle = NPCs.ArchaeaNPC.AngleTo(proj.Center, n.Center);
                                Dust dust = Dust.NewDustDirect(proj.Center + NPCs.ArchaeaNPC.AngleToSpeed(angle, r), 1, 1, DustID.Fire);
                            }
                            n.StrikeNPC(damage, 0f, 0);
                            break;
                        }
                    }
                }
            }

        }
        public static void ShockTarget(Vector2 itemHitbox, NPC target, int damage)
        {
            float angle = NPCs.ArchaeaNPC.AngleTo(itemHitbox, target.Center);
            for (int d = 0; d < target.Distance(itemHitbox); d++)
            {
                Dust dust = Dust.NewDustDirect(itemHitbox + NPCs.ArchaeaNPC.AngleToSpeed(angle, d), 1, 1, DustID.Fire);
            }
            target.StrikeNPC(damage, 0f, 0);
        }

        public NPC[] GetTargets(float range)
        {
            NPC[] npcs = new NPC[256];
            foreach (NPC n in Main.npc)
                if (GetOnCondition(n))
                    npcs[n.whoAmI] = n;
            return npcs;
        }
        public NPC TargetNPC()
        {
            foreach (NPC npc in Main.npc)
                if (npc.active && npc.life > 0 && !npc.dontTakeDamage && !npc.immortal)
                    if (npc.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                        return npc;
            return null;
        }
        public Player TargetPlayer()
        {
            foreach (Player player in Main.player)
                if (player.active && !player.dead)
                    if (player.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                       return player;
            return null;
        }
        public int SetType()
        {
            if (timeLeft-- < 0)
            {
                Break();
                timeLeft = 180;
                return Deflector.Start;
            }
            return Deflector.Reset;
        }
        public void Break()
        {
            Dust[] dusts = NPCs.ArchaeaNPC.DustSpread(proj.Center);
            foreach (Dust dust in dusts)
                dust.noGravity = true;
            proj.active = false;
        }

        public Vector2 GetPosition(Vector2 center, float radius)
        {
            return NPCs.ArchaeaNPC.AngleBased(center, angle += rotate, radius);
        }
        public static bool GetOnCondition(NPC npc)
        {
            return !npc.friendly && npc.active && npc.life > 0 && !npc.dontTakeDamage && !npc.immortal;
        }
    }
}
