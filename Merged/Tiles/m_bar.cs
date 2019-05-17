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
    public class m_bar : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            
            Main.tileFrameImportant[Type]	= true;
			Main.tileLavaDeath[Type]		= false;
			Main.tileSolid[Type] 			= true;
            Main.tileSolidTop[Type]         = true;
            Main.tileMergeDirt[Type]		= false;
			Main.tileLighted[Type] 			= false;
			Main.tileBlockLight[Type]		= true;
			Main.tileNoSunLight[Type]		= false;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            dustType = 1;
            drop = mod.ItemType("magno_bar");
            //  UI map tile color
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Rubidium Bar");
            AddMapEntry(new Color(151, 102, 65), name);
            mineResist = 1.2f;
            minPick = 35;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
