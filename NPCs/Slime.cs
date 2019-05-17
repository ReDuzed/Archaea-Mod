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
    public class Slime : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Slime")
                return false;
            else return true;
        }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 48;
            npc.height = 32;
            npc.lifeMax = 50;
            npc.defense = 6;
            npc.damage = 10;
            npc.value = 500;
        }

        public bool flip;
        public bool moveX;
        public bool inRange;
        public virtual bool Hurt()
        {
            return npc.life < npc.lifeMax && npc.life > 0 && oldLife != npc.life;
        }
        public bool FacingWall()
        {
            if (Collision.SolidCollision(npc.position + new Vector2(-8f, 0f), npc.width + 16, npc.height))
                return true;
            else return false;
        }
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        public int counter;
        public int oldLife;
        public const int maxAggro = 2;
        public const int interval = 600;
        public virtual float jumpHeight(bool facingWall = false, bool inRange = false)
        {
            if (facingWall)
                return 2.5f * multi;
            if (!inRange)
                return 1.8f * multi;
            else return 2.2f * multi;
        }
        public virtual float speedX()
        {
            return 2f * multi;
        }
        public float multi
        {
            get { return Main.rand.NextFloat(2f, 4f); }
        }
        public float velX;
        public Pattern pattern = Pattern.JustSpawned;
        public Player target;
        public bool SlimeAI()
        {
            if (timer++ > interval)
                timer = 0;
            
            if (npc.velocity.Y == 0f && !npc.wet && Collision.SolidCollision(npc.position, npc.width, npc.height + 8))
                npc.velocity = Vector2.Zero;
            target = ArchaeaNPC.FindClosest(npc, true);
            if (target == null)
            {
                DefaultActions();
                return false;
            }
            inRange = ArchaeaNPC.WithinRange(target.position, ArchaeaNPC.defaultBounds(npc));
            switch (pattern)
            {
                case Pattern.JustSpawned:
                    if (JustSpawned())
                        goto case Pattern.Idle;
                    return false;
                case Pattern.Idle:
                    pattern = Pattern.Idle;
                    if (inRange)
                        goto case Pattern.Active;
                    DefaultActions(150, flip);
                    return true;
                case Pattern.Active:
                    if (inRange)
                    {
                        if (Hurt())
                            goto case Pattern.Attack;
                    }
                    else if (timer % interval / 4 == 0)
                        counter++;
                    if (counter > maxAggro)
                    {
                        counter = 0;
                        goto case Pattern.Idle;
                    }
                    Active();
                    return true;
                case Pattern.Attack:
                    Attack();
                    return true;
                default:
                    return false;
            }
        }
        public virtual bool JustSpawned()
        {
            return true;
        }
        public virtual void DefaultActions(int interval = 180, bool moveX = false)
        {
        }
        public virtual void Active(int interval = 120)
        {
        }
        public virtual void Attack()
        {
        }
        public virtual void SlimeJump(float speedY, bool horizontal = false, float speedX = 0f, bool direction = true)
        {
        }
        public void FadeTo(int alpha, bool fadeOut = true)
        {
            if (fadeOut)
            {
                if (npc.alpha < alpha)
                    npc.alpha++;
            }
            else if (npc.alpha > 0)
                npc.alpha--;
        }
        public virtual void SyncNPC()
        {
        }
    }
}
