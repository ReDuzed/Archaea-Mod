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
    public class Orbital : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }
        private bool update = true;
        private int ai = -1;
        private float angle;
        private Vector2 center;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        private Target target;
        public override bool PreAI()
        {
            return PreAI(update && projectile.timeLeft > 30);
        }
        public bool PreAI(bool update)
        {
            if (update)
            {
                switch (ai)
                {
                    case -1:
                        center = owner.Center;
                        goto case 0;
                    case 0:
                        ai = 0;
                        angle = projectile.ai[0];
                        projectile.penetrate = -1;
                        float a = NPCs.ArchaeaNPC.AngleTo(center, Main.MouseWorld);
                        if (projectile.Distance(Main.MouseWorld) > projectile.width)
                            center += NPCs.ArchaeaNPC.AngleToSpeed(a, 10f);
                        else center = Main.MouseWorld;
                        projectile.Center = NPCs.ArchaeaNPC.AngleBased(center, angle, 45f);
                        if (owner.ownedProjectileCounts[projectile.type] == 6)
                            goto case 1;
                        return false;
                    case 1:
                        ai = 1;
                        projectile.penetrate = 1;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        private int type;
        public override void AI()
        {
            if (owner.inventory[owner.selectedItem].type != mod.ItemType<n_Staff>())
                projectile.Kill();
            if (ArchaeaItem.Elapsed(30))
            {
                target = Target.GetClosest(owner, Target.GetTargets(projectile, 300f).Where(t => t != null).ToArray());
                projectile.netUpdate = true;
            }
            switch (type)
            {
                case 0:
                    if (target == null)
                    {
                        center = Main.MouseWorld;
                        angle += Draw.radian * 4f;
                        projectile.Center = NPCs.ArchaeaNPC.AngleBased(center, angle, 45f);
                        return;
                    }
                    else type = 1;
                    break;
                case 1:
                    if (target == null || !target.npc.active || target.npc.life <= 0)
                        goto case 2;
                    float a = NPCs.ArchaeaNPC.AngleTo(projectile.Center, target.npc.Center);
                    projectile.velocity += Speed(a, target.npc.Center);
                    NPCs.ArchaeaNPC.VelocityClamp(projectile, -8f, 8f);
                    break;
                case 2:
                    float a2 = NPCs.ArchaeaNPC.AngleTo(projectile.Center, center);
                    center = Main.MouseWorld;
                    projectile.velocity += Speed(a2, center);
                    NPCs.ArchaeaNPC.VelocityClamp(projectile, -5f, 5f);
                    if (projectile.Distance(Main.MouseWorld) < 90f)
                        type = 0;
                    break;
                default:
                    break;
            }
        }
        public override void Kill(int timeLeft)
        {
            NPCs.ArchaeaNPC.DustSpread(projectile.Center, projectile.width, projectile.height, 6, 4, 2f);
        }
        public Vector2 Speed(float angle, Vector2 target)
        {
            return NPCs.ArchaeaNPC.AngleToSpeed(angle, projectile.Distance(target) * 0.5f);
        }
    }
}
