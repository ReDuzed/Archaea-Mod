using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Merged.Tiles
{
	public class c_ore : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileSolid[Type] = true;
            //  connects with dirt
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = false;
            dustType = 1;
			drop = mod.ItemType("cinnabar_ore");
            //  UI map tile color
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cinnabar Ore");
            AddMapEntry(new Color(201, 152, 115), name);
            mineResist = 1.8f;
            minPick = 35;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}