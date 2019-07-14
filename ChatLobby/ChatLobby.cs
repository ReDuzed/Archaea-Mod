using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Graphics;

using ArchaeaMod.ModUI;

namespace ArchaeaMod
{
    public class ChatLobby : ModPlayer
    {
        private bool init, hint, hintChat;
        private bool connected, connect, muted;
        public bool showLobbyConnect;
        private int oldText;
        private int hash;
        private int oldWidth, oldHeight;
        private BinaryWriter writer;
        private Button button, mute, lobbyButton, host;
        private TextBox[] details;
        private Timer timer;
        public override void PreUpdate()
        {
            if (!init)
            {
                button = new Button("Connect", new Rectangle(Main.screenWidth - 400, Main.screenHeight - 100, 10 * 10, 24));
                mute = new Button("Mute", new Rectangle(Main.screenWidth - 295, Main.screenHeight - 100, 5 * 10 + 5, 24));
                host = new Button("Host", new Rectangle(Main.screenWidth - 295, Main.screenHeight - 130, 5 * 10 + 5, 24));
                lobbyButton = new Button("Chat Lobby", new Rectangle(20, 256, 10 * 10, 24));
                details = new TextBox[]
                {
                    new TextBox(new Rectangle(Main.screenWidth - 400, Main.screenHeight - 70, 16 * 10, 24)),
                    new TextBox(new Rectangle(Main.screenWidth - 400, Main.screenHeight - 40, 16 * 10, 24)),
                };
                details[0].text = "IP Address";
                details[1].text = "Port";
                oldWidth = Main.screenWidth;
                oldHeight = Main.screenHeight;
                init = true;
            }
            if (Main.playerInventory)
            {
                if (lobbyButton.LeftClick())
                    showLobbyConnect = !showLobbyConnect;
            }
            string text = Main.chatText;
            if (writer != null && text.Length > 0 && ArchaeaPlayer.KeyPress(Keys.RightShift) && !muted)
            {
                writer.Write((byte)4);
                writer.Write(hash);
                writer.Write(text);
                writer.Flush();
                Main.NewText("[!] <" + player.name + "> " + text, new Color(150, 150, 150));
                text = string.Empty;
                Main.chatRelease = false;
                Main.drawingPlayerChat = false;
            }
            if (!showLobbyConnect)
                return;
            foreach (var t in details)
            {
                if (t.active && (t.text == "IP Address" || t.text == "Port"))
                    t.text = "";
                if (t.LeftClick() && t.HoverOver())
                {
                    t.active = !t.active;
                    foreach (var o in details)
                        if (o != t)
                            o.active = false;
                    break;
                }
                t.UpdateInput();
            }
            if (button.LeftClick())
            {
                foreach (var t in details)
                    t.active = false;
                if (!connected)
                    connected = ChatLobbyConnect();
                else
                {
                    connected = false;
                    writer.Write((byte)3);
                    writer.Flush();
                    writer.Close();
                    timer.Close();
                }
                button.text = connected ? "Disconnect" : "Connect";
            }
            if (mute.LeftClick() && connected)
            {
                muted = !muted;
                Main.NewText("Remote chat " + (muted ? "muted" : "not muted"), new Color(200, 200, 200));
            }
            if (oldWidth != Main.screenWidth || oldHeight != Main.screenHeight)
            {
                button.box = new Rectangle(Main.screenWidth - 400, Main.screenHeight - 100, 10 * 10, 24);
                mute.box = new Rectangle(Main.screenWidth - 295, Main.screenHeight - 100, 5 * 10 + 5, 24);
                host.box = new Rectangle(Main.screenWidth - 295, Main.screenHeight - 130, 5 * 10 + 5, 24);
                details[0].box = new Rectangle(Main.screenWidth - 400, Main.screenHeight - 70, 16 * 10, 24);
                details[1].box = new Rectangle(Main.screenWidth - 400, Main.screenHeight - 40, 16 * 10, 24);
                oldWidth = Main.screenWidth;
                oldHeight = Main.screenHeight;
            }
            if (host.LeftClick())
            {
                if (!Lobby.LobbyServer.hosting)
                {
                    if (details[1].text.Length > 3)
                    {
                        int num = 0;
                        if (!int.TryParse(details[1].text, out num))
                        {
                            if (!hint)
                            {
                                Main.NewText("Try a port number above 1000 (a number might help)");
                                hint = true;
                            }
                            return;
                        }
                        Lobby.LobbyServer.Main(new string[] { details[1].text });
                    }
                }
                else
                {
                    Main.NewText("Lobby closed");
                    Lobby.LobbyServer.hosting = false;
                    Lobby.LobbyServer.Disconnect();
                }
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!Main.hideUI && player.active && Main.playerInventory)
            {
                lobbyButton.Draw();
                if (showLobbyConnect)
                {
                    Main.spriteBatch.DrawString(Main.fontMouseText, "Chat Lobby", new Vector2(Main.screenWidth - 400, Main.screenHeight - 124), Color.WhiteSmoke);
                    button.Draw();
                    mute.Draw();
                    foreach (var t in details)
                        t.DrawText();
                    host.Draw();
                }
            }
        }
        private bool ChatLobbyConnect(string host = "")
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Parse(host == "" ? details[0].text : host), int.Parse(details[1].text));
            }
            catch
            {
                return false;
            }
            NetworkStream stream = new NetworkStream(client.Client);
            writer = new BinaryWriter(stream);
            BinaryReader reader = new BinaryReader(stream);
            writer.Write(player.name);
            hash = player.GetHashCode();
            writer.Write(hash);
            writer.Flush();
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += delegate (object sender, ElapsedEventArgs e) 
            { 
                if (stream.DataAvailable)
                {
                    byte b = reader.ReadByte();
                    switch (b)
                    {
                        case (byte)1:
                            writer.Write((byte)2);
                            writer.Flush();
                            break;
                        case (byte)4:
                            string name = reader.ReadString();
                            string message = reader.ReadString();
                            if (!muted) 
                                Main.NewText("[!] <" + name + "> " + message, new Color(200, 200, 200));
                            break;
                    }
                }
            };
            return client.Connected;
        }
    }
}