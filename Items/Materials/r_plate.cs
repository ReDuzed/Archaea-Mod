using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Items.Materials
{
    public class r_plate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Plate");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.maxStack = 999;
        }
    }
}