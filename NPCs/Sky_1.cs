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
    public class Sky_1 : Sky_air
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resentful Spirit");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.knockBackResist = 0f;
            npc.damage = 10;
            npc.value = 4000;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
        }

        private bool init;
        private float upperPoint;
        private float oldX;
        private Vector2 idle;
        private Vector2 upper;
        public override bool PreAI()
        {
            if (!init)
            {
                if (Main.tile[(int)npc.position.X / 16, (int)npc.position.Y / 16].wall != ArchaeaWorld.skyBrickWall)
                {
                    Vector2 newPosition = ArchaeaNPC.FindAny(npc, target(), true, 800);
                    if (newPosition != Vector2.Zero)
                    {
                        npc.position = newPosition;
                        npc.netUpdate = true;
                    }
                    else return false;
                }
                oldX = npc.position.X;
                upperPoint = npc.position.Y - 50f;
                idle = npc.position;
                upper = new Vector2(oldX, upperPoint);
                init = true;
            }
            return PreSkyAI() && BeginActive();
        }
        private bool fade;
        private bool attack = true;
        private bool findNewTarget;
        private Vector2 move;
        public override void AI()
        {
            SkyAI();
        }

        private bool BeginActive()
        {
            if (amount < 1f)
            {
                amount += 0.02f;
                degree = 5d * amount;
                npc.position.Y = Vector2.Lerp(idle, upper, amount).Y;
                npc.position.X += (float)Math.Cos(degree);
                return false;
            }
            else return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0;
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
