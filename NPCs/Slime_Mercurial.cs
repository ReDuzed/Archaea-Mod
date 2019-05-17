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
    public class Slime_Mercurial : Slime
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Slime");
            Main.npcFrameCount[npc.type] = 2;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 32;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.lavaImmune = true;
        }
        private int count;
        private float compensateY;
        private bool preAI;
        public override bool PreAI()
        {
            if (npc.wet)
                npc.velocity.Y = 0.3f;
            preAI = SlimeAI();
            if (preAI)
            {
                if (npc.velocity.Y != 0f)
                    npc.velocity.X = velX;
            }
            return preAI;
        }
        public override void AI()
        {
            if (FacingWall())
                if (timer % interval / 4 == 0)
                    if (count++ > 3)
                    {
                        oldLife = npc.life;
                        pattern = Pattern.Active;
                        count = 0;
                    }
            base.AI();
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 300);
            NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null, BuffID.Darkness);
        }
        #region slime methods
        public override bool JustSpawned()
        {
            flip = Main.rand.Next(2) == 0;
            SyncNPC();
            return true;
        }
        public override void DefaultActions(int interval = 180, bool moveX = false)
        {
            if (timer % interval == 0 && timer != 0)
            {
                SlimeJump(jumpHeight(), moveX, speedX(), flip);
                flip = !flip;
            }
        }
        public override void Active(int interval = 120)
        {
            if (timer % interval == 0 && timer != 0)
            {
                SlimeJump(jumpHeight(FacingWall()), true, speedX(), flip);
                if (count++ > 3)
                {
                    flip = !flip;
                    count = 0;
                }
            }
            FadeTo(0, false);
        }
        public override void Attack()
        {
            pattern = Pattern.Attack;
            if (timer % 120 == 0 && timer != 0)
                SlimeJump(jumpHeight(FacingWall()), true, speedX(), target.position.X > npc.position.X);
            FadeTo(100, true);
            if (!target.active || target.dead)
                pattern = Pattern.Idle;
        }
        public override void SlimeJump(float speedY, bool horizontal = false, float speedX = 0, bool direction = true)
        {
            npc.velocity.Y -= speedY * 1.2f;
            if (horizontal)
            {
                velX = direction ? speedX / 2f : speedX / 2f * -1;
                SyncNPC();
            }
        }
        #endregion
        public override void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(flip);
            writer.Write(velX);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            flip = reader.ReadBoolean();
            velX = reader.ReadSingle();
        }

        private bool elapsed;
        private int time;
        private int time2;
        private int frame;
        private int height;
        public override void FindFrame(int frameHeight)
        {
            if (!Main.dedServ)
                frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            height = frameHeight;
            if (npc.velocity.Y != 0f)
            {
                if (time++ % 5 == 0)
                    elapsed = !elapsed;
                frame = elapsed ? 1 : 0;
            }
            else
            {
                if (pattern == Pattern.Attack)
                {
                    if (time2++ % 6 == 0)
                        elapsed = !elapsed;
                }
                else if (time2++ % 8 == 0)
                    elapsed = !elapsed;
                frame = elapsed ? 1 : 0;
            }
            npc.frame.Y = frame * frameHeight;
        }
    }
}
