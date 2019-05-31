using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs.Legacy
{
    public class Sky_boss_legacy : Legacy.Sky_air_legacy
    {
        public override bool Autoload(ref string name)
        {
            return true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky_boss_legacy");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 5000;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = 45000;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
        }

        private int index;
        private int max = 6;
        private Projectile[] projs = new Projectile[6];
        public override void AI()
        {
            ProjectileDirection();
            if (pattern != (Pattern.Active | Pattern.Idle) && npc.position != npc.oldPosition || npc.velocity.X < 0f && npc.oldVelocity.X >= 0f || npc.velocity.X > 0f && npc.oldVelocity.X <= 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y >= 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y <= 0f)
                SyncNPC();
        }
        public override bool PreFadeOut()
        {
            index = 0;
            for (double r = 0d; r < Math.PI * 2d; r += Math.PI / 3d)
            {
                if (index < max)
                {
                    projs[index] = Projectile.NewProjectileDirect(npc.Center, ArchaeaNPC.AngleToSpeed((float)r, 2f), ProjectileID.Fireball, 20, 4f);
                    projs[index].timeLeft = 900;
                    projs[index].rotation = (float)r;
                    projs[index].tileCollide = false;
                    index++;
                }
            }
            return true;
        }
        private int collect;
        private int total = 8;
        private float radius = 100f;
        private float range;
        private float oldRange;
        private Energy[] energy;
        public static Player[] targets;
        public override bool JustSpawned()
        {
            index = 0;
            energy = new Energy[total];
            for (double r = 0; r < Math.PI * 2d; r += Math.PI / (total / 2d))
                if (index < total)
                    energy[index++] = new Energy(npc, radius, (float)r);
            Target.npc = npc;
            range = 300f;
            oldRange = range;
            targets = UpdateTargets();
            Target.type = 0;
            pattern = Pattern.Idle;
            return false;
        }
        public override bool BeginTeleport()
        {
            if (Hurt())
            {
                range -= 25;
                Target.range = range;
            }
            foreach (Energy e in energy)
            {
                if (e.Absorb(range, new Action(Target.BeingAttacked)))
                {
                    range = oldRange;
                    Target.range = range;
                    targets = UpdateTargets();
                    Target.type = Main.rand.Next(3);
                    return true;
                }
            }
            return false;
        }

        private void ProjectileDirection()
        {
            foreach (Projectile proj in projs)
            {
                if (proj != null)
                {
                    ArchaeaNPC.RotateIncrement(target().Center.X > npc.Center.X, ref proj.rotation, ArchaeaNPC.AngleTo(proj.Center, target().Center), 1f, out proj.rotation);
                    proj.velocity += ArchaeaNPC.AngleToSpeed(proj.rotation, 0.10f);
                    proj.timeLeft = 60;
                    if (proj.Colliding(proj.Hitbox, target().Hitbox))
                        proj.active = false;
                    if (proj.velocity.X < 0f && proj.oldVelocity.X >= 0f || proj.velocity.X > 0f && proj.oldVelocity.X <= 0f || proj.velocity.Y < 0f && proj.oldVelocity.Y >= 0f || proj.velocity.Y > 0f && proj.oldVelocity.Y <= 0f)
                        proj.netUpdate = true;
                }
            }
        }
        protected Player[] UpdateTargets()
        {
            Player[] targets = new Player[Main.player.Length];
            for (int i = 0; i < Main.player.Length; i++)
                if (Main.player[i].active && !Main.player[i].dead)
                    if (Main.player[i].Distance(npc.Center) < 1000f)
                        targets[i] = Main.player[i];
            return targets;
        }
        public override void BossHeadSlot(ref int index)
        {
            index = NPCHeadLoader.GetBossHeadSlot(ArchaeaMain.skyHead);
        }
    }
    public class Energy
    {
        private int time;
        private int elapsed = 30;
        public int total;
        public int max = 6;
        public float radius;
        private float oldRadius;
        public float rotation;
        private float scale;
        private float variance;
        private float rotate;
        private static float r;
        public Vector2 center;
        private Color color;
        private Dust[] dust = new Dust[400];
        public NPC npc;
        public Energy(NPC npc, float radius, float rotation)
        {
            this.npc = npc;
            this.radius = radius;
            oldRadius = radius;
            this.rotation = rotation;
            color = Main.rand.Next(2) == 0 ? Color.Yellow : Color.Blue;
            scale = Main.rand.NextFloat(1.5f, 4f);
            rotate = r += 0.5f;
        }
        public void Reset()
        {
            variance = 0f;
            radius = oldRadius;
            time = 0;
            total = 0;
        }
        public bool Absorb(float range, Action action)
        {
            variance += Main.rand.NextFloat(0.5f, 3f);
            if (time % elapsed * 5 * rotate == 0)
            {
                center = ArchaeaNPC.AngleBased(npc.Center, rotation + variance, range);
                dust[total] = Dust.NewDustDirect(center, 1, 1, DustID.Fire, 0f, 0f, 0, color, scale);
                dust[total].noGravity = true;
                total++;
            }
            foreach (Dust d in dust)
                if (d != null)
                {
                    d.velocity = ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(d.position, npc.Center), 3f);
                    Target.VelClamp(ref d.velocity, -3f, 3f, out d.velocity);
                }
            action.Invoke();
            if (range < npc.width || total > elapsed * 12)
            {
                Reset();
                return true;
            }
            return false;
        }
    }
    public class Target
    {
        private static int time;
        private static int elapsed = 60;
        public static int type;
        private const int
            Melee = 0,
            Range = 1,
            Magic = 2;
        public static float range;
        private static float rotation;
        private static float rotate;
        private static int index;
        public static NPC npc;
        private static Energy[] energy = new Energy[3000];
        public static void BeingAttacked()
        {
            foreach (Player target in Sky_boss_legacy.targets)
            {
                if (time++ % elapsed * 2 == 0 && time != 0)
                {
                    if (target != null)
                    {
                        if (npc.Distance(target.Center) < range)
                        {
                            switch (type)
                            {
                                case Melee:
                                    target.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), 10, 0);
                                    NetMessage.SendData(MessageID.HurtPlayer, -1, -1, null, target.whoAmI);
                                    break;
                                case Range:
                                    target.velocity += ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(target.Center, npc.Center), 0.5f);
                                    VelClamp(ref target.velocity, -2f, 2f, out target.velocity);
                                    break;
                                case Magic:
                                    if (target.statMana > 5)
                                    {
                                        target.statMana -= 5;
                                        target.manaRegenDelay = 180;
                                        NetMessage.SendData(MessageID.PlayerMana, -1, -1, null, target.whoAmI, target.statMana);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static void VelClamp(ref Vector2 input, float min, float max, out Vector2 result)
        {
            if (input.X < min)
                input.X = min;
            if (input.X > max)
                input.X = max;
            if (input.Y < min)
                input.Y = min;
            if (input.Y > max)
                input.Y = max;
            result = input;
        }
    }
}
