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
    public class Hatchling_tail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hatchling");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 36;
            npc.height = 26;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 0;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
        }

        private NPC leader
        {
            get { return Main.npc[(int)npc.ai[0]]; }
        }
        private NPC head
        {
            get { return Main.npc[(int)npc.ai[1]]; }
        }
        private int spacing = 4;
        private float chaseSpeed = 5f;
        public override void AI()
        {
            npc.rotation = npc.AngleTo(leader.Center);
            if (npc.Distance(leader.Center) >= npc.width - spacing)
            {
                chaseSpeed += 0.2f;
                float angle = npc.AngleTo(leader.Center);
                float cos = (float)(chaseSpeed * Math.Cos(angle));
                float sine = (float)(chaseSpeed * Math.Sin(angle));
                npc.velocity = new Vector2(cos, sine);
            }
            else
            {
                npc.velocity = Vector2.Zero;
                chaseSpeed = 5f;
            }
            if (!head.active || head.life <= 0)
                npc.active = false;
        }
        public override bool CheckActive()
        {
            return head.active;
        }
    }
}
