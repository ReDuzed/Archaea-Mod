using System.IO;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod
{
    public class ArchaeaMain : Mod
    {
        public static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        private bool swapTracks;
        private bool triggerSwap;
        public static string magnoHead = "ArchaeaMod/Gores/magno_head";
        public static string skyHead = "ArchaeaMod/Gores/sky_head";
        public override void Load()
        {
            AddBossHeadTexture(magnoHead, ModNPCID.MagnoliacHead);
            AddBossHeadTexture(skyHead, ModNPCID.SkyBoss);
            if (!Main.dedServ)
            {
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/The_Undying_Flare"), ItemType<Items.Tiles.mbox_magno_boss>(), TileType<Tiles.music_boxes>(), 0);
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Magno_Biome"), ItemType<Items.Tiles.mbox_magno_1>(), TileType<Tiles.music_boxes>(), 36);
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Dark_and_Evil_with_a_hint_of_Magma"), ItemType<Items.Tiles.mbox_magno_2>(), TileType<Tiles.music_boxes_alt>(), 36);
            }
        }
        public void SetModInfo(out string name, ref ModProperties properties)
        {
            name = "Archaea Mod";
            properties.Autoload = true;
            properties.AutoloadBackgrounds = true;
            properties.AutoloadGores = true;
            properties.AutoloadSounds = true;
        }
        
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            Player player = Main.LocalPlayer;
            if ((int)Main.time == Main.dayLength / 2)
                triggerSwap = true;
            if (Main.netMode != 2 && player.active && !Main.gameMenu)
            {
                if (player.GetModPlayer<ArchaeaPlayer>().MagnoBiome)
                {
                    if (!swapTracks)
                        music = GetSoundSlot(SoundType.Music, "Sounds/Music/Magno_Biome");
                    else music = GetSoundSlot(SoundType.Music, "Sounds/Music/Dark_and_Evil_with_a_hint_of_Magma");
                    priority = MusicPriority.BiomeHigh;
                }
                else if (triggerSwap)
                {
                    swapTracks = !swapTracks;
                    triggerSwap = false;
                }
            }
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetHandler.Receive(reader);
        }
    }
    public class NetHandler
    {
        private static Mod mod
        {
            get { return ArchaeaMain.getMod; }
        }
        public static void Send(byte type, int toWho = -1, int fromWho = -1, int i = 0, float f = 0f, float f2 = 0f, int i2 = 0, bool b = false)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write(type);
            packet.Write(i);
            packet.Write(f);
            packet.Write(f2);
            packet.Write(i2);
            packet.Write(b);
            packet.Send(toWho, fromWho);
        }
        public static void Receive(BinaryReader reader)
        {
            if (reader.PeekChar() == -1)
                return;
            int n = 0, n2 = 0;
            byte type = reader.ReadByte();
            int t = reader.ReadInt32();
            float f = reader.ReadSingle();
            float f2 = reader.ReadSingle();
            int i = reader.ReadInt32();
            bool b = reader.ReadBoolean();
            switch (type)
            {
                case Packet.WorldTime:
                    Main.dayTime = b;
                    Main.time = f;
                    NetMessage.SendData(7, -1, -1, null);
                    break;
                case Packet.SpawnNPC:
                    if (!b)
                    {
                        n = NPC.NewNPC((int)f, (int)f2, t);
                        Main.npc[n].whoAmI = n;
                        NetMessage.SendData(23, -1, -1, null, n);
                    }
                    else NPC.SpawnOnPlayer(i, t);
                    break;
                case Packet.SpawnItem:
                    NetMessage.SendData(21, -1, -1, null, t);
                    break;
                case Packet.TeleportPlayer:
                    if (Main.netMode == 2)
                        Send(Packet.TeleportPlayer, t, -1, t, f, f2);
                    else Main.player[t].Teleport(new Vector2(f, f2));
                    break;
                case Packet.StrikeNPC:
                    Main.npc[t].StrikeNPC(i, f, 0);
                    NetMessage.SendData(28, -1, -1, null, t);
                    break;
                case Packet.ArchaeaMode:
                    break;
                case Packet.SyncClass:
                    break;
                case Packet.SyncInput:
                    if (Main.netMode == 2)
                        Send(Packet.SyncInput, t, -1);
                    else
                    {
                        Mode.ModeToggle modWorld = mod.GetModWorld<Mode.ModeToggle>();
                        modWorld.progress = !modWorld.progress;
                    }
                    break;
            }
        }
    }
    public class Packet
    {
        public const byte
            WorldTime = 1,
            SpawnNPC = 2,
            SpawnItem = 3,
            TeleportPlayer = 4,
            StrikeNPC = 5,
            ArchaeaMode = 6,
            SyncClass = 7,
            SyncInput = 8;
    }
}
