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
        public bool SkyPortal;
        public override void UpdateBiomes()
        {
            ArchaeaWorld modWorld = mod.GetModWorld<ArchaeaWorld>();
            MagnoBiome = modWorld.MagnoBiome;
            SkyFort = modWorld.SkyFort;
            SkyPortal = modWorld.SkyPortal;
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
            other.GetModPlayer<ArchaeaPlayer>(mod).SkyPortal = SkyPortal;
        }
        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flag = new BitsByte();
            flag[0] = MagnoBiome;
            flag[1] = SkyFort;
            flag[2] = SkyPortal;
            writer.Write(flag);
        }
        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flag = reader.ReadByte();
            MagnoBiome = flag[0];
            SkyFort = flag[1];
            SkyPortal = flag[2];
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
            classChosen = tag.GetBool("Chosen");
        }
        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "PlayerID", playerUID },
                { "Chosen", classChosen }
            };
        }
        private bool start;
        public bool debugMenu;
        public bool spawnMenu;
        public override void PreUpdate()
        {
            Color textColor = Color.Yellow;
            //  ITEM TEXT and SKY FORT DEBUG GEN
            if (!start && !Main.dedServ)
            {
                if (Main.netMode == 0)
                {
                    Main.NewText("To enter commands, input [Tab] (instead of Enter), [F2] for item spawning using chat search via item name, [F3] for NPC debug and balancing", Color.LightBlue);
                    Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold [Left Control] and click to go to mouse", textColor);
                }
                if (Main.netMode == 2)
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Input /info and use [Tab] to list commands"), textColor);
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
                    var s = new Structures(position, ArchaeaWorld.skyBrick, ArchaeaWorld.skyBrickWall);
                    s.InitializeFort();
                    */
                    if (Main.netMode == 0)
                    {
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
            }
            if (KeyHold(Keys.LeftControl) && LeftClick())
            {
                if (Main.netMode == 2)
                    NetHandler.Send(Packet.TeleportPlayer, -1, -1, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
                else player.Teleport(Main.MouseWorld);
            }
            string chat = (string)Main.chatText.Clone();
            bool enteredCommand = KeyPress(Keys.Tab);
            if (chat.StartsWith("/info"))
            {
                if (enteredCommand)
                {
                    if (Main.netMode != 2)
                    {
                        Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold Left Control and click to go to mouse", textColor);
                        Main.NewText("Press [F2] and type an item name in chat, then hover over item icon", textColor);
                        Main.NewText("[F3] for NPC debug and balancing", textColor);
                    }
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
                        "Sky_3",
                        "Slime_Itchy",
                        "Slime_Mercurial",
                        "Magnoliac_head",
                        "Sky_boss",
                        "Sky_boss_legacy"
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
                                Main.NewText(s + " " + mod.NPCType(s), textColor);
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
                                {
                                    npc.StrikeNPC(npc.lifeMax, 0f, 1, true);
                                    if (Main.netMode != 0)
                                        NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI);
                                }
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
                        int.TryParse(stackCount, out stack);
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
                            NetHandler.Send(Packet.TeleportPlayer, 256, -1, Main.LocalPlayer.whoAmI, Main.spawnTileX * 16, Main.spawnTileY * 16);
                        else
                            player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
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
            if (KeyPress(Keys.F2))
            {
                if (Main.netMode == 1)
                    NetHandler.Send(Packet.Debug, 256, -1, player.whoAmI);
                else debugMenu = !debugMenu;
            }
            if (KeyPress(Keys.F3))
            {
                if (Main.netMode == 1)
                    NetHandler.Send(Packet.Debug, 256, -1, player.whoAmI, 1f);
                else spawnMenu = !spawnMenu;
            }
        }
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool RightHold()
        {
            return Main.mouseRight;
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
            if (outOfBounds)
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
                    outOfBounds = false;
                }
            }
            else
            {
                oldPosition = Vector2.Zero;
                effectTime = maxTime;
                boundsCheck = 0;
            }
            if (classChoice != ClassID.None && !classChosen)
            {
                Main.NewText(playerUID + " "  + classChoice);
                if (!ArchaeaWorld.playerIDs.Contains(playerUID))
                {
                    ArchaeaWorld.playerIDs.Add(playerUID);
                    ArchaeaWorld.classes.Add(classChoice);
                }
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
        private bool outOfBounds;
        private bool[] zones = new bool[index];
        private const int index = 12;
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
                SkyFort,
                MagnoBiome
            };
            var modWorld = mod.GetModWorld<ArchaeaWorld>();
            if (modWorld.cordonBounds)
            {
                for (int i = 0; i < zones.Length; i++)
                {
                    if (zones[i])
                    {
                        if (outOfBounds = !ObjectiveMet(i))
                        {
                            if (oldPosition == Vector2.Zero)
                                oldPosition = player.position;
                            break;
                        }
                        else
                        {
                            outOfBounds = false;
                        }
                    }
                }
            }
        }
        private bool ObjectiveMet(int zone)
        {
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
            if (!SkyFort)
            {
                if (darkAlpha > 0f)
                    darkAlpha -= 1f / 150f;
            }
            else
            {
                if (darkAlpha < 1f)
                    darkAlpha += 1f / 150f;
            }
            Texture2D texture = Main.magicPixel;
            Color color = Color.Black * darkAlpha;
            int range = 200;
            int side = Main.screenWidth / 2 - range;
            int top = Main.screenHeight / 2 - range;
            sb.Draw(texture, new Rectangle(0, 0, side, Main.screenHeight), color);
            sb.Draw(texture, new Rectangle(Main.screenWidth - side, 0, side, Main.screenHeight), color);
            sb.Draw(texture, new Rectangle(side, 0, range * 2, top), color);
            sb.Draw(texture, new Rectangle(side, Main.screenHeight - top, range * 2, top), color);
            sb.Draw(mod.GetTexture("Gores/fort_vignette_ui"), new Rectangle(side, top, range * 2, range * 2), Color.Black * darkAlpha);
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
            if (!Main.hardMode)
                DarkenedVision();
            if (debugMenu)
                DebugMenu();
            if (spawnMenu)
                SpawnMenu();
        }
        private bool init;
        private List<string> name = new List<string>();
        private List<int> id = new List<int>();
        private void DebugMenu()
        {
            if (!init || id == null || id.Count == 0 || name == null || name.Count == 0)
            {
                name.Clear();
                id.Clear();
                for (int i = 0; i < Main.itemTexture.Length; i++)
                {
                    int item = Item.NewItem(Vector2.Zero, i, 1);
                    name.Add(Main.item[item].Name);
                    id.Add(i);
                    if (item < Main.item.Length)
                        Main.item[item].active = false;
                }
                init = true;
            }
            Func<string, Texture2D[]> search = delegate(string Name)
            {
                List<Texture2D> t = new List<Texture2D>();
                if (Name.Length > 2 && name != null && name.Count > 0)
                {
                    for (int i = 0; i < name.Count; i++)
                    {
                        if (name[i].ToLower().Contains(Name.ToLower()))
                        {
                            t.Add(Main.itemTexture[i]);
                        }
                    }
                }
                t.Add(Main.magicPixel);
                return t.ToArray();
            };
            if (Main.chatText != null && Main.chatText.Length > 2)
            {
                Texture2D[] array = search(Main.chatText);
                if (array != null && array.Length > 0 && array[0] != Main.magicPixel)
                {
                    int index = Main.itemTexture.ToList().IndexOf(array[0]);
                    int x = 20;
                    int y = 112;
                    sb.Draw(array[0], new Vector2(x, y), Color.White);
                    sb.DrawString(Main.fontMouseText, string.Format("{0} {1}", name[index], id[index]), new Vector2(x + 50, y + 4), Color.White);

                    Rectangle grab = new Rectangle(x, y, 48, 48);
                    if (grab.Contains(Main.MouseScreen.ToPoint()))
                    {
                        sb.DrawString(Main.fontMouseText, "Left/Right click", new Vector2(x, y + 50), Color.White);
                        if (LeftClick() || RightHold())
                        {
                            int t = Item.NewItem(player.Center, index);
                            if (Main.netMode != 0)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
                        }
                    }
                }
            }
        }
        private bool initMenu;
        private string[] label;
        private Rectangle box;
        private TextBox[] input;
        private Button[] button;
        private void SpawnMenu()
        {
            int x = 80;
            int y = 180;
            int width = 300;
            int height = 106;
            if (!initMenu)
            {
                label = new string[]
                {
                    "ID",
                    "Life",
                    "Defense",
                    "Damage",
                    "KB Resist"
                };
                box = new Rectangle(x - 10, y, width + 164, height);
                input = new TextBox[]
                {
                    new TextBox(new Rectangle(x + 100, y + 4, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 24, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 44, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 64, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 84, width - 20, 18))
                };
                button = new Button[]
                {
                    new Button("Spawn", new Rectangle(x + width + 90, y + 4, 60, 18)),
                    new Button("Clear", new Rectangle(x + width + 90, y + 34, 60, 18))
                };
                initMenu = true;
            }
            sb.Draw(Main.magicPixel, box, Color.Black * 0.25f);
            for (int n = 0; n < label.Length; n++)
                sb.DrawString(Main.fontMouseText, label[n], new Vector2(x - 6, y + 4 + n * 20), Color.White * 0.9f);
            foreach (TextBox t in input)
            {
                if (t.box.Contains(Main.MouseScreen.ToPoint()) && LeftClick())
                {
                    foreach (var i in input)
                        i.active = false;
                    t.active = true;
                }
                if (t.active)
                    t.UpdateInput();
                t.DrawText();
            }
            foreach (Button b in button)
            {
                if (b.LeftClick())
                {
                    if (b.text == "Clear")
                    {
                        foreach (var t in input)
                        {
                            t.text = "";
                            t.active = false;
                        }
                    }
                    else if (b.text == "Spawn")
                    {
                        float[] vars = new float[5];
                        for (int i = 0; i < input.Length; i++)
                        {
                            float.TryParse(input[i].text, out vars[i]);
                        }
                        float randX = Main.rand.NextFloat(player.position.X - 300, player.position.X + 300);
                        float Y = player.position.Y - 100;
                        if (Main.netMode != 0)
                            NetHandler.Send(Packet.SpawnNPC, -1, -1, (int)vars[0], vars[1], vars[2], (int)vars[3], false, vars[4], Main.MouseWorld.X, Main.MouseWorld.Y);
                        else
                        {
                            int n = NPC.NewNPC((int)randX, (int)Y, (int)vars[0], 0);
                            Main.npc[n].lifeMax = (int)vars[1];
                            Main.npc[n].life = (int)vars[1];
                            Main.npc[n].defense = (int)vars[2];
                            Main.npc[n].damage = (int)vars[3];
                            Main.npc[n].knockBackResist = vars[4];
                        }
                    }
                }
                b.Draw();
            }
        }
        sealed class TextBox
        {
            public bool active;
            public string text = "";
            public Color color
            {
                get { return active ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f; }
            }
            public Rectangle box;
            private KeyboardState oldState;
            private KeyboardState keyState
            {
                get { return Keyboard.GetState(); }
            }
            private SpriteBatch sb
            {
                get { return Main.spriteBatch; }
            }
            public TextBox(Rectangle box)
            {
                this.box = box;
            }
            public void UpdateInput()
            {
                if (active)
                {
                    foreach (Keys key in keyState.GetPressedKeys())
                    {
                        if (oldState.IsKeyUp(key))
                        {
                            if (key == Keys.F3)
                                return;
                            if (key == Keys.Back)
                            {
                                if (text.Length > 0)
                                    text = text.Remove(text.Length - 1);
                                oldState = keyState;
                                return;
                            }
                            else if (key == Keys.Space)
                                text += " ";
                            else if (text.Length < 24)
                            {
                                string n = key.ToString().ToLower();
                                if (n.StartsWith("d") && n.Length == 2)
                                    n = n.Substring(1);
                                text += n;
                            }
                        }
                    }
                    oldState = keyState;
                }
            }
            public void DrawText()
            {
                sb.Draw(Main.magicPixel, box, color);
                sb.DrawString(Main.fontMouseText, text, new Vector2(box.X + 2, box.Y + 1), Color.White);
            }
        }
        sealed class Button
        {
            public string text = "";
            public Color color
            {
                get { return box.Contains(Main.MouseScreen.ToPoint()) ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f; }
            }
            public Rectangle box;
            private SpriteBatch sb
            {
                get { return Main.spriteBatch; }
            }
            public bool LeftClick()
            {
                return box.Contains(Main.MouseScreen.ToPoint()) && ArchaeaPlayer.LeftClick();
            }
            public Button(string text, Rectangle box)
            {
                this.text = text;
                this.box = box;
            }
            public void Draw()
            {
                sb.Draw(Main.magicPixel, box, color);
                sb.DrawString(Main.fontMouseText, text, new Vector2(box.X + 2, box.Y + 2), Color.White * 0.90f);
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
                Fort = 11,
                Magno = 12;
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
