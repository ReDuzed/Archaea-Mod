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

namespace ArchaeaMod.Items.Armors
{
    [AutoloadEquip(EquipType.Body)]
    public class ShockPlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock Plate");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.defense = 10;
            item.value = 5000;
            item.rare = ItemRarityID.Orange;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return
            head == mod.GetItem<ShockMask>().item &&
            body == item && 
            legs == mod.GetItem<ShockLegs>().item;
        }
    }

    public class Bolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 10;
            projectile.knockBack = 0f;
            projectile.alpha = 240;
            projectile.timeLeft = 40;
            projectile.friendly = true;
            projectile.tileCollide = false;
        }

        private int ai = -1;
        private float spawnY = 800f;
        private float speedY
        {
            get { return spawnY / 10f; }
        }
        private NPC target
        {
            get { return Main.npc[(int)projectile.ai[0]]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    projectile.Center = target.Center - new Vector2(0f, spawnY);
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }
        public override void AI()
        {
            if (projectile.alpha > 0)
                projectile.alpha -= 20;
            if (projectile.position.Y < target.position.Y + target.height - projectile.height)
                projectile.position.Y += speedY;
        }
    }
}
