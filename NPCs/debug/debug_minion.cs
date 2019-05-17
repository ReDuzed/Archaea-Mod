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
using Terraria.ModLoader;

namespace ArchaeaMod.NPCs.debug
{
    public class debug_minion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Debug Minion");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.scale = 0.5f;
            npc.alpha = 255;
            npc.lifeMax = 10;
            npc.friendly = true;
            npc.immortal = true;
            npc.lavaImmune = true;
        }
        private bool init;
        
        public static string name = "";
        public static bool boss;
        public static int type = -1;
        public override void AI()
        {
            if (!init)
                npc.TargetClosest();
            Player player = Main.player[npc.target];
            npc.color = player.eyeColor;
            npc.rotation += 0.1f;
            npc.position = player.position + new Vector2(0, 64);
            if (type != -1)
            {
                if (Main.netMode != 0)
                    SendData(type, "", boss, Main.MouseWorld);
                else NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, type);
                type = -1;
            }
            if (name != string.Empty)
            {
                if (Main.netMode != 0)
                    SendData(-1, name, false, Main.MouseWorld);
                else Item.NewItem(Main.MouseWorld, mod.ItemType(name));
                name = string.Empty;
            }
        }
        private void SendData(int type = -1, string name = "", bool boss = false, Vector2 position = default(Vector2))
        {
            ModPacket packet = mod.GetPacket();
            packet.Write(type);
            packet.Write(name);
            packet.Write(boss);
            packet.WriteVector2(position);
            packet.Send(256, -1);
        }
        public static void HandlePacket(BinaryReader reader)
        {
            Vector2 position = Vector2.Zero;
            if (reader.PeekChar() != -1)
            {
                type = reader.ReadInt32();
                name = reader.ReadString();
                boss = reader.ReadBoolean();
                position = reader.ReadVector2();
            }
            if (type != -1)
            {
                if (boss)
                    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, type);
                else
                {
                    int n = NPC.NewNPC((int)position.X, (int)position.Y, type);
                    Main.npc[n].whoAmI = n;
                    if (Main.netMode == 2)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
                boss = false;
                type = -1;
            }
            if (name != string.Empty)
            {
                Item item = ModLoader.GetMod("ArchaeaMod").GetItem(name).item;
                int i = Item.NewItem(position, item.type, item.maxStack);
                Main.item[i].whoAmI = i;
                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                name = string.Empty;
            }
        }
    }
}
