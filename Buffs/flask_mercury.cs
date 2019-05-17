using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Buffs
{
    public class flask_mercury : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mercury Edge");
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
        }
    }
}
