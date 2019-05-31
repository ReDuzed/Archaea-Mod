using System;
using System.Collections.Generic;
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
    public class Sky_2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marauder");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 350;
            npc.alpha = 255;
            npc.lavaImmune = true;
        }

        private bool direction
        {
            get { return target().position.X > npc.position.X; }
        }
        private int timer
        {
            get { return (int)npc.ai[0]++; }
            set { npc.ai[0] = value; }
        }
        private int pathing
        {
            get { return (int)npc.ai[1]; }
            set { npc.ai[1] = value; }
        }
        private int redirectTimer
        {
            get { return (int)npc.ai[2]; }
            set { npc.ai[2] = value; }
        }
        public Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, false, 500);
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                return player;
            }
            else return Main.player[Main.myPlayer];
        }
        private bool init;
        private Vector2 newPosition;
        public override void AI()
        {
            if (!init)
            {
                pathing = 1;
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
                    else return;
                }
                init = true;
            }
            if (npc.alpha > 0)
                npc.alpha -= 5;
            if (timer++ > 600)
            {
                timer = 0;
                redirectTimer = 0;
            }
            if (timer % 300 == 0 && timer != 0)
                Attack();
            if (timer % 180 == 0)
            {
                if (FacingWall())
                    npc.velocity.Y -= 5f;
                else npc.velocity.Y -= 3f;
                SyncNPC();
            }
            if (canSee())
            {
                if (OnGround())
                    npc.velocity.X -= direction ? 0.25f * -1 : 0.25f;
                else npc.velocity.X -= direction ? 8f * -1 : 8f;
            }
            else
            {
                npc.velocity.X += pathing * 1f;
                if (FacingWall() && redirectTimer++ % 180 == 0)
                    pathing *= -1;
            }
            if (BottomLeftTile() && !TopLeftTile() && npc.velocity.X < 0f)
            {
                npc.position.X -= 2f;
                npc.position.Y -= 8f;
            }
            if (BottomRightTile() && !TopRightTile() && npc.velocity.X > 0f)
            {
                npc.position.X += 2f;
                npc.position.Y -= 8f;
            }
            ArchaeaNPC.VelClampX(npc, -3f, 3f);
            if (npc.velocity.X < 0f && npc.oldVelocity.X >= 0f || npc.velocity.X > 0f && npc.oldVelocity.X <= 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y >= 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y <= 0f)
                SyncNPC();
        }

        private int projIndex;
        private Projectile[] proj = new Projectile[6];
        private void Attack()
        {
            projIndex = 0;
            for (int k = -3; k <= 3; k += 2)
            {
                proj[projIndex] = Projectile.NewProjectileDirect(npc.Center, ArchaeaNPC.AngleToSpeed(ArchaeaNPC.RandAngle(), 4f), ProjectileID.ShadowFlame, 15, 5f);
                proj[projIndex].tileCollide = false;
                proj[projIndex].timeLeft = 180;
                proj[projIndex].hostile = true;
                proj[projIndex].friendly = false;
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj[projIndex].whoAmI);
                projIndex++;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 480);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.AddPlayerBuff, target.whoAmI, -1, null);
        }

        private bool FacingWall()
        {
            return TopLeftTile() && BottomLeftTile() || TopRightTile() && BottomRightTile();
        }
        private bool OnGround()
        {
            int x = (int)npc.position.X;
            int y = (int)npc.position.Y;
            return Collision.SolidTiles(x, x + npc.width, y, y + npc.height + 4);
        }
        private bool canSee()
        {
            Vector2 line;
            for (float k = 0; k < npc.Distance(target().position); k += 0.5f)
            {
                line = npc.Center + ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(npc, target()), k);
                int i = (int)line.X / 16;
                int j = (int)line.Y / 16;
                Tile tile = Main.tile[i, j];
                if (tile.active() && Main.tileSolid[tile.type])
                    return false;
            }
            return true;
        }
        private bool TopLeftTile()
        {
            int i = (int)(npc.position.X + 8) / 16;
            int j = (int)(npc.position.Y + 8) / 16;
            if (Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].active())
                return true;
            return false;
        }
        private bool TopRightTile()
        {
            int i = (int)(npc.position.X + npc.width - 8) / 16;
            int j = (int)(npc.position.Y + 8) / 16;
            if (Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].active())
                return true;
            return false;
        }
        private bool BottomLeftTile()
        {
            int i = (int)(npc.position.X + 8) / 16;
            int j = (int)(npc.position.Y + npc.height - 8) / 16;
            if (Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i -1, j].active())
                return true;
            return false;
        }
        private bool BottomRightTile()
        {
            int i = (int)(npc.position.X + npc.width - 8) / 16;
            int j = (int)(npc.position.Y + npc.height - 8) / 16;
            if (Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].active())
                return true;
            return false;
        }

        private void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
        private bool findWall;
        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            if (!findWall)
               writer.WriteVector2(newPosition);
        }
        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            if (!findWall)
            {
                npc.position = reader.ReadVector2();
                findWall = true;
            }
        }
    }
}
