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
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 24;
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
        private bool chosenTexture;
        private int time
        {
            get { return (int)npc.ai[3]; }
            set { npc.ai[3] = value; }
        }
        private float upperPoint;
        private float oldX;
        private Vector2 idle;
        private Vector2 upper;
        private Vector2 newPosition;
        public override bool PreAI()
        {
            if (time++ > frameCount * frameTime)
                time = 0;
            if (!chosenTexture)
            {
                int rand = Main.rand.Next(4);
                switch (rand)
                {
                    case 0:
                        break;
                    case 1:
                        Main.npcTexture[npc.type] = mod.GetTexture("Gores/Sky_1_1");
                        npc.defense = 30;
                        npc.netUpdate = true;
                        break;
                    case 2:
                        Main.npcTexture[npc.type] = mod.GetTexture("Gores/Sky_1_2");
                        npc.defense = 20;
                        npc.netUpdate = true;
                        break;
                    case 3:
                        Main.npcTexture[npc.type] = mod.GetTexture("Gores/Sky_1_3");
                        npc.defense = 25;
                        npc.netUpdate = true;
                        break;
                }
                chosenTexture = true;
            }
            if (!init)
            {
                int i = (int)npc.position.X / 16;
                int j = (int)npc.position.Y / 16;
                if (ArchaeaWorld.Inbounds(i, j) && Main.tile[i, j].wall != ArchaeaWorld.skyBrickWall)
                {
                    newPosition = ArchaeaNPC.FindAny(npc, target(), true, 800);
                    if (newPosition != Vector2.Zero)
                    {
                        npc.netUpdate = true;
                        if (Main.netMode == 0)
                            npc.position = newPosition;
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
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 480);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null);
        }
        private int frame;
        private int frameCount;
        private int frameTime;
        public override void FindFrame(int frameHeight)
        {
            frameTime = 7;
            if (!Main.dedServ)
            {
                frameCount = Main.npcFrameCount[npc.type];
                frameHeight = Main.npcTexture[npc.type].Height / frameCount;
            }
            if (time % frameTime == 0 && time != 0)
            {
                if (frame < frameCount - 1)
                    frame++;
                else frame = 0;
            }
            npc.frame.Y = frame * frameHeight;
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
        private bool foundWall;
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (!foundWall)
                writer.WriteVector2(newPosition);
            writer.WriteVector2(move);
            writer.Write(attack);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (!foundWall)
            {
                npc.position = reader.ReadVector2();
                foundWall = true;
            }
            move = reader.ReadVector2();
            attack = reader.ReadBoolean();
        }
    }
}
