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
using Terraria.Localization;
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs
{
    public class Caster : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Caster")
                return false;
            return true;
        }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = 5000;
            npc.alpha = 255;
            npc.lavaImmune = true;
        }
        
        public bool hasAttacked;
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        public int elapse = 180;
        public int attacks
        {
            get { return (int)npc.ai[1]; }
            set { npc.ai[1] = value; }
        }
        public int pattern
        {
            get { return (int)npc.ai[2]; }
            set { npc.ai[2] = value; }
        }
        public int maxAttacks
        {
            get { return Main.rand.Next(3, 6); }
        }
        public int dustType;
        public Vector2 move;
        public Player npcTarget
        {
            get { return Main.player[npc.target]; }
        }
        public virtual Player nearbyPlayer()
        {
            Player player = ArchaeaNPC.FindClosest(npc, false, 800);
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                return player;
            }
            else return npcTarget;
        }
        private bool init;
        private int oldPattern;
        private const int
            Attacking = 2;

        public override bool PreAI()
        {
            return SinglePlayerAI();
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            knockback = 0f;
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            knockback = 0f;
        }
        public override void AI()
        {
        }
        public virtual bool JustSpawned()
        {
            return true;
        }
        public virtual void Teleport()
        {

        }
        public virtual bool PreAttack()
        {
            return true;
        }
        public virtual void Attack()
        {
        }
        private void SyncNPC(float x, float y)
        {
            if (Main.netMode == 2)
            {
                npc.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, npc.position.X, npc.position.Y);
                npc.netUpdate = true;
            }
            if (Main.netMode == 0)
            {
                npc.position = new Vector2(x, y);
            }
        }

        private bool SinglePlayerAI()
        {
            if (timer++ > elapse)
                timer = 0;
            ArchaeaNPC.SlowDown(ref npc.velocity);
            switch (pattern)
            {
                case PatternID.JustSpawned:
                    npc.target = ArchaeaNPC.FindClosest(npc, true).whoAmI;
                    if (JustSpawned())
                        goto case PatternID.FadeIn;
                    break;
                case PatternID.FadeIn:
                    pattern = PatternID.FadeIn;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        break;
                    }
                    else goto case PatternID.Idle;
                case PatternID.FadeOut:
                    pattern = PatternID.FadeOut;
                    if (npc.alpha < 255)
                    {
                        npc.immortal = true;
                        npc.alpha += 5;
                        break;
                    }
                    goto case PatternID.Teleport;
                case PatternID.Teleport:
                    pattern = PatternID.Teleport;
                    move = ArchaeaNPC.FindAny(npc, npcTarget);
                    if (move != Vector2.Zero)
                    {
                        SyncNPC(move.X, move.Y);
                        Teleport();
                        hasAttacked = false;
                        goto case PatternID.FadeIn;
                    }
                    break;
                case PatternID.Idle:
                    pattern = PatternID.Idle;
                    npc.immortal = false;
                    if (timer % elapse == 0 && Main.rand.Next(3) == 0)
                    {
                        if (!hasAttacked)
                        {
                            hasAttacked = true;
                            goto case PatternID.Attack;
                        }
                        else goto case PatternID.FadeOut;
                    }
                    return false;
                case PatternID.Attack:
                    pattern = PatternID.Attack;
                    if (PreAttack())
                    {
                        if (attacks > 0)
                            Attack();
                        attacks++;
                    }
                    if (attacks > maxAttacks)
                    {
                        pattern = PatternID.Idle;
                        attacks = 0;
                    }
                    return true;
            }
            if (oldPattern != pattern)
                SyncNPC(npc.position.X, npc.position.Y);
            oldPattern = pattern;
            return false;
        }
        private bool MPAI()
        {
            if (timer++ > elapse)
            {
                timer = 0;
                if (pattern < Attacking)
                    pattern++;
            }

            if (!init)
                init = JustSpawned();
            if (pattern == Attacking)
            {
                if (attacks < maxAttacks)
                {
                    if (timer % elapse == 0 && timer != 0)
                    {
                        attacks++;
                        Attack();
                    }
                }
                else
                {
                    if (npc.alpha < 255)
                        npc.alpha += 5;
                    else
                    {
                        move = ArchaeaNPC.FindAny(npc, Main.player[npc.target], true, 300);
                        if (move != Vector2.Zero)
                        {
                            pattern++;
                            Teleport();
                            SyncNPC(move.X, move.Y);
                        }
                    }
                }
                return true;
            }
            if (npc.alpha > 0)
                npc.alpha -= 5;
            else pattern = 0;
            return false;
        }
    }
}
