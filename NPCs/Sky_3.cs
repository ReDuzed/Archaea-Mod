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

using ArchaeaMod.NPCs;

namespace ArchaeaMod.NPCs
{
    public class Sky_3 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gargoyle");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 200;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = 150;
            npc.alpha = 255;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.lavaImmune = true;
        }
        private bool attack;
        private bool init;
        private bool firstTarget;
        
        private const float rushSpeed = 12f;
        private const float slowRate = 0.1f;
        private const float rotateSpeed = 0.05f;
        private Vector2 tracking;
        private Vector2 newPosition;
        private int time
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        private int ai
        {
            get { return (int)npc.ai[1]; }
            set { npc.ai[1] = value; }
        }
        private const int 
            Idle = 0,
            Activated = 1,
            Attack = 2;
        private Player npcTarget
        {
            get { return Main.player[npc.target]; }
        }
        public Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, firstTarget, 800);
            firstTarget = false;
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                npc.netUpdate = true;
                return player;
            }
            else return Main.player[npc.target];
        }
        public override void AI()
        {
            time++;
            if (!init)
            {
                int i = (int)npc.position.X / 16;
                int j = (int)npc.position.Y / 16;
                if (ArchaeaWorld.Inbounds(i, j) && Main.tile[i, j].wall != ArchaeaWorld.skyBrickWall)
                {
                    newPosition = ArchaeaNPC.FindAny(npc, ArchaeaNPC.FindClosest(npc, true), true, 800);
                    if (newPosition != Vector2.Zero)
                    {
                        npc.netUpdate = true;
                        init = true;
                    }
                }
            }
            if (newPosition != Vector2.Zero && ai == Idle)
                npc.position = newPosition;
            if (time > 150)
            {
                if (npc.alpha > 12)
                    npc.alpha -= 12;
                else npc.alpha = 0;
            }
            if (npc.target == 255)
            {
                if (npc.life < npc.lifeMax)
                    npc.TargetClosest();
                return;
            }
            if (time > 300)
            {
                time = 0;
                ai = Attack;
            }
            if (time == 240)
                tracking = npcTarget.Center;
            if (ai == Attack)
            {
                npc.velocity += ArchaeaNPC.AngleToSpeed(npc.AngleTo(tracking), rushSpeed);
                ai = Activated;
                npc.netUpdate = true;
            }
            else
            {
                ArchaeaNPC.SlowDown(ref npc.velocity, 0.2f);
                if (time % 45 == 0 && time != 0)
                    npc.velocity = Vector2.Zero;
            }
            if (npc.velocity.X >= npc.oldVelocity.X || npc.velocity.X < npc.oldVelocity.X || npc.velocity.Y >= npc.oldVelocity.Y || npc.velocity.Y < npc.oldVelocity.Y)
                npc.netUpdate = true;
            if (npc.Center.X <= npcTarget.Center.X)
            {
                float angle = (float)Math.Round(Math.PI * 0.2f, 1);
                if (npc.rotation > angle)
                npc.rotation -= rotateSpeed;
                else npc.rotation += rotateSpeed;
            }
            else
            {
                float angle = (float)Math.Round((Math.PI * 0.2f) * -1, 1);
                if (npc.rotation > angle)
                npc.rotation -= rotateSpeed;
                else npc.rotation += rotateSpeed;
            }
            if (ai == Idle && npc.Distance(npcTarget.Center) < 64f)
            {
                ArchaeaNPC.DustSpread(npc.position, npc.width, npc.height, DustID.Stone, 5, 1.2f);
                npc.TargetClosest();
                ai = Activated;
                npc.netUpdate = true;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 480);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(newPosition);
            writer.WriteVector2(tracking);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            newPosition = reader.ReadVector2();
            tracking = reader.ReadVector2();
        }
    }
}