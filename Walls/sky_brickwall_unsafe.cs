using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod.Walls
{
    public class sky_brickwall_unsafe : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;
            drop = mod.ItemType<Items.Walls.sky_brickwall>();
            AddMapEntry(new Color(80, 10, 10));
        }
    }
}
