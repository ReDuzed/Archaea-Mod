using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ArchaeaMod.Mode
{
    public class ModeToggle : ModWorld
    {
        public static bool archaeaMode;
        public bool progress;
        public static float healthScale;
        public static float damageScale;
        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "ArchaeaMode", archaeaMode },
                { "HealthScale", healthScale },
                { "DamageScale", damageScale },
                { "DayCount", dayCount },
                { "TotalTime", totalTime }
            };
        }
        public override void Load(TagCompound tag)
        {
            archaeaMode = tag.GetBool("ArchaeaMode");
            healthScale = tag.GetFloat("HealthScale");
            damageScale = tag.GetFloat("DamageScale");
            dayCount = tag.GetFloat("DayCount");
            totalTime = tag.GetFloat("TotalTime");
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(archaeaMode);
            writer.Write(totalTime);
            writer.Write(dayCount);
        }
        public override void NetReceive(BinaryReader reader)
        {
            archaeaMode = reader.ReadBoolean();
            totalTime = reader.ReadSingle();
            dayCount = reader.ReadSingle();
        }
        private bool init;
        public override void PreUpdate()
        {
            if (!init)
                init = true;
        }
        public static float dayCount;
        public float totalTime;
        public override void PostUpdate()
        {
            //if (Main.netMode == 0 && ArchaeaPlayer.KeyPress(Keys.O))
            //    progress = !progress;
            totalTime += (float)Main.frameRate / 60f;
            dayCount = totalTime / (float)Main.dayLength;
        }
        public override void PostDrawTiles()
        {
            SpriteBatch sb = Main.spriteBatch;
            if (progress)
            {
                Rectangle panel = new Rectangle(20, 80, 180, 120);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Main.magicPixel, panel, Color.DodgerBlue * 0.67f);
                sb.DrawString(Main.fontMouseText, "Life scale: " + new ModeNPC().ModeChecksLifeScale(), new Vector2(panel.Left + 4, panel.Top + 4), Color.White);
                sb.DrawString(Main.fontMouseText, "Damage scale: " + new ModeNPC().ModeChecksDamageScale(), new Vector2(panel.Left + 4, panel.Top + 24), Color.White);
                sb.DrawString(Main.fontMouseText, "Day: " + Math.Round(dayCount + 1, 0), new Vector2(panel.Left + 4, panel.Top + 44), Color.White);
                sb.DrawString(Main.fontMouseText, "World time: " + Math.Round(totalTime / 60d / 60d, 1), new Vector2(panel.Left + 4, panel.Top + 64), Color.White);
                sb.End();
            }
        }
    }
    public class ModeNPC : GlobalNPC
    {
        public override bool InstancePerEntity 
        {
            get { return true; }
        }
        private bool init;
        private readonly int 
            start = 0, health = 1, mana = 2, bosses = 3, bottom = 4, npcs = 5, week= 6, crafting = 7, downedMagno = 8;
        private readonly float[] scaling = new float[] { 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f };
        private Mod Mod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public override void AI(NPC n)
        {
            if (!init)
            {
                if (ModeToggle.archaeaMode)
                {
                    n.lifeMax = (int)(n.lifeMax * (ModeToggle.healthScale = ModeChecksLifeScale()));
                    n.damage = (int)(n.damage * (ModeToggle.damageScale = ModeChecksDamageScale()));
                    n.life = n.lifeMax;
                }
                init = true;
            }
        }
        public float ModeChecksLifeScale()
        {
            float multiplier = 1f;
            if (!ModeToggle.archaeaMode)
                return multiplier;
            multiplier *= scaling[start];
            foreach (Player player in Main.player)
            {
                if (player != null)
                {
                    if (player.statLifeMax >= 200)
                    {
                        multiplier *= scaling[health];
                    }
                    if (player.statManaMax >= 100)
                    {
                        multiplier *= scaling[mana];
                    }
                    if (player.position.Y > Main.bottomWorld * 0.75f)
                    {
                        multiplier *= scaling[bottom];
                    }
                }
            }
            int count = 0;
            for (int i = 0; i < Main.townNPCCanSpawn.Length; i++)
            {
                if (Main.townNPCCanSpawn[i])
                {
                    if (count++ > 4)
                    {
                        multiplier *= scaling[npcs];
                        break;
                    }
                }
            }
            if (ModeToggle.dayCount > 6)
            {
                multiplier *= scaling[week];
            }
            if (ModeTile.tileProgress)
            {
                multiplier *= scaling[crafting];
            }
            if (Mod.GetModWorld<ArchaeaWorld>().downedMagno)
            {
                multiplier *= scaling[downedMagno];
            }
            return multiplier;
        }
        public float ModeChecksDamageScale()
        {
            float multiplier = 1f;
            if (!ModeToggle.archaeaMode)
                return multiplier;
            multiplier *= scaling[start];
            foreach (Player player in Main.player)
            {
                if (player != null)
                {
                    if (player.statLifeMax >= 200)
                    {
                        multiplier *= scaling[health];
                    }
                    if (player.statManaMax >= 100)
                    {
                        multiplier *= scaling[mana];
                    }
                    if (player.position.Y > Main.bottomWorld * 0.75f)
                    {
                        multiplier *= scaling[bottom];
                    }
                }
            }
            int count = 0;
            for (int i = 0; i < Main.townNPCCanSpawn.Length; i++)
            {
                if (Main.townNPCCanSpawn[i])
                {
                    if (count++ > 4)
                    {
                        multiplier *= scaling[npcs];
                        break;
                    }
                }
            }
            if (ModeToggle.dayCount > 6)
            {
                multiplier *= scaling[week];
            }
            if (ModeTile.tileProgress)
            {
                multiplier *= scaling[crafting];
            }
            if (Mod.GetModWorld<ArchaeaWorld>().downedMagno)
            {
                multiplier *= scaling[downedMagno];
            }
            return multiplier;
        }
    }

    public class ModeTile : GlobalTile
    {
        public static bool tileProgress;
        private int[,] playerCrafting = new int[5,3];
        public override void PlaceInWorld(int i, int j, Item item)
        {
            int type = 0;
            int[] crafting = new int[] 
            { 
                TileID.Anvils, 
                TileID.MythrilAnvil, 
                TileID.Furnaces,
                TileID.LihzahrdFurnace,
                TileID.Benches
            };
            for (int l = 0; l < crafting.Length; l++)
            {
                if (item.createTile == crafting[l])
                {
                    type = crafting[l];
                    break;
                }
            }
            if (type == 0) 
            {
                return;
            }
            int index = 0;
            for (int k = 0; k < 5; k++)
            {
                if (playerCrafting[k,0] == 0)
                {
                    index = k;
                    tileProgress = false;
                    break;
                }
                else if (k == 4)
                {
                    tileProgress = true;
                    return;
                }
            }
            foreach (int m in playerCrafting)
            {
                if (m == type)
                {
                    return;
                }
            }
            playerCrafting[index,0] = type;
            playerCrafting[index,1] = i;
            playerCrafting[index,2] = j;
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            for (int k = 0; k < playerCrafting.GetLength(0); k++)
            {
                if (type == playerCrafting[k, 0])
                {
                    for (int m = -2; m <= 2; m++)
                    for (int n = -2; n <= 2; n++)
                    {
                        if (playerCrafting[k, 1] - m == i && playerCrafting[k, 2] - n == j)
                        {
                            playerCrafting[k, 0] = 0;
                            break;
                        }
                    }
                }
            }
        }
    }
    public class ModePlayer : ModPlayer
    {
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (Main.playerInventory)
            {

            }
        }
    }
}

