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
    public class c_crystalsmall : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(2);
            TileObjectData.addTile(Type);
            drop = mod.ItemType("cinnabar_crystal");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cinnabar Crystal");
            AddMapEntry(new Color(210, 110, 110), name);
            disableSmartCursor = true;
            mineResist = 1.2f;
            minPick = 55;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.604f;
            g = 0.161f;
            b = 0.161f;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("cinnabar_crystal"), 1, true, 0, true, false);
        }

        bool tileCheckFlip = false;
        int x, y;
        int frame;
        float rotation;
        Texture2D texture;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {   
            SpriteEffects effects = SpriteEffects.None;
            Tile tile = Main.tile[i, j];

            if (Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].active() && Main.tile[i, j + 1].type != 0)
                frame = 3;
            if (Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].active() && Main.tile[i, j - 1].type != 0)
                frame = 0;
            if (Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].active() && Main.tile[i + 1, j].type != 0)
                frame = 2;
            if (Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].active() && Main.tile[i - 1, j].type != 0)
                frame = 1;

            texture = Main.tileTexture[Type];

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(texture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(0, frame * 18, 16, 16),
                Lighting.GetColor(i, j), 0f, default(Vector2), 1f, effects, 0f);
            return false;
        }
    }
}
