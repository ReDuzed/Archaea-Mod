using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs.Bosses
{
    public class Sky_boss : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necrosis");
        }
        public override void SetDefaults()
        {
            npc.width = 176;
            npc.height = 192;
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

        public bool Hurt()
        {
            bool hurt = npc.life < npc.lifeMax && npc.life > 0 && oldLife != npc.life;
            oldLife = npc.life;
            return hurt;
        }
        private int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        private int npcCounter
        {
            get { return (int)npc.ai[1]; }
            set { npc.ai[1] = value; }
        }
        private int counter
        {
            get { return (int)npc.ai[2]; }
            set { npc.ai[2] = value; }
        }
        private bool fade;
        private bool firstTarget = true;
        private int oldLife;
        private Vector2 move;
        private Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, true);
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                return player;
            }
            return Main.player[npc.target];
        }
        private bool init;
        private bool attack;
        private int index;
        private int sendIndex;
        private float angle;
        private Projectile[] orbs = new Projectile[7];
        private Projectile[] flames = new Projectile[5];
        public override bool PreAI()
        {
            if (!init)
                init = true;
            return init;
        }
        public override void AI()
        {
            if (timer++ > 900)
            {
                npcCounter++;
                timer = 0;
            }
            if (timer % 600 == 0 && timer != 0)
            {
                move = Vector2.Zero;
                do
                {
                    move = ArchaeaNPC.FindEmptyRegion(target(), ArchaeaNPC.defaultBounds(target()));
                } while (move == Vector2.Zero);
                SyncNPC(move.X, move.Y);
                fade = true;
            }
            if (timer % 300 == 0 && timer != 0)
            {
                if (timer != 600)
                {
                    move = target().Center;
                    SyncNPC(true, true);
                }
            }
            if (npcCounter > 1)
            {
                Vector2 newPosition = ArchaeaNPC.FindAny(npc, target(), false, 300);
                int n = NPC.NewNPC((int)newPosition.X, (int)newPosition.Y, mod.NPCType<Sky_1>(), 0, 0f, 0f, 0f, 0f, npc.target);
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                npcCounter = 0;
                SyncNPC();
            }
            if (fade)
            {
                npc.velocity = Vector2.Zero;
                if (npc.alpha < 255)
                {
                    npc.scale -= 1f / 90f;
                    npc.alpha += 255 / 15;
                }
                else 
                {
                    npc.position = move;
                    if (FlameBurst())
                    {
                        npc.scale = 1f;
                        fade = false;
                    }
                }
            }
            else
            {
                if (npc.alpha > 0)
                    npc.alpha -= 255 / 60;
                if (timer < 600 && !attack)
                {
                    if (timer % 150 == 0)
                        move = ArchaeaNPC.FindAny(npc, target(), false);
                    float angle = npc.AngleTo(move);
                    float cos = (float)(0.2f * Math.Cos(angle));
                    float sine = (float)(0.2f * Math.Sin(angle));
                    npc.velocity += new Vector2(cos, sine);
                    ArchaeaNPC.VelocityClamp(ref npc.velocity, -4f, 4f);
                }
            }
            if (attack)
            {
                if (counter++ % 90 == 0)
                {
                    angle += (float)Math.PI / 3f;
                    float cos = (float)(npc.Center.X + npc.width * 3f * Math.Cos(angle));
                    float sine = (float)(npc.Center.Y + npc.height * 3f * Math.Sin(angle));
                    int t = Projectile.NewProjectile(new Vector2(cos, sine), Vector2.Zero, mod.ProjectileType<Orb>(), 12, 2f, 255, 0f, target().whoAmI);
                    Main.projectile[t].whoAmI = t;
                    if (Main.netMode == 2)
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, t);
                    index++;
                }
                if (index == 6)
                    attack = false;

            }
            else
            {
                index = 0;
                angle = ArchaeaNPC.RandAngle();
                SyncNPC(false, false);
            }
            if (fade && npc.position != npc.oldPosition || npc.velocity.X < 0f && npc.oldVelocity.X >= 0f || npc.velocity.X > 0f && npc.oldVelocity.X <= 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y >= 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y <= 0f)
                SyncNPC();
        }
        public override void BossHeadSlot(ref int index)
        {
            index = NPCHeadLoader.GetBossHeadSlot(ArchaeaMain.skyHead);
        }

        private bool FlameBurst()
        {
            float angle = ArchaeaNPC.RandAngle();
            for (int i = 0; i < flames.Length; i++)
            {
                flames[i] = Projectile.NewProjectileDirect(npc.Center, ArchaeaNPC.AngleToSpeed(angle), mod.ProjectileType<Flame>(), 20, 3f, 255, 1f, npc.target);
                angle += (float)Math.PI / 3f;
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, flames[i].whoAmI);
            }
            return true;
        }

        private void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private void SyncNPC(float x, float y)
        {
            move = new Vector2(x, y);
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private void SyncNPC(bool attack, bool immortal)
        {
            this.attack = attack;
            npc.immortal = immortal;
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(move);
            writer.Write(attack);
            writer.Write(npc.immortal);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            move = reader.ReadVector2();
            attack = reader.ReadBoolean();
            npc.immortal = reader.ReadBoolean();
        }
    }

    internal class Orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 10;
            projectile.knockBack = 2f;
            projectile.timeLeft = 360;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
        }
        private int timer
        {
            get { return (int)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        private Player target
        {
            get { return Main.player[(int)projectile.ai[1]]; }
        }
        private NPC boss
        {
            get { return Main.npc[(int)projectile.localAI[0]]; }
        }
        private bool init;
        public override void AI()
        {
            if (projectile.alpha > 0)
                projectile.alpha -= 255 / 60;
            else projectile.alpha = 0;
            float maxSpeed = Math.Max(((boss.lifeMax + 1 - boss.life) / boss.lifeMax) * 5f, 2f);
            float angle;
            if (timer++ > 90)
            {
                if (target.active && !target.dead)
                    angle = projectile.AngleTo(target.Center);
                else angle = projectile.AngleFrom(target.Center);
                projectile.velocity += ArchaeaNPC.AngleToSpeed(angle, 0.5f);
                ArchaeaNPC.VelocityClamp(ref projectile.velocity, maxSpeed * -1, maxSpeed);
            }
            projectile.rotation = projectile.velocity.ToRotation();
            if (Main.netMode == 2 && (projectile.velocity.X < 0f && projectile.oldVelocity.X >= 0f || projectile.velocity.X > 0f && projectile.oldVelocity.X <= 0f || projectile.velocity.Y < 0f && projectile.oldVelocity.Y >= 0f || projectile.velocity.Y > 0f && projectile.oldVelocity.Y <= 0f))
                projectile.netUpdate = true;
            if (projectile.scale == 1f)
            {
                for (int k = 0; k < 4; k++)
                {
                    int t = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[t].noGravity = true;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            ArchaeaNPC.DustSpread(projectile.position, projectile.width, projectile.height, 6, 6, 2f);
        }
    }


    internal class Flame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 20;
            projectile.knockBack = 2f;
            projectile.timeLeft = 450;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
        }
        private int i;
        private int j;
        private Player player
        {
            get { return Main.player[(int)projectile.ai[1]]; }
        }
        public override void AI()
        {
            if (projectile.Distance(player.Center) > 2048)
                projectile.active = false;
            if (projectile.ai[0] != 1f)
                projectile.timeLeft = 90;
            i = (int)projectile.position.X / 16;
            j = (int)projectile.position.Y / 16;
            projectile.rotation = projectile.velocity.ToRotation();
            if (TileLeft() || TileRight())
                projectile.velocity.X *= -1;
            if (TileTop() || TileBottom())
                projectile.velocity.Y *= -1;
            for (int k = 0; k < 3; k++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame);
            if (Main.netMode == 2 && (projectile.velocity.X < 0f && projectile.oldVelocity.X >= 0f || projectile.velocity.X > 0f && projectile.oldVelocity.X <= 0f || projectile.velocity.Y < 0f && projectile.oldVelocity.Y >= 0f || projectile.velocity.Y > 0f && projectile.oldVelocity.Y <= 0f))
                projectile.netUpdate = true;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null, BuffID.ShadowFlame, 300);
        }
        public override bool PreKill(int timeLeft)
        {
            if (projectile.scale > 0f)
            {
                projectile.scale -= 1f / 60f;
                return false;
            }
            else return true;
        }
        public override void Kill(int timeLeft)
        {
            ArchaeaNPC.DustSpread(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 10, 2f);
        }
        private bool TileLeft()
        {
            if (Main.tile[i - 1, j].active() && Main.tileSolid[Main.tile[i - 1, j].type])
            {
                projectile.position.X += 18f;
                return true;
            }
            return false;
        }
        private bool TileRight()
        {
            if (Main.tile[i + 1, j].active() && Main.tileSolid[Main.tile[i + 1, j].type])
            {
                projectile.position.X -= 18f;
                return true;
            }
            return false;
        }
        private bool TileTop()
        {
            if (Main.tile[i, j - 1].active() && Main.tileSolid[Main.tile[i, j - 1].type])
            {
                projectile.position.Y += 18f;
                return true;
            }
            return false;
        }
        private bool TileBottom()
        {
            if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type])
            {
                projectile.position.X -= 18f;
                return true;
            }
            return false;
        }
    }

    internal class Energy
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
            foreach (Dust d in Main.dust)
                if (d != null)
                {
                    if (Vector2.Distance(d.position - npc.position, Vector2.Zero) < range + 32)
                    {
                        d.velocity = ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(d.position, npc.Center), 3f);
                        Target.VelClamp(ref d.velocity, -3f, 3f, out d.velocity);
                    }
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
    internal class Target
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
            foreach (Player target in Main.player.Where(t => t.Distance(npc.Center) < range))
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
