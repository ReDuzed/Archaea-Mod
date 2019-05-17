using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs.Bosses
{
    public class Magnoliac_head : Digger
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magno");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 32;
            npc.height = 32;
            npc.lifeMax = 5000;
            npc.defense = 10;
            npc.knockBackResist = 0.5f;
            npc.damage = 20;
            npc.value = 45000;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.npcSlots = maxParts;
            bodyType = mod.NPCType<Magnoliac_body>();
            tailType = mod.NPCType<Magnoliac_tail>();
        }
        public override int maxParts
        {
            get { return 45; }
        }
        private int cycle
        {
            get { return (int)npc.ai[1]; }
            set { npc.ai[1] = value; }
        }
        private const int spawnMinions = 30;
        public override void AI()
        {
            if (timer % 60 == 0 && timer != 0)
                cycle++;
            WormAI();
        }
        public override bool PreAttack()
        {
            bool patch = true;
            if (cycle == spawnMinions && !patch)
            {
                for (int i = 0; i < Math.Min((npc.life * -1 + npc.lifeMax) * 0.0001d, 5); i++)
                {
                    int n = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, mod.NPCType<Merged.NPCs.Copycat_head>());
                    Main.npc[n].whoAmI = n;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
                Main.PlaySound(SoundID.Roar, npc.Center);
                cycle = 0;
            }
            return true;
        }

        public override void Digging()
        {
            if (projs == null || projCenter == null || attack == null)
                return;
            attack.Update(npc, target());
            for (int j = 0; j < projs[1].Length; j++)
            {
                for (int i = 0; i < max; i++)
                {
                    projs[i][j].Stationary(j, npc.width);
                    if (timer % maxTime / 2 == 0 && timer != 0)
                    {
                        Vector2 v = ArchaeaNPC.FindEmptyRegion(target(), ArchaeaNPC.defaultBounds(target()));
                        if (v != Vector2.Zero)
                            projs[i][j].position = v;
                    }
                }
            }
        }
        private bool start = true;
        private int index;
        private int max = 5;
        private float rotate;
        private Vector2[] projCenter;
        private Attack[][] projs;
        private Attack attack;
        public override void Attacking()
        {
            if (timer % maxTime == 0 && timer != 0)
            {
                if (projs != null)
                {
                    for (int j = 0; j < projs.GetLength(0); j++)
                        foreach (Attack sets in projs[j])
                            sets.proj.active = false;
                }
                attack = new Attack(Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ProjectileID.Fireball, 20, 4f));
                attack.proj.tileCollide = false;
                attack.proj.ignoreWater = true;
                max = Math.Max(8 / npc.life, 3);
                projCenter = new Vector2[max];
                projs = new Attack[max][];
                for (int i = 0; i < projs.GetLength(0); i++)
                {
                    projs[i] = new Attack[6];
                    index = 0;
                    for (double r = 0d; r < Math.PI * 2d; r += Math.PI / 3d)
                    {
                        if (index < 6)
                        {
                            projs[i][index] = new Attack(Projectile.NewProjectileDirect(ArchaeaNPC.AngleBased(npc.Center, (float)r, npc.width * 4f), Vector2.Zero, ProjectileID.Fireball, 20, 4f), (float)r);
                            projs[i][index].proj.timeLeft = maxTime;
                            projs[i][index].proj.rotation = (float)r;
                            projs[i][index].proj.tileCollide = false;
                            projs[i][index].proj.ignoreWater = true;
                            Vector2 v = Vector2.Zero;
                            do
                            {
                                v = ArchaeaNPC.FindEmptyRegion(target(), ArchaeaNPC.defaultBounds(target()));
                                projs[i][index].position = v;
                            } while (v == Vector2.Zero);
                            index++;
                        }
                    }
                }
                start = false;
                index = 0;
            }
        }
        public override void BossHeadSlot(ref int index)
        {
            index = NPCHeadLoader.GetBossHeadSlot(ArchaeaMain.magnoHead);
        }
        public override void NPCLoot()
        {
            mod.GetModWorld<ArchaeaWorld>().downedMagno = true;
        }
    }

    public class Attack
    {
        public static float variance;
        public float rotation;
        public Projectile proj;
        private Vector2 focus;
        public Vector2 position;
        public Attack(Projectile proj)
        {
            this.proj = proj;
            position = proj.position;
        }
        public Attack(Projectile proj, float rotation)
        {
            this.proj = proj;
            this.rotation = rotation + (variance += 0.2f);
            position = proj.position;
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
        }
        public void Stationary(int j, int radius)
        {
            rotation += 0.017f;
            proj.timeLeft = 100;
            proj.Center = ArchaeaNPC.AngleBased(position, (float)Math.PI / 3f * j, radius * 4f * (float)Math.Cos(rotation));
        }
        public void Update(NPC npc, Player target)
        {
            proj.timeLeft = 100;
            if (npc.Distance(target.Center) < 800)
                focus = target.Center;
            else focus = npc.Center;
            ArchaeaNPC.RotateIncrement(proj.Center.X > focus.X, ref proj.rotation, ArchaeaNPC.AngleTo(focus, proj.Center), 0.5f, out proj.rotation);
            proj.velocity += ArchaeaNPC.AngleToSpeed(npc.rotation, 0.4f);
            VelClamp(ref proj.velocity, -5f, 5f, out proj.velocity);
            if (proj.velocity.X < 0f && proj.oldVelocity.X >= 0f || proj.velocity.X > 0f && proj.oldVelocity.X <= 0f || proj.velocity.Y < 0f && proj.oldVelocity.Y >= 0f || proj.velocity.Y > 0f && proj.oldVelocity.Y <= 0f)
                proj.netUpdate = true;
        }
        private void VelClamp(ref Vector2 input, float min, float max, out Vector2 result)
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
