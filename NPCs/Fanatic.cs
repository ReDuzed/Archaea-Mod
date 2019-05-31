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
    public class Fanatic : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fanatical Caster");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 58;
            npc.lifeMax = 100;
            npc.defense = 10;
            npc.knockBackResist = 1f;
            npc.damage = 10;
            npc.value = 1000;
            npc.lavaImmune = true;
        }
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        public int maxAttacks
        {
            get { return 4; }
        }
        public int dustType;
        public Vector2 move;
        public Player npcTarget
        {
            get { return Main.player[npc.target]; }
        }
        private bool init;
        private float compensate
        {
            get { return (float)(npcTarget.velocity.Y * (0.017d * 5d)); }
        }
        private bool fade;
        public override void AI()
        {
            int attackTime = 180 + 90 * maxAttacks;
            if (timer++ > 60 + attackTime)
                timer = 0;
            ArchaeaNPC.SlowDown(ref npc.velocity, 0.1f);
            if (!init)
            {
                npc.target = ArchaeaNPC.FindClosest(npc, true).whoAmI;
                SyncNPC(npc.position.X, npc.position.Y);
                dustType = 6;
                var dusts = ArchaeaNPC.DustSpread(npc.Center, 1, 1, dustType, 10);
                foreach (Dust d in dusts)
                    d.noGravity = true;
                init = true;
            }
            if (!fade)
            {
                if (npc.alpha > 0)
                    npc.alpha -= 255 / 60;
            }
            else
            {
                if (npc.alpha < 255)
                    npc.alpha += 255 / 50;
                else
                {
                    timer = attackTime + 50;
                    move = ArchaeaNPC.FindAny(npc, npcTarget, true);
                    if (move != Vector2.Zero)
                    {
                        SyncNPC(move.X, move.Y);
                        var dusts = ArchaeaNPC.DustSpread(npc.Center - new Vector2(npc.width / 4, npc.height / 4), npc.width / 2, npc.height / 2, dustType, 10, 2.4f);
                        foreach (Dust d in dusts)
                            d.noGravity = true;
                        fade = false;
                        timer = 0;
                    }
                }
            }
            if (timer > 180 && timer <= attackTime)
            {
                OrbGrow();
                if (timer >= 180 + 60 && timer % 90 == 0)
                    Attack();
            }
            if (timer >= attackTime)
                fade = true;
            else fade = false;
        }
        private float scale = 0.2f;
        private float weight;
        private Dust energy;
        public void Attack()
        {
            int proj = Projectile.NewProjectile(npc.Center + new Vector2(npc.width * 0.35f * npc.direction, -4f), ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(npc, npcTarget) + compensate, 4f), ProjectileID.Fireball, 10, 1f);
            Main.projectile[proj].timeLeft = 300;
            Main.projectile[proj].friendly = false;
            Main.projectile[proj].tileCollide = false;
            scale = 0.2f;
        }
        public void OrbGrow()
        {
            npc.direction = npc.position.X < npcTarget.position.X ? 1 : -1;
            npc.spriteDirection = npc.direction;
            scale += 0.03f;
            energy = Dust.NewDustDirect(npc.Center + new Vector2(npc.width * 0.35f * npc.direction, -4f), 3, 3, dustType, 0f, -0.2f, 0, default(Color), scale);
            energy.noGravity = true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha < 20;
        }

        public void SyncNPC(float x, float y)
        {
            if (Main.netMode != 0)
                npc.netUpdate = true;
            else
            {
                npc.position = new Vector2(x, y);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(move);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.position = reader.ReadVector2();
        }

        private int frame;
        public override void FindFrame(int frameHeight)
        {
            int attackPhase = 90 * maxAttacks;
            if (timer < 180 || timer >= 180 + attackPhase)
                frame = 0;
            if (timer > 180 && timer < 180 + attackPhase && timer % 30 == 0)
                frame++;
            if (!Main.dedServ)
                frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            if (frame < 3)
                npc.frame.Y = frame * frameHeight;
            else frame = 0;
        }
    }
}
