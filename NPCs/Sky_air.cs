using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ArchaeaMod.NPCs
{
    public class Sky_air : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Sky_air")
                return false;
            return true;
        }
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        private bool firstTarget = true;
        public Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, firstTarget);
            firstTarget = false;
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                return player;
            }
            else return Main.player[npc.target];
        }
        private int oldLife;
        public bool Hurt()
        {
            bool hurt = npc.life < npc.lifeMax && npc.life > 0 && oldLife != npc.life;
            oldLife = npc.life;
            return hurt;
        }
        private bool init;
        private float upperPoint;
        private float oldX;
        public float amount;
        public double degree;
        private Vector2 idle;
        private Vector2 upper;
        public virtual bool PreSkyAI()
        {
            if (!init && JustSpawned())
            {
                oldX = npc.position.X;
                upperPoint = npc.position.Y - 50f;
                idle = npc.position;
                upper = new Vector2(oldX, upperPoint);
                init = true;
            }
            return init;
        }
        private bool fade;
        private bool attack = true;
        private bool findNewTarget;
        private Vector2 move;
        internal void SkyAI()
        {
            if (timer++ > 900)
            {
                SyncNPC(true);
                timer = 0;
            }
            if (timer % 300 == 0 && timer != 0)
                fade = true;
            findNewTarget = target() == null || !target().active || target().dead;
            if (!findNewTarget)
                MaintainProximity(300f);
            if (fade)
            {
                if (npc.alpha < 255 && PreFadeOut())
                    npc.alpha += 255 / 60;
                else if (timer % 90 == 0)
                {
                    if (!findNewTarget)
                    {
                        npc.position = ArchaeaNPC.FindAny(npc, target(), false, 300);
                        SyncNPC();
                    }
                    fade = false;
                }
            }
            else
            {
                if (npc.alpha > 0)
                    npc.alpha -= 255 / 60;
            }
            if (!fade && npc.alpha == 0)
            {
                if (timer % 150 == 0)
                {
                    if (!findNewTarget)
                    {
                        if (!attack)
                        {
                            move = ArchaeaNPC.FindAny(npc, target(), false, 300);
                            SyncNPC(move.X, move.Y);
                        }
                        else if (PreAttack())
                        {
                            move = target().Center;
                            SyncNPC(move.X, move.Y);
                        }
                    }
                }
                if (move != Vector2.Zero && (npc.position.X > move.X || npc.position.X < move.X || npc.position.Y > move.Y || npc.position.Y < move.Y))
                {
                    float angle = npc.AngleTo(move);
                    float cos = (float)(0.25f * Math.Cos(angle));
                    float sine = (float)(0.25f * Math.Sin(angle));
                    npc.velocity += new Vector2(cos, sine);
                    ArchaeaNPC.VelocityClamp(ref npc.velocity, -3f, 3f);
                    if (npc.velocity.X < 0f && npc.oldVelocity.X >= 0f || npc.velocity.X > 0f && npc.oldVelocity.X <= 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y >= 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y <= 0f)
                        SyncNPC();
                }
            }
        }

        private void MaintainProximity(float range)
        {
            if (!findNewTarget && !attack && npc.Distance(target().Center) > range)
            {
                move = ArchaeaNPC.FindAny(npc, target(), false, 200);
                SyncNPC(move.X, move.Y);
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0;
        }

        public virtual bool JustSpawned()
        {
            return true;
        }
        public virtual bool PreAttack()
        {
            return true;
        }
        public virtual bool PreFadeOut()
        {
            return true;
        }
        public virtual bool PostTeleport()
        {
            return true;
        }

        private void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private void SyncNPC(float x, float y)
        {
            if (Main.netMode == 2)
            {
                npc.netUpdate = true;
                move = new Vector2(x, y);
            }
        }
        private void SyncNPC(bool attack)
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
            this.attack = attack;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(move);
            writer.Write(attack);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            move = reader.ReadVector2();
            attack = reader.ReadBoolean();
        }
    }
}
