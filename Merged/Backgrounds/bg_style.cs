using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Backgrounds
{
    public class bg_style : ModUgBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return false;
        }

        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/bg_magno");
            textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/bg_magno_surface");
            textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/bg_magno_connector");
            textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/bg_magno");
        }
    }   
} 