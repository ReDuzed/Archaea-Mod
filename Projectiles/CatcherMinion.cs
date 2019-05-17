using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;

namespace ArchaeaMod.Projectiles
{
    public class CatcherMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Catcher");
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 26;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }

        private int ai = -1;
        private int rand;
        private int elapsed;
        private int tries;
        private float idleSpeed = 3f;
        private float roam = 90f;
        private float range = 400f;
        private float followSpeed = 6f;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        private Target target;
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    projectile.position = owner.Center + new Vector2(32 * owner.direction, 0f);
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
                case 1:
                    if (ArchaeaItem.Elapsed(60))
                        elapsed++;
                    if (elapsed == 5)
                        goto case 0;
                    float angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, new Vector2(owner.Center.X, owner.position.Y + owner.height));
                    projectile.velocity = NPCs.ArchaeaNPC.AngleToSpeed(angle, followSpeed);
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        goto case 2;
                    return false;
                case 2:
                    ai = 2;
                    projectile.Center = owner.Center - new Vector2(owner.width / 2 * owner.direction, 16f);
                    if (target != null)
                        goto case 0;
                    break;
            }
            return true;
        }

        private int time;
        private Vector2 old;
        private Vector2 moveTo;
        public override void AI()
        {
            if (!owner.active || owner.dead || !owner.HasBuff(mod.BuffType<Buffs.buff_catcher>()))
                projectile.active = false;
            if (target == null || target.npc.life <= 0 || !target.npc.active)
            {
                if (ArchaeaItem.Elapsed(180))
                {
                    target = Target.GetClosest(owner, Target.GetTargets(projectile, 300f).Where(t => t != null).ToArray());
                    rand = Main.rand.Next(3);
                    old = projectile.Center;
                    Vector2 speed;
                    switch (rand)
                    {
                        case 0:
                            speed = NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.RandAngle(), idleSpeed);
                            projectile.velocity += speed;
                            break;
                        case 1:
                            speed = NPCs.ArchaeaNPC.AngleToSpeed((float)-Math.PI / 2f, idleSpeed);
                            moveTo = projectile.Center + speed;
                            Tile ground = Main.tile[(int)moveTo.X / 16, (int)moveTo.Y / 16];
                            if (ground.active())
                            {
                                projectile.velocity += speed;
                                break;
                            }
                            else goto case 0;
                        case 2:
                            if (Main.rand.NextFloat() < 0.15f)
                                ai = 1;
                            break;
                    }
                }
                if (projectile.Distance(old) > roam * 1.5f)
                    projectile.velocity = Vector2.Zero;
            }
            else if (!FindOwner())
            {
                ai = 0;
                float angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, target.npc.Center);
                if (ArchaeaItem.Elapsed(120))
                    projectile.velocity = NPCs.ArchaeaNPC.AngleToSpeed(angle, 10f);
                else NPCs.ArchaeaNPC.VelocityClamp(ref projectile.velocity, -6f, 6f);
            }
            FindGround(ai == 0);
        }
        protected bool FindGround(bool update)
        {
            if (update)
            {
                Tile ground = Main.tile[(int)projectile.Center.X / 16, (int)projectile.Center.Y / 16];
                if (!ground.active())
                    projectile.velocity.Y += 0.655f;
                return ground.active();
            }
            return false;
        }
        protected bool FindOwner()
        {
            if (projectile.Distance(owner.Center) > 400f)
            {
                Vector2 move = new Vector2(Main.rand.NextFloat(owner.position.X - 200f, owner.position.X + 200f), Main.rand.NextFloat(owner.position.Y - 200f, owner.position.Y + 200f));
                Tile ground = Main.tile[(int)move.X / 16, (int)move.Y / 16];
                if (ground.active())
                {
                    projectile.Center = move;
                    tries = 0;
                }
                if (tries++ > 300)
                {
                    ai = 1;
                    tries = 0;
                }
                return true;
            }
            return false;
        }
        public void FloatyAI()
        {
            if (target == null)
            {
                float angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, owner.Center);
                if (owner.controlJump || projectile.Distance(owner.Center) > 500f)
                    projectile.velocity.X += NPCs.ArchaeaNPC.AngleToSpeed(angle, 1f).X;
                if (owner.controlLeft || owner.controlRight)
                {
                    projectile.velocity.X += owner.velocity.X / projectile.Distance(owner.Center);
                    if (projectile.Center.X > owner.Center.X)
                        projectile.velocity.X -= 0.2f;
                    else projectile.velocity.X += 0.2f;
                }
                projectile.velocity.Y = owner.velocity.Y;
                NPCs.ArchaeaNPC.VelocityClamp(projectile, -8f, 8f);
            }
            else
            {

            }
        }
        public override bool PreKill(int timeLeft)
        {
            timeLeft = 36000;
            return false;
        }
    }
}
