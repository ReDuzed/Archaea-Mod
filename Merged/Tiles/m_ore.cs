using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

using ArchaeaMod.NPCs;

namespace ArchaeaMod.Merged.Tiles
{
	public class m_ore : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = false;
            dustType = 1;
			drop = mod.ItemType("magno_ore");
            //  UI map tile color
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Rubidium");
            AddMapEntry(new Color(201, 152, 115), name);
            soundStyle = 0;
            soundType = 21;
            mineResist = 1.5f;
            minPick = 35;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool Drop(int i, int j)
        {
            float chance = Main.rand.NextFloat();
            if (chance >= 0.30f)
                return true;
            int x = i * 16 + 8;
            int y = j * 16 + 8;
            float range = 3f * 16;
            Vector2 center = new Vector2(x, y);
            Player[] proximity = Main.player.Where(t => t.Distance(center) < range).ToArray();
            for (float k = 0; k < Math.PI * 2f; k++)
            {
                for (int l = 0; l < range; l++)
                {
                    Vector2 velocity = ArchaeaNPC.AngleToSpeed(k, 3f);
                    int rand = Main.rand.Next(20);
                    if (rand == 0)
                        Dust.NewDustDirect(ArchaeaNPC.AngleBased(center, k, range), 1, 1, DustID.Smoke, velocity.X, velocity.Y, 0, default(Color), 2f);
                    if (rand == 10)
                        Dust.NewDustDirect(ArchaeaNPC.AngleBased(center, k, l), 4, 4, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
                }
            }
            foreach (Player player in proximity)
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " struck dead in a mining accident"), 10, player.position.X / 16 < i ? -1 : 1);
            return true;
        }
    }
}
