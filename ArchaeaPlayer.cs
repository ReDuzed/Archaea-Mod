using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using ReLogic.Graphics;

using ArchaeaMod.GenLegacy;
using ArchaeaMod.Mode;
using ArchaeaMod.ModUI;

namespace ArchaeaMod
{
    public class ArchaeaPlayer : ModPlayer
    {
        #region biome
        public bool MagnoBiome;
        public bool SkyFort;
        public override void UpdateBiomes()
        {
            ArchaeaWorld modWorld = mod.GetModWorld<ArchaeaWorld>();
            MagnoBiome = modWorld.MagnoBiome;
            SkyFort = modWorld.SkyFort;
        }
        public override bool CustomBiomesMatch(Player other)
        {
            if (MagnoBiome)
                return MagnoBiome = other.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome;
            if (SkyFort)
                return SkyFort = other.GetModPlayer<ArchaeaPlayer>(mod).SkyFort;
            return false;
        }
        public override void CopyCustomBiomesTo(Player other)
        {
            other.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome = MagnoBiome;
            other.GetModPlayer<ArchaeaPlayer>(mod).SkyFort = SkyFort;
        }
        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flag = new BitsByte();
            flag[0] = MagnoBiome;
            flag[1] = SkyFort;
            writer.Write(flag);
        }
        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flag = reader.ReadByte();
            MagnoBiome = flag[0];
            SkyFort = flag[1];
        }
        #endregion
        public static class ClassID
        {
            public const int
            None = 0,
            All = 1,
            Melee = 2,
            Magic = 3,
            Ranged = 4,
            Throwing = 5,
            Summoner = 6;
        }
        public int classChoice = 0;
        public int playerUID = 0;
        public override void Load(TagCompound tag)
        {
            playerUID = tag.GetInt("PlayerID");
            if (playerUID == 0)
                playerUID = GetHashCode();
        }
        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "PlayerID", playerUID },
            };
        }
        private bool start;
        public override void PreUpdate()
        {
            Color textColor = Color.Yellow;
            //  ITEM TEXT and SKY FORT DEBUG GEN
            if (!start)
            {
                if (Main.netMode == 0)
                {
                    Main.NewText("To enter commands, input LeftShift (instead of Enter)", textColor);
                    Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold Left Control and click to go to mouse", textColor);
                }
                else 
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Input /info and use [Left Shift] to list commands"), textColor);
                }
                start = true;
            }
            if (KeyHold(Keys.LeftAlt))
            {
                if (KeyPress(Keys.LeftControl))
                {
                    //SkyHall hall = new SkyHall();
                    //hall.SkyFortGen();
                    /*
                    Vector2 position;
                    do
                    {
                        position = new Vector2(WorldGen.genRand.Next(200, Main.maxTilesX - 200), 50);
                    } while (position.X < Main.spawnTileX + 150 && position.X > Main.spawnTileX - 150);
                    var s = new Structures(position);
                    s.InitializeFort();
                    */
                    for (int i = 0; i < Main.rightWorld / 16; i++)
                        for (int j = 0; j < Main.bottomWorld / 16; j++)
                        {
                            Main.mapInit = true;
                            Main.loadMap = true;
                            Main.refreshMap = true;
                            Main.updateMap = true;
                            Main.Map.Update(i, j, 255);
                            Main.Map.ConsumeUpdate(i, j);
                        }
                }
            }
            if (KeyHold(Keys.LeftControl) && LeftClick())
            {
                if (Main.netMode != 0)
                {
                    //  NetHandler.Send(Packet.TeleportPlayer, 256, -1, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
                }
                else player.Teleport(Main.MouseWorld);
            }
            string chat = (string)Main.chatText.Clone();
            bool enteredCommand = KeyPress(Keys.LeftShift);
            if (chat.StartsWith("/info"))
            {
                if (enteredCommand)
                {
                    if (Main.netMode != 2)
                        Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold Left Control and click to go to mouse", textColor);
                }
            }
            if (chat.StartsWith("/"))
            {
                if (chat.StartsWith("/list"))
                {
                    string[] npcs = new string[]
                    {
                        "Fanatic",
                        "Hatchling_head",
                        "Mimic",
                        "Sky_1",
                        "Sky_2",
                        "Slime_Itchy",
                        "Slime_Mercurial"
                    };
                    string[] items1 = new string[]
                    {
                        "cinnabar_bow",
                        "cinnabar_dagger",
                        "cinnabar_hamaxe",
                        "cinnabar_pickaxe",
                        "magno_Book",
                        "magno_summonstaff",
                        "magno_treasurebag",
                        "magno_trophy",
                        "magno_yoyo"
                    };
                    string[] items2 = new string[]
                    {
                        "c_Staff",
                        "c_Sword",
                        "n_Staff",
                        "r_Catcher",
                        "r_Flail",
                        "r_Javelin",
                        "r_Tomohawk",
                        "ShockLegs",
                        "ShockMask",
                        "ShockPlate"
                    };
                    string[] items3 = new string[]
                    {
                        "Broadsword",
                        "Calling",
                        "Deflector",
                        "Sabre",
                        "Staff"
                    };
                    if (chat.Contains("npcs"))
                    {
                        if (enteredCommand)
                            foreach (string s in npcs)
                                Main.NewText(s, textColor);
                    }
                    if (chat.Contains("items1"))
                    {
                        if (enteredCommand)
                            foreach (string s in items1)
                                Main.NewText(s, textColor);
                    }
                    if (chat.Contains("items2"))
                    {
                        if (enteredCommand)
                            foreach (string s in items2)
                                Main.NewText(s, textColor);
                    }
                    if (chat.Contains("items3"))
                    {
                        if (enteredCommand)
                            foreach (string s in items3)
                                Main.NewText(s, textColor);
                    }
                }
                if (chat.StartsWith("/npc"))
                {
                    string text = Main.chatText.Substring(Main.chatText.IndexOf(' ') + 1);
                    if (!chat.Contains("strike"))
                    {
                        if (enteredCommand)
                        {
                            NPC n = mod.GetNPC(text).npc;
                            if (Main.netMode != 0)
                                NetHandler.Send(Packet.SpawnNPC, 256, -1, n.type, Main.MouseWorld.X, Main.MouseWorld.Y, player.whoAmI, n.boss);
                            else
                            {
                                if (n.boss)
                                    NPC.SpawnOnPlayer(player.whoAmI, n.type);
                                else NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, n.type);
                            }
                        }
                    }
                    else
                    {
                        if (enteredCommand)
                            foreach (NPC npc in Main.npc)
                                if (npc.active && !npc.friendly && npc.life > 0)
                                    npc.StrikeNPC(npc.lifeMax, 0f, 1, true);
                    }
                }
                if (chat.StartsWith("/item"))
                {
                    string text = Main.chatText;
                    if (enteredCommand)
                    {
                        string itemType = text.Substring("/item ".Length);
                        string stackCount = "";
                        if (itemType.Count(t => t == ' ') != 0)
                            stackCount = itemType.Substring(text.LastIndexOf(' ') + 1);
                        bool modded = false;
                        int type;
                        int stack = 0;
                        if (modded = !int.TryParse(itemType, out type))
                            type = mod.ItemType(itemType);
                        if (modded)
                        {
                            int t = Item.NewItem(Main.MouseWorld, type, mod.GetItem(itemType).item.maxStack);
                            if (Main.netMode != 0)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
                        }
                        else
                        {
                            int.TryParse(stackCount, out stack);
                            int t2 = Item.NewItem(Main.MouseWorld, type, stack == 0 ? 1 : stack);
                            if (Main.netMode != 0)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t2);
                        }
                    }
                }
                if (chat.StartsWith("/spawn"))
                    if (enteredCommand)
                    {
                        if (Main.netMode != 0)
                            NetHandler.Send(Packet.TeleportPlayer, 256, -1, player.whoAmI, Main.spawnTileX * 16, Main.spawnTileY * 16);
                        else player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
                    }
                if (chat.StartsWith("/day"))
                {
                    if (enteredCommand)
                    {
                        float time = 10f * 60f * 60f / 2f;
                        if (Main.netMode == 0)
                        {
                            Main.dayTime = true;
                            Main.time = time;
                        }
                        else NetHandler.Send(Packet.WorldTime, 256, -1, 0, time, 0f, 0, true);
                    }
                }
                if (chat.StartsWith("/night"))
                {
                    if (enteredCommand)
                    {
                        float time = 8f * 60f * 60f / 2f;
                        if (Main.netMode == 0)
                        {
                            Main.dayTime = false;
                            Main.time = time;
                        }
                        else NetHandler.Send(Packet.WorldTime, 256, -1, 0, time, 0f, 0, false);
                    }
                }
                if (chat.StartsWith("/rain"))
                {
                    if (chat.Contains("off"))
                        if (enteredCommand)
                            Main.raining = false;
                    if (chat.Contains("on"))
                        if (enteredCommand)
                            Main.raining = true;
                }
            }
            if (enteredCommand)
            {
                Main.chatText = string.Empty;
                Main.drawingPlayerChat = false;
                Main.chatRelease = false;
            }
            if (KeyPress(Keys.O) && ModeToggle.archaeaMode && Main.chatText == "")
                NetHandler.Send(Packet.SyncInput, 256, -1, Main.LocalPlayer.whoAmI);
        }
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public static bool KeyHold(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (item.melee)
            {
                if (player.HasBuff(mod.BuffType<Buffs.flask_mercury>()))
                {
                    target.AddBuff(mod.BuffType<Buffs.mercury>(), 600);
                }
            }
        }

        private bool classChosen;
        private const int maxTime = 360;
        private int effectTime = maxTime;
        private int boundsCheck;
        public override void PostUpdate()
        {
            if (inBounds)
            {
                effectTime--;
                for (float i = 0; i < Math.PI * 2f; i += new Draw().radians(effectTime / 64f))
                {
                    int offset = 4;
                    float x = (float)(player.Center.X - offset + (effectTime / 4) * Math.Cos(i));
                    float y = (float)(player.Center.Y + (effectTime / 4) * Math.Sin(i));
                    var dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, DustID.Fire, Main.rand.NextFloat(-0.5f, 0.5f), -2f, 0, default(Color), 2f);
                    dust.noGravity = true;
                }
                if ((int)Main.time % 60 == 0)
                    boundsCheck++;
                if (boundsCheck == 5)
                {
                    if (oldPosition != Vector2.Zero)
                    {
                        if (Main.netMode == 0)
                            player.Teleport(oldPosition);
                        else NetHandler.Send(Packet.TeleportPlayer, 256, -1, player.whoAmI, oldPosition.X, oldPosition.Y);
                    }
                    oldPosition = Vector2.Zero;
                    effectTime = maxTime;
                    boundsCheck = 0;
                    inBounds = false;
                }
            }
            else
            {
                oldPosition = Vector2.Zero;
                effectTime = maxTime;
                boundsCheck = 0;
            }
            if (classChoice != 0 && !classChosen)
            {
                ArchaeaWorld.playerIDs.Add(playerUID);
                ArchaeaWorld.classes.Add(classChoice);
                classChosen = true;
            }
        }

        public override bool PreItemCheck()
        {
            Item item = player.inventory[player.selectedItem];
            bool nonTool = item.pick == 0 && item.axe == 0 && item.hammer == 0;
            switch (classChoice)
            {
                case -1:
                    if (nonTool && item.damage > 0)
                        return false;
                    break;
                case ClassID.Melee:
                    if (!item.melee)
                        goto case -1;
                    break;
                case ClassID.Magic:
                    if (!item.magic)
                        goto case -1;
                    break;
                case ClassID.Ranged:
                    if (!item.ranged)
                        goto case -1;
                    break;
                case ClassID.Summoner:
                    if (!item.summon)
                        goto case -1;
                    break;
                case ClassID.Throwing:
                    if (!item.thrown)
                        goto case -1;
                    break;
                case ClassID.All:
                    break;
            }
            return true;
        }
        public void ClassHotbar()
        {
            for (int i = 0; i < 10; i++)
            {
                Item item = player.inventory[i];
                bool nonTool = item.pick == 0 && item.axe == 0 && item.hammer == 0;
                switch (classChoice)
                {
                    case ClassID.Melee:
                        if (!item.melee && nonTool && item.damage > 0)
                        {
                            MoveItem(item);
                            item.type = ItemID.None;
                            return;
                        }
                        break;
                }
            }
        }
        private void MoveItem(Item item)
        {
            for (int i = player.inventory.Length - 10; i >= 10; i--)
            {
                Item slot = player.inventory[i];
                if (slot.Name == "" || slot.stack < 1 || slot == null || slot.type == ItemID.None)
                {
                    player.inventory[i] = item.DeepClone();
                    return;
                }
            }
        }
        private bool inBounds;
        private bool[] zones = new bool[index];
        private const int index = 12;
        private int[] unlocked = new int[index];
        private Vector2 oldPosition;
        public void BiomeBounds()
        {
            zones = new bool[]
            {
                player.ZoneBeach,
                player.ZoneCorrupt,
                player.ZoneCrimson,
                player.ZoneDesert,
                player.ZoneDungeon,
                player.ZoneHoly,
                player.ZoneJungle,
                player.ZoneMeteor,
                player.ZoneOverworldHeight,
                player.ZoneSnow,
                player.ZoneUndergroundDesert,
                SkyFort
            };
            unlocked[BiomeID.Overworld] = 1;
            var modWorld = mod.GetModWorld<ArchaeaWorld>();
            if (modWorld.cordonBounds)
            {
                for (int i = 0; i < unlocked.Length; i++)
                {
                    if (zones[i])
                    {
                        if (inBounds = !ObjectiveMet(i))
                        {
                            if (oldPosition == Vector2.Zero)
                                oldPosition = player.position;
                            break;
                        }
                        else
                        {
                            inBounds = false;
                        }
                    }
                }
            }
        }
        private bool ObjectiveMet(int zone)
        {
            if (unlocked[zone] == 1)
                return true;
            switch (zone)
            {
                case BiomeID.Beach:
                    return true;
                case BiomeID.Desert:
                    break;
                case BiomeID.Snow:
                    return true;
                case BiomeID.Fort:
                    break;
            }
            return false;
        }
        private float darkAlpha = 0f;
        private void DarkenedVision()
        {
            Texture2D texture = Main.magicPixel;
            Color color = Color.Black * (darkAlpha < 1f ? darkAlpha += 1f / 150f : 1f);
            int side = Main.screenWidth / 4;
            int top = Main.screenHeight / 4;
            sb.Draw(texture, new Rectangle(0, 0, side, Main.screenHeight), color);
            sb.Draw(texture, new Rectangle(Main.screenWidth - side, 0, side, Main.screenHeight), color);
            sb.Draw(texture, new Rectangle(side, 0, side * 2, top), color);
            sb.Draw(texture, new Rectangle(side, Main.screenHeight - top, side * 2, top), color);
        }
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        private Action<float, float> method;
        public bool classChecked;
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (classChoice == ClassID.None && drawInfo.drawPlayer.active && drawInfo.drawPlayer.whoAmI == Main.LocalPlayer.whoAmI)
            {
                if (ArchaeaWorld.playerIDs.Contains(playerUID))
                    classChoice = ArchaeaWorld.classes[ArchaeaWorld.playerIDs.IndexOf(playerUID)];
                OptionsUI.MainOptions(drawInfo.drawPlayer);
            }
            if (!SkyFort)
            {
                if (darkAlpha > 0f)
                    darkAlpha -= 1f / 150f;
            }
            else
            {
                DarkenedVision();
            }
        }
        public override Texture2D GetMapBackgroundImage()
        {
            if (MagnoBiome)
                return mod.GetTexture("Backgrounds/MapBGMagno");
            return base.GetMapBackgroundImage();
        }
        sealed class BiomeID
        {
            public const int
                Beach = 0,
                Corrupt = 1,
                Crimson = 2,
                Desert = 3,
                Dungeon = 4,
                Hallowed = 5,
                Jungle = 6,
                Meteor = 7,
                Overworld = 8,
                Snow = 9,
                UGDesert = 10,
                Fort = 11;
        }
    }

    public class Draw
    {
        public const float radian = 0.017f;
        public float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
}
