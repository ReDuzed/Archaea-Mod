using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod.Backgrounds
{
    public class bg_style : ModUgBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return Main.LocalPlayer.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome;
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