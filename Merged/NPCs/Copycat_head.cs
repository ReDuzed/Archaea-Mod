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


namespace ArchaeaMod.Merged.NPCs
{
    public class Copycat_head : ArchaeaMod.NPCs.Digger
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Copycat");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.width = 26;
            npc.height = 38;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 0;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            bodyType = mod.NPCType<Copycat_body>();
            tailType = mod.NPCType<Copycat_tail>();
        }
        
        public override void AI()
        {
            WormAI();
        }
    }
}
