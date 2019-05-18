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

namespace ArchaeaMod.Items.Alternate
{
    public class Calling : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Calling");
            Tooltip.SetDefault("Conjures a minion");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 0;
            item.value = 3500;
            item.useTime = 60;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Green;
        }
        private int count;
        private int minions;
        private int buffType
        {
            get { return mod.BuffType<Minion.CallMinionBuff>(); }
        }
        private Projectile minion;
        public override bool UseItem(Player player)
        {
            minions = count + player.numMinions;
            if (minions == player.maxMinions || player.HasBuff(buffType))
            {
                if (minion != null)
                    minion.active = false;
                minion = Projectile.NewProjectileDirect(player.position - new Vector2(0, player.height), Vector2.Zero, mod.ProjectileType<Minion>(), 0, 0f, player.whoAmI);
            }
            if (!player.HasBuff(buffType))
            {
                player.AddBuff(buffType, 36000);
                minion = Projectile.NewProjectileDirect(player.position - new Vector2(0, player.height), Vector2.Zero, mod.ProjectileType<Minion>(), 0, 0f, player.whoAmI);
                count = player.ownedProjectileCounts[minion.type];
            }
            return true;
        }
    }

    public class Minion : ModProjectile
    {
        private int time
        {
            get { return (int)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }
        private int ai = -1;
        private int coolDown;
        public const int
            Reset = -2,
            Start = -1,
            Move = 0,
            Active = 1,
            Attack = 2;
        private float range = 300f;
        private float distance;
        private float maxDistance = 180f;
        private float angle;
        public float moveSpeed = 1.5f;
        public Vector2 moveTo;
        public Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public Target follow;
        public Target target;
        public Target[] targets;
        private Draw draw;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slowing Minion");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 0;
            projectile.knockBack = 0f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }
        public override bool PreAI()
        {
            if (!owner.HasBuff(mod.BuffType<CallMinionBuff>()))
            {
                owner.numMinions -= owner.ownedProjectileCounts[projectile.type];
                projectile.active = false;
                return false;
            }
            if (targets == null)
                return true;
            switch (ai)
            {
                case Reset:
                    ai = Reset;
                    if ((distance -= 5f) == 0f)
                    {
                        ai = Move;
                        goto case Move;
                    }
                    break;
                case Start:
                    draw = new Draw();
                    projectile.Center = Main.MouseWorld;
                    moveTo = projectile.Center;
                    ai = Move;
                    goto case Move;
                case Move:
                    if (coolDown > 0)
                        coolDown--;
                    if (coolDown == 0)
                    {
                        angle = 0f;
                        if (LeftClick())
                            moveTo = Main.MouseWorld;
                        if (!projectile.Hitbox.Contains(moveTo.ToPoint()))
                        {
                            projectile.velocity += NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(projectile.Center, moveTo), moveSpeed);
                            distance = 0f;
                        }
                        if (target != null && time-- < 0)
                        {
                            time = 0;
                            goto case Active;
                        }
                    }
                    else
                    {
                        moveTo = NPCs.ArchaeaNPC.AngleBased(owner.Center, angle += Draw.radian * 5f, 135f);
                        projectile.Center += NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(projectile.Center, moveTo), moveSpeed);
                        if (coolDown > 0)
                            coolDown--;
                    }
                    NPCs.ArchaeaNPC.VelocityClamp(projectile, -5f, 5f);
                    break;
                case Active:
                    ai = Active;
                    projectile.velocity = Vector2.Zero;
                    if (RightClick())
                    {
                        coolDown = time;
                        goto case Reset;
                    }
                    if (time++ < 300)
                        goto case Attack;
                    break;
                case Attack:
                    if (coolDown++ > 180)
                        goto case Reset;
                    if (target != null)
                    {
                        if (distance < maxDistance)
                            distance += 10f;
                        Color color = Color.DodgerBlue;
                        Lighting.AddLight(projectile.Center, color.R / 255f, color.G / 255f, color.B / 255f);
                    }
                    foreach (Target t in targets)
                        if (t.npc.Distance(projectile.Center) < distance)
                            t.AttackEffect(Target.Frozen);
                    break;

            }
            return true;
        }

        private int type = -2;
        private Vector2 move;
        public override void AI()
        {
            if (ArchaeaItem.Elapsed(60))
            {
                targets = Target.GetTargets(projectile, range).Where(t => t != null).ToArray();
                target = Target.GetClosest(owner, targets);
            }
            if (targets == null)
                return;
            if (RightClick())
            {
                foreach (Target tg in targets.Where(t => t != null))
                    if (tg.npc.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                        follow = tg;
            }
            if (follow == null)
                return;
            if (!follow.npc.active || follow.npc.Distance(owner.Center) > Main.screenWidth / 2f)
                follow = null;
            if (follow != null && ai != Attack)
            {
                moveTo = follow.npc.Center;
                if (!projectile.Hitbox.Contains(moveTo.ToPoint()))
                    projectile.velocity += NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(projectile.Center, moveTo), moveSpeed);
            }
            NPCs.ArchaeaNPC.VelocityClamp(projectile, -5f, 5f);
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (distance != 0f)
            {
                for (float r = 0f; r < distance; r += draw.radians(distance))
                {
                    Vector2 c = NPCs.ArchaeaNPC.AngleBased(projectile.Center, r, distance) - Main.screenPosition;
                    spriteBatch.Draw(Main.magicPixel, new Rectangle((int)c.X, (int)c.Y, 1, 1), Color.DodgerBlue * 0.50f);
                }
            }
        }
        public override bool PreKill(int timeLeft)
        {
            timeLeft = 36000;
            return false;
        }

        protected bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        protected bool RightClick()
        {
            return Main.mouseRightRelease && Main.mouseRight;
        }

        public class CallMinionBuff : ModBuff
        {
            public override void SetDefaults()
            {
                DisplayName.SetDefault("Minion Helper");
            }
            public override void ModifyBuffTip(ref string tip, ref int rare)
            {
                tip = "It eats ice";
            }
        }
    }
}
