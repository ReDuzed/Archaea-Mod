using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

using ArchaeaMod.ModUI;

namespace Lobby
{
    public class LobbyServer
    {
        public static bool hosting;
        private static Chatter[] players = new Chatter[1001];
        static TcpListener queue;
        static System.Timers.Timer timer, loop;
        public static void Main(string[] args)
        {
            Terraria.Main.NewText("Lobby started at " + DateTime.Now);
            try
            {
                queue = new TcpListener(IPAddress.Any, int.Parse(args[0]));
                queue.Start();
            }
            catch 
            {
                return;
            }
            loop = new System.Timers.Timer(100);
            loop.Enabled = true;
            loop.AutoReset = true;
            loop.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                if (queue.Pending())
                {
                    NetworkStream stream = new NetworkStream(queue.AcceptSocket());
                    BinaryReader reader = new BinaryReader(stream);
                    Chatter.AddPlayer(stream);
                }
            };
            loop.Start();
            timer = new System.Timers.Timer(100);
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                foreach (Chatter player in LobbyServer.players.Where(t => t != null && t.connected))
                {
                    player.Update();
                }
            };
            timer.Start();
            hosting = true;
        }
        public static void Disconnect()
        {
            try
            {
                loop.Stop();
                loop.Dispose();
                timer.Stop();
                timer.Dispose();
                queue.Stop();
            }
            catch
            {
                return;
            }
        }
        internal class Chatter
        {
            private const byte
                NONE = 0,
                PING = 1,
                PONG = 2,
                QUIT = 3,
                USER_MSG = 4;
            public string name;
            public bool connected = true;
            private int count;
            private int hash;
            private const int second = 10;
            private BinaryReader reader;
            private BinaryWriter writer;
            private NetworkStream stream;
            public static void AddPlayer(Stream stream)
            {
                int index = 1000;
                for (int i = 0; i < LobbyServer.players.Length; i++)
                {
                    if (i == index)
                    {
                        return;
                    }
                    if (LobbyServer.players[i] == null || !LobbyServer.players[i].connected)
                    {
                        index = i;
                        break;
                    }
                }
                LobbyServer.players[index] = new Chatter();
                LobbyServer.players[index].reader = new BinaryReader(stream);
                LobbyServer.players[index].writer = new BinaryWriter(stream);
                LobbyServer.players[index].stream = (NetworkStream)stream;
                LobbyServer.players[index].Initialize();
            }
            private void Initialize()
            {
                name = reader.ReadString();
                hash = reader.ReadInt32();
                Terraria.Main.NewText(DateTime.Now.ToShortTimeString() + ": " + name + " has joined.");
            }
            public bool Update()
            {
                try
                {
                    NetReceive();
                    if (count++ == second * 15)
                        NetSend(PING);
                    if (count > second * 60)
                        Disconnect();
                }
                catch
                {
                    stream.Close();
                    Disconnect();
                    return true;
                }
                return connected;
            }
            public void NetSend(byte packet, string toWho = "", string fromWho = "", string key = "")
            {
                switch (packet)
                {
                    case PING:
                        writer.Write(packet);
                        break;
                    case USER_MSG:
                        writer.Write(packet);
                        writer.Write(fromWho);
                        writer.Write(key);
                        break;
                }
                writer.Flush();
            }
            private void NetReceive()
            {
                if (!stream.DataAvailable)
                    return;
                byte packet = reader.ReadByte();
                switch (packet)
                {
                    case PONG:
                        count = 0;
                        break;
                    case QUIT:
                        Disconnect();
                        break;
                    case USER_MSG:
                        int hash = reader.ReadInt32();
                        string msg = reader.ReadString();
                        foreach (Chatter p in LobbyServer.players.Where(t => t != null && t.hash != hash && t.connected))
                        {
                            p.NetSend(USER_MSG, "", name, msg);
                        }
                        count = 0;
                        break;
                }
            }
            public void Disconnect()
            {
                reader.Dispose();
                writer.Dispose();
                stream.Dispose();
                connected = false;
                Terraria.Main.NewText(name + " disconnected");
            }
        }
    }
}
