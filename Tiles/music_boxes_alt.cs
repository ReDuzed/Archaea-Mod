using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Tiles
{
    public class music_boxes_alt : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 1, 0);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            disableSmartCursor = true;
            dustType = 1;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Music Box");
            AddMapEntry(Color.White, name);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            noItem = true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameY == 0)
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType<Items.Tiles.mbox_magno_boss>());
            else Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType<Items.Tiles.mbox_magno_2>());
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = Main.tile[i, j].frameY == 0 ? mod.ItemType<Items.Tiles.mbox_magno_boss>() : mod.ItemType<Items.Tiles.mbox_magno_1>();
        }
    }
}
