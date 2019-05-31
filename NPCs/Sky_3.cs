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
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
        }
        private bool activated;
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
        public override bool PreAI()
        {
            npc.immortal = !activated;
            npc.dontTakeDamage = !activated;
            npc.noGravity = activated;
            npc.noTileCollide = activated;
            npc.color = !activated ? Color.Gray : Color.White;
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
                        init = true;
                    }
                    else return false;
                }
            }
            return activated;
        }
        public override void AI()
        {
            if (time++ > 300)
            {
                time = 0;
                attack = true;
                npc.netUpdate = true;
            }
            if (time == 240)
            {
                tracking = target().Center;
                npc.netUpdate = true;
            }
            if (attack)
            {
                npc.velocity += ArchaeaNPC.AngleToSpeed(npc.AngleTo(tracking), rushSpeed);
                npc.netUpdate = true;
                attack = false;
            }
            ArchaeaNPC.SlowDown(ref npc.velocity, slowRate);
            if (npc.velocity.X >= npc.oldVelocity.X || npc.velocity.X < npc.oldVelocity.X || npc.velocity.Y >= npc.oldVelocity.Y || npc.velocity.Y < npc.oldVelocity.Y)
                npc.netUpdate = true;
            if (npc.Center.X <= target().Center.X)
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
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (!activated)
                ArchaeaNPC.DustSpread(npc.position, npc.width, npc.height, DustID.Stone, 5, 1.2f);
            target.AddBuff(BuffID.Darkness, 480);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null);
            npc.target = target.whoAmI;
            npc.netUpdate = true;
            activated = true;
        }

        private bool findWall;
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (!findWall)
                writer.WriteVector2(newPosition);
            writer.WriteVector2(tracking);
            writer.Write(activated);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (!findWall)
            {
                npc.position = reader.ReadVector2();
                findWall = true;
            }
            tracking = reader.ReadVector2();
            activated = reader.ReadBoolean();
        }
    }
}