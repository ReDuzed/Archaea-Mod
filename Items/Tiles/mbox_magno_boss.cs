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

namespace ArchaeaMod.Items.Tiles
{
    public class mbox_magno_boss : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Music Box");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.value = 3500;
            item.rare = 3;
            item.useStyle = 1;
            item.useTime = 10;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.noMelee = true;
            item.consumable = true;
            item.createTile = mod.TileType<ArchaeaMod.Tiles.music_boxes>();
        }
    }
}
