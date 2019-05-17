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

namespace ArchaeaMod.NPCs.Legacy
{
    public class Sky_air : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Sky_air")
                return false;
            return true;
        }

        public bool Hurt()
        {
            bool hurt = npc.life < npc.lifeMax && npc.life > 0 && oldLife != npc.life;
            oldLife = npc.life;
            return hurt;
        }
        private bool fade;
        private bool rangeOut;
        public bool proximity = true;
        public virtual bool aggroChance(int chance)
        {
            return Main.rand.Next(chance) == 0;
        }
        private int oldLife;
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        private int count;
        private float range;
        public virtual Rectangle Range
        {
            get { return defaultRange; }
        }
        public Rectangle defaultRange
        {
            get { return new Rectangle((int)npc.position.X - 200, (int)npc.position.Y - 150, 400, 300); }
        }
        public virtual Rectangle targetRange
        {
            get { return new Rectangle((int)target().position.X - 200, (int)target().position.Y - 150, 400, 300); }
        }
        private bool firstTarget = true;
        private Vector2 oldMove;
        internal Pattern pattern = Pattern.JustSpawned;
        public Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, firstTarget, 400);
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
            if (timer % 30 == 0 && false)
                Main.NewText(move + " : " + npc.position + " : " + target().position);
            if (timer++ > 600)
                timer = 0;
            if (proximity)
            {
                MaintainProximity();
                if (rangeOut)
                    return false;
            }
            if (pattern != Pattern.Attack)
                if (npc.alpha > 0)
                    npc.alpha -= 25;
            npc.direction = npc.position.X < move.X ? 1 : -1;
            if (oldMove != move && move != Vector2.Zero || npc.direction != npc.oldDirection)
                SyncNPC();
            oldMove = move;
            switch (pattern)
            {
                case Pattern.JustSpawned:
                    ai = 0;
                    attackRate = 180;
                    moveRate = 150;
                    range = 300f;
                    oldLife = npc.life;
                    if (JustSpawned())
                    {
                        if (aggroChance(80))
                            pattern = Pattern.Attack;
                        goto case Pattern.Idle;
                    }
                    return false;
                case Pattern.Idle:
                    pattern = Pattern.Idle;
                    DefaultAI();
                    if (Hurt())
                        goto case Pattern.Active;
                    if (npc.Distance(target().position) < range * 0.6f)
                        goto case Pattern.Active;
                    return true;
                case Pattern.Active:
                    pattern = Pattern.Active;
                    if (aggroChance(9000) || count > 6)
                    {
                        count = 0;
                        goto case Pattern.Attack;
                    }
                    Active();
                    return true;
                case Pattern.Attack:
                    pattern = Pattern.Attack;
                    AttackPattern();
                    return true;
            }
            if (!target().active || target().dead)
                DefaultAI();
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (rangeOut)
            {
                if (npc.alpha < 250)
                {
                    npc.alpha += 25;
                }
                else
                {
                    move = ArchaeaNPC.FindAny(npc, target(), false);
                    if (move != Vector2.Zero)
                    {
                        type = 0;
                        index = 0;
                        if (pattern != Pattern.Attack)
                            count++;
                        npc.position = move;
                        rangeOut = false;
                    }
                }
            }
            return npc.alpha < 250;
        }
        protected void MaintainProximity()
        {
            float right = target().Center.X + Main.screenWidth / 2.5f;
            float left = target().Center.X - Main.screenWidth / 2.5f;
            float down = target().Center.Y + Main.screenHeight / 2.5f;
            float up = target().Center.Y - Main.screenHeight / 2.5f;
            fade = npc.position.X > right || npc.position.X < left || npc.position.Y > down || npc.position.Y < up;
            if (fade)
                rangeOut = true;
        }
        protected bool DefaultAI()
        {
            reData = true;
            switch (ai)
            {
                case 0:
                    ai = 0;
                    if (PreDefaultMove())
                    {
                        type = 0;
                        index = 0;
                        goto case 1;
                    }
                    return false;
                case 1:
                    ai = 1;
                    move = ArchaeaNPC.FindAny(npc, target(), false);
                    if (npc.alpha < 225)
                        npc.alpha += 25;
                    else
                    {
                        if (move != Vector2.Zero)
                            npc.position = move;
                        goto case 2;
                    }
                    return true;
                case 2:
                    ai = 2;
                    if (npc.alpha > 0)
                        npc.alpha -= 25;
                    else goto case 0;
                    return true;
            }
            return false;
        }
        public bool reData = true;
        private bool moving;
        private int ai;
        private int type = -1;
        private int index;
        private int rand = 0;
        private int moveRate;
        private int moveCount;
        private float upperPoint;
        private float oldX;
        public float amount;
        public double degree;
        public Vector2 idle;
        public Vector2 upper;
        private Vector2 newMove;
        private Vector2[] points = new Vector2[5];
        protected void Active()
        {
            if (reData)
            {
                oldX = npc.position.X;
                upperPoint = npc.position.Y - 50f;
                idle = npc.position;
                upper = new Vector2(oldX, upperPoint);
                type = -1;
                points = new Vector2[5];
                reData = false;
            }
            switch (type)
            {
                case -1:
                    BeginInRange();
                    if (BeginActive())
                    {
                        type = 0;
                        index = 0;
                        goto case 0;
                    }
                    break;
                case 0:
                    Vector2 ground = ArchaeaNPC.FindEmptyRegion(target(), targetRange);
                    if (index < points.Length)
                    {
                        if (ground != Vector2.Zero)
                        {
                            points[index] = ground;
                            index++;
                        }
                    }
                    else
                    {
                        index = 0;
                        goto case 1;
                    }
                    break;
                case 1:
                    type = 1;
                    if (index++ > moveRate)
                    {
                        rand = Main.rand.Next(points.Length);
                        index = 0;
                        moveCount++;
                        count++;
                    }
                    newMove = points[rand];
                    ArchaeaNPC.PositionToVel(npc, newMove, 0.6f, 0.4f, true, -5f, 5f);
                    if (npc.Distance(target().position) > range * 2f || moveCount > 2)
                    {
                        moveCount = 0;
                        type = 0;
                        goto case 0;
                    }
                    break;
            }
        }

        private bool preAttack;
        private const int
            Teleport = 0,
            Attack = 1,
            Movement = 2,
            FadeOut = 5,
            FadeIn = 6;
        private int attack = -1;
        private int attacks;
        private int attackRate;
        private int time;
        public virtual int totalAttacks
        {
            get { return 4; }
        }
        public Vector2 move;
        protected void AttackPattern()
        {
            if (preAttack)
            {
                move = Vector2.Zero;
                attacks = 0;
                attack = -1;
                time = 0;
                preAttack = false;
                reData = true;
            }
            if (attack != FadeOut)
                npc.velocity = Vector2.Zero;
            switch (attack)
            {
                case -1:
                    goto case Teleport;
                case Attack:
                    if (BeginAttack() && time++ % attackRate == 0 && time != 0)
                    {
                        attacks++;
                        if (attacks > totalAttacks)
                        {
                            pattern = Pattern.Active;
                            reData = true;
                            preAttack = true;
                            break;
                        }
                        goto case Teleport;
                    }
                    break;
                case Teleport:
                    attack = Teleport;
                    if (PreFadeOut())
                        goto case FadeOut;
                    break;
                case FadeOut:
                    attack = FadeOut;
                    move = ArchaeaNPC.FindAny(npc, target(), false);
                    if (npc.alpha < 200)
                        npc.alpha += 5;
                    else if (BeginTeleport())
                    {
                        if (npc.Distance(target().position) < 800f)
                            if (move != Vector2.Zero)
                                npc.position = move;
                        goto case FadeIn;
                    }
                    break;
                case FadeIn:
                    attack = FadeIn;
                    if (npc.alpha > 0)
                        npc.alpha -= 5;
                    else goto case Attack;
                    break;
            }
        }

        public virtual bool JustSpawned()
        {
            return true;
        }
        public virtual bool PreDefaultMove()
        {
            return true;
        }
        public virtual void BeginInRange()
        {

        }
        public virtual bool BeginActive()
        {
            return true;
        }
        public virtual bool BeginAttack()
        {
            return true;
        }
        public virtual bool PreFadeOut()
        {
            return true;
        }
        public virtual bool BeginTeleport()
        {
            return true;
        }
        public void SyncNPC()
        {
            if (Main.netMode == 2)
                npc.netUpdate = true;
        }
    }
}
