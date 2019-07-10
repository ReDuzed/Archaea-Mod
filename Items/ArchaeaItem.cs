using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Buffs;
using ArchaeaMod.Items.Alternate;
using ArchaeaMod.Projectiles;

namespace ArchaeaMod
{
    public class ModItemID
    {
        public static int Deflector
        {
            get { return ArchaeaMain.getMod.ItemType<Deflector>(); }
        }
        public static int Sabre
        {
            get { return ArchaeaMain.getMod.ItemType<Sabre>(); }
        }
    }
}
namespace ArchaeaMod.Items
{
    public class ArchaeaItem
    {
        public static void ActiveChannelStyle(Player player)
        {
            player.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
            float PI = (float)Math.PI;
            float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, Main.MouseWorld);
            player.itemRotation = angle + (player.direction == -1 ? PI : 0);
            player.itemLocation.X = NPCs.ArchaeaNPC.AngleBased(player.position, angle, player.width / 4).X - player.width / 2 - 4;
        }
        public static float StartThrowX(Player player)
        {
            float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, Main.MouseWorld);
            return NPCs.ArchaeaNPC.AngleBased(player.position, angle, player.width / 4).X - player.width / 2 - 4;
        }
        public static bool NotEquipped(Player player, int type)
        {
            int index = 0;
            for (int i = 0; i < player.armor.Length; i++)
            {
                if (player.armor[i].type != type)
                    index++;
                else break;
                if (index == player.armor.Length - 1 && player.armor[i].type != type)
                    return true;
            }
            index = 0;
            return false;
        }
        public static bool Elapsed(int interval)
        {
            return Math.Round(Main.time, 0) % interval == 0;
        }
        public static bool ArmorSet(Player player, string head, string body, string legs)
        {
            return player.armor[0].Name == head &&
                   player.armor[1].Name == body &&
                   player.armor[2].Name == legs;
        }
        public static void Bolt(Player owner, NPC target, ref Vector2 start)
        {
            float max = target.Distance(start);
            for (int k = 0; k < max; k++)
            {
                for (int i = 0; i < 30; i++)
                {
                    if (start.Y > target.position.Y + target.height)
                        return;
                    float angle = Main.rand.NextFloat(0f, (float)Math.PI);
                    start += NPCs.ArchaeaNPC.AngleToSpeed(angle, k);
                    Projectile proj = Projectile.NewProjectileDirect(start, Vector2.Zero, ArchaeaMain.getMod.ProjectileType<Pixel>(), 20, 0f, owner.whoAmI, Pixel.Electric, Pixel.Active);
                    proj.timeLeft = 3;
                }
            }
        }
    }

    public class ArchaeaItem_Global : GlobalItem
    {
        public override void HoldItem(Item item, Player player)
        {
            if (player.releaseUseItem && player.controlUseItem && item.thrown)
            {
                float range = 500f;
                Target[] targets = Target.GetTargets(player, range).Where(t => t != null).ToArray();
                if (targets == null)
                    return;
                if (ArchaeaItem.ArmorSet(player, "Shock Mask", "Shock Plate", "Shock Greaves") || ArchaeaItem.ArmorSet(player, "Ancient Shock Mask", "Ancient Shock Plate", "Ancient Shock Greaves"))
                    foreach (Target target in targets)
                    {
                        if (Target.HitByThrown(player, target))
                        {
                            Vector2 start = target.npc.Center - new Vector2(0f, 500f);
                            ArchaeaItem.Bolt(player, target.npc, ref start);
                        }
                        break;
                    }
            }
        }
    }

    public class Target
    {
        public int time;
        public NPC npc;
        public Player player;
        public const int
            Default = 0,
            ShockWave = 1,
            Frozen = 2,
            Fire = 3;
        private Mod mod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public Target(NPC npc, Player player)
        {
            this.npc = npc;
            this.player = player;
        }
        public void AttackEffect(int type)
        {
            switch (type)
            {
                case Default:
                    break;
                case ShockWave:
                    float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, npc.Center);
                    npc.velocity.Y -= 8f;
                    npc.velocity += NPCs.ArchaeaNPC.AngleToSpeed(angle, 12f);
                    NPCs.ArchaeaNPC.VelocityClamp(npc, -10f, 10f);
                    break;
                case Frozen:
                    npc.AddBuff(mod.BuffType<frozen>(), 60);
                    break;
                case Fire:
                    npc.AddBuff(BuffID.OnFire, 10);
                    break;
            }
        }
        public bool Elapsed(int interval)
        {
            return time++ % interval == 0 && time != 0;
        }
        public static bool HitByThrown(Player player, Target target)
        {
            foreach (Projectile proj in Main.projectile)
                if (proj.owner == player.whoAmI && proj.thrown)
                    if (proj.Hitbox.Distance(target.npc.Center) < proj.width + target.npc.width / 2 + 16f)
                        return true;
            return false;
        }
        public static Target GetClosest(Player owner, Target[] targets)
        {
            List<float> ranges = new List<float>();
            foreach (Target target in targets)
            {
                ranges.Add(target.npc.Distance(owner.Center));
                return targets[ranges.IndexOf(ranges.Min())];
            }
            return null;
        }
        public static Target[] GetTargets(Player player, float range)
        {
            Target[] targets = new Target[255];
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Shield.GetOnCondition(Main.npc[i])) continue;
                if (player.Distance(Main.npc[i].Center) > range) continue;
                targets[i] = new Target(Main.npc[i], player);
            }
            return targets;
        }
        public static Target[] GetTargets(NPC npc, float range)
        {
            Target[] targets = new Target[255];
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Shield.GetOnCondition(Main.npc[i])) continue;
                if (npc.Distance(Main.npc[i].Center) > range) continue;
                targets[i] = new Target(Main.npc[i], null);
            }
            return targets;
        }
        public static Target[] GetTargets(Projectile projectile, float range)
        {
            Target[] targets = new Target[255];
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Shield.GetOnCondition(Main.npc[i])) continue;
                if (projectile.Distance(Main.npc[i].Center) > range) continue;
                targets[i] = new Target(Main.npc[i], null);
            }
            return targets;
        }
    }
}
