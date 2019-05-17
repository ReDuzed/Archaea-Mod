using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs
{
    public class Mimic : Slime
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mimic");
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 32;
            npc.lifeMax = 650;
            npc.defense = 10;
            npc.damage = 20;
            npc.knockBackResist = 1f;
            npc.value = Item.sellPrice(0, 1, 50, 0);
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

        private bool init;
        private bool activated;
        public override bool JustSpawned()
        {
            if (!init)
            {
                flip = Main.rand.Next(2) == 0;
                SyncNPC();
                init = true;
            }
            if (npc.life < npc.lifeMax || (Main.mouseRight && npc.Hitbox.Contains(Main.MouseWorld.ToPoint()) && ArchaeaNPC.WithinRange(target.position, new Rectangle(npc.Hitbox.X - 75, npc.Hitbox.Y - 75, 150, 150))))
            {
                pattern = Pattern.Attack;
                activated = true;
                SyncNPC();
            }
            return activated;
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
        }
        public override void Attack()
        {
            if (timer % 120 == 0 && timer != 0)
                SlimeJump(jumpHeight(FacingWall()), true, speedX(), target.position.X > npc.position.X);
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

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            float force = npc.knockBackResist * knockback;
            velX = player.Center.X > npc.Center.X ? force * -1 : force;
            SyncNPC();
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            float force = npc.knockBackResist * knockback;
            velX = projectile.position.X > npc.Center.X ? force * -1 : force;
            SyncNPC();
        }

        public override void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(activated);
            writer.Write(flip);
            writer.Write(velX);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            activated = reader.ReadBoolean();
            flip = reader.ReadBoolean();
            velX = reader.ReadSingle();
        }

        private int time;
        private int frame;
        public override void FindFrame(int frameHeight)
        {
            if (pattern == Pattern.JustSpawned)
                frame = 0;
            if (!Main.dedServ)
                frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            if (frame < 5 && timer++ % 3 == 0)
            {
                if (npc.velocity.Y < 0f)
                    frame++;
            }
            if (npc.velocity.Y == 0f && frame > 1 && timer++ % 3 == 0)
            {
                npc.spriteDirection = npc.position.X < target.position.X ? 1 : -1;
                frame--;
            }
            npc.frame.Y = frame * frameHeight;
        }
    }
}
