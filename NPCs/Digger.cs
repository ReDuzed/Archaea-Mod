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

namespace ArchaeaMod.NPCs
{
    public class Digger : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Digger")
                return false;
            return true;
        }

        internal bool chaseThroughAir = true;
        private bool maxRange = true;
        internal const int maxTime = 600;
        internal int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        internal int bodyType;
        internal int tailType;
        public virtual int maxParts
        {
            get { return 8; }
            set { }
        }
        public virtual float acceleration
        {
            get { return 0.25f; }
        }
        public virtual float speedClamp
        {
            get { return 5f; }
        }
        internal Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, maxRange, 2048);
            if (player != null)
            {
                maxRange = false;
                return player;
            }
            else return Main.player[npc.target];
        }
        private bool init;
        private bool attack = true;
        private Vector2 chase;
        internal NPC neck;
        public bool PreWormAI()
        {
            if (!init)
            {
                int x = (int)npc.position.X;
                int y = (int)npc.position.Y;
                int type = bodyType;
                int[] parts = new int[maxParts];
                parts[0] = NPC.NewNPC(x, y, type, 0, npc.whoAmI);
                Main.npc[parts[0]].whoAmI = parts[0];
                Main.npc[parts[0]].realLife = npc.whoAmI;
                neck = Main.npc[parts[0]];
                for (int i = 1; i < maxParts; i++)
                {
                    type = i == maxParts - 1 ? tailType : bodyType;
                    parts[i] = NPC.NewNPC(x, y, type, 0, parts[i - 1], npc.whoAmI);
                    Main.npc[parts[i]].whoAmI = parts[i];
                    Main.npc[parts[i]].realLife = npc.whoAmI;
                }
                foreach (int part in parts)
                {
                    if (Main.netMode == 2)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, part);
                }
                init = true;
            }
            return init;
        }
        private float newRotation;
        public void WormAI()
        {
            if (!PreWormAI())
                return;
            if (timer++ > maxTime)
                timer = 0;
            if (timer % 300 == 0)
                SyncNPC(true);
            npc.rotation = npc.velocity.ToRotation();
            npc.noGravity = chaseThroughAir;
            if (attack && PreAttack())
            {
                Attacking();
                if (npc.Hitbox.Intersects(target().Hitbox) || target().dead || !target().active)
                    attack = false;
                chase = target().Center;
                if (timer % 30 == 0)
                    SyncNPC(chase.X, chase.Y);
            }
            else
            {
                if (timer % 120 == 0)
                {
                    chase = ArchaeaNPC.FindAny(npc, 400);
                    SyncNPC(chase.X, chase.Y);
                }
            }
            if (npc.Hitbox.Intersects(target().Hitbox))
            {
                chase = ArchaeaNPC.FindAny(npc, 400);
                SyncNPC(chase.X, chase.Y);
                attack = false;
            }
            if (StartDigging())
            {
                Digging();
                float acceleration = attack ? this.acceleration : 0.1f;
                float angle = npc.AngleTo(chase);
                float cos = (float)(acceleration * Math.Cos(angle));
                float sine = (float)(acceleration * Math.Sin(angle));
                npc.velocity += new Vector2(cos, sine);
                ArchaeaNPC.VelocityClamp(ref npc.velocity, speedClamp * -1, speedClamp);
            }
            if (npc.velocity.X < 0f && npc.oldVelocity.X >= 0f || npc.velocity.X > 0f && npc.oldVelocity.X <= 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y >= 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y <= 0f)
                SyncNPC();
        }

        private void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private void SyncNPC(float x, float y)
        {
            chase = new Vector2(x, y);
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private void SyncNPC(bool attack)
        {
            this.attack = attack;
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(chase);
            writer.Write(attack);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chase = reader.ReadVector2();
            attack = reader.ReadBoolean();
        }

        public virtual bool PreMovement()
        {
            return true;
        }
        public virtual void PostMovement()
        {
        }
        public virtual bool StartDigging()
        {
            return true;
        }
        public virtual void Digging()
        {

        }
        public virtual bool PreAttack()
        {
            return true;
        }
        public virtual void Attacking()
        {

        }
    }
}
