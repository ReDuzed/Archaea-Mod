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
        private const float rushSpeed = 12f;
        private const float slowRate = 0.1f;
        private const float rotateSpeed = 0.05f;
        private Player target
        {
            get { return Main.player[npc.target]; }
        }
        private Vector2 tracking;
        private int time
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
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
                if (Main.tile[(int)npc.position.X / 16, (int)npc.position.Y / 16].wall != ArchaeaWorld.skyBrickWall)
                {
                    Vector2 newPosition = ArchaeaNPC.FindAny(npc, target, true, 800);
                    if (newPosition != Vector2.Zero)
                    {
                        npc.position = newPosition;
                        npc.netUpdate = true;
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
            }
            if (time == 240)
            {
                tracking = target.Center;
                npc.netUpdate = true;
            }
            if (attack)
            {
                npc.velocity += ArchaeaNPC.AngleToSpeed(npc.AngleTo(tracking), rushSpeed);
                attack = false;
            }
            ArchaeaNPC.SlowDown(ref npc.velocity, slowRate);
            if (npc.velocity.X > 0.1f || npc.velocity.X < -0.1f || npc.velocity.Y > 0.1f || npc.velocity.Y < 0.1f)
                npc.netUpdate = true;
            if (npc.Center.X <= target.Center.X)
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
            npc.target = target.whoAmI;
            activated = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(tracking);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            tracking = reader.ReadVector2();
        }
    }
}