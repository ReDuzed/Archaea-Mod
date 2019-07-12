using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI.Chat;

using ArchaeaMod.Mode;

namespace ArchaeaMod.ModUI
{
    public class OptionsUI
    {
        private static string choiceName = "";
        private static string[] classes = new string[] { "Melee", "Ranged", "Magic", "Summoner", "All" };
        private static string[] categories = new string[] { "Class Select", "Cordoned Biomes", "Archaea Mode" };
        private static bool initElements;
        private static int choice = 0;
        private const int offset = 32;
        private static int oldWidth, oldHeight;
        private static Element[] classOptions;
        private static Element[] mainOptions;
        private static Element apply;
        private static Element back;
        private static Element selected;
        private static Element lookup;
        private static Element objectiveBox;
        private static SpriteBatch sb { get { return Main.spriteBatch; } }
        public static void Initialize()
        {
            mainOptions = new Element[categories.Length];
            classOptions = new Element[classes.Length];
            apply = new Element(Rectangle.Empty);
            back = new Element(Rectangle.Empty);
            oldWidth = Main.screenWidth;
            oldHeight = Main.screenHeight;
            classOptions = new Element[classes.Length];
            mainOptions = new Element[categories.Length];
            for (int i = 0; i < classes.Length; i++)
                classOptions[i] = new Element(Rectangle.Empty);
            for (int j = 0; j < categories.Length; j++)
                mainOptions[j] = new Element(Rectangle.Empty);
            UpdateLocation();
        }
        internal static void UpdateLocation()
        {
            apply.bounds = new Rectangle(Main.screenWidth / 2 - offset, Main.screenHeight / 2 + 48, 64, 64);
            back.bounds = new Rectangle(Main.screenWidth / 2 - offset, Main.screenHeight / 2 + 48, 64, 64);
            for (int i = 0; i < classes.Length; i++)
            {
                float angle = (float)Math.PI * 2f / classes.Length;
                int cos = (int)(Main.screenWidth / 2 + 196f * Math.Cos(angle * i - Math.PI / 2f));
                int sine = (int)(Main.screenHeight / 2 + 196f * Math.Sin(angle * i - Math.PI / 2f));
                classOptions[i].bounds = new Rectangle(cos - offset, sine - offset, 64, 64);
            }
            for (int j = 0; j < categories.Length; j++)
            {
                float angle = (float)Math.PI * 2f / categories.Length;
                int cos = (int)(Main.screenWidth / 2 + 196f * Math.Cos(angle * j - Math.PI / 2f));
                int sine = (int)(Main.screenHeight / 2 + 196f * Math.Sin(angle * j - Math.PI / 2f));
                mainOptions[j].bounds = new Rectangle(cos - offset, sine - offset, 64, 64);
            }
        }
        private static bool reset = true;
        private static bool classSelect;
        private static Mod mod;
        private static ArchaeaPlayer modPlayer;
        public static void MainOptions(Player player)
        {
            if (reset)
            {
                Initialize();
                apply.color = Color.Gray;
                reset = false;
            }
            if (classOptions != null && (oldWidth != Main.screenWidth || oldHeight != Main.screenHeight))
            {
                oldWidth = Main.screenWidth;
                oldHeight = Main.screenHeight;
                UpdateLocation();
            }
            mod = ModLoader.GetMod("ArchaeaMod");
            modPlayer = player.GetModPlayer<ArchaeaPlayer>(mod);
            var modWorld = mod.GetModWorld<ArchaeaWorld>();
            if (classSelect)
            {
                ClassSelect(player);
            }
            else
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    sb.Draw(mod.GetTexture("Gores/config_icons"), mainOptions[i].bounds, new Rectangle(44 * i, 0, 44, 44), mainOptions[i].color);
                    if (mainOptions[i].HoverOver())
                    {
                        if (i == 0 && mainOptions[i].LeftClick())
                            classSelect = true;
                        sb.DrawString(Main.fontMouseText, categories[i], new Vector2(mainOptions[i].bounds.X, mainOptions[i].bounds.Bottom), Color.White);
                    }
                }
                //currently without designation
                foreach (Element opt in mainOptions)
                {
                    if (opt.LeftClick())
                    {
                        opt.active = !opt.active;
                        opt.color = opt.active ? Color.Blue : Color.White;
                    }
                }
                //first player joined code
                /*
                { 
                    mainOptions[1].active = ModeToggle.archaeaMode;
                    mainOptions[2].active = modWorld.cordonBounds;
                    mainOptions[1].color = mainOptions[1].active ? Color.Blue : Color.White;
                    mainOptions[2].color = mainOptions[2].active ? Color.Blue : Color.White;
                }
                */
                sb.Draw(mod.GetTexture("Gores/config_icons"), apply.bounds, new Rectangle(44 * 4, 0, 44, 44), apply.color = selected != null ? Color.White : Color.Gray);
                if (apply.HoverOver())
                    sb.DrawString(Main.fontMouseText, "Apply", new Vector2(apply.bounds.X, apply.bounds.Bottom), Color.White);
                if (apply.LeftClick() && apply.color != Color.Gray)
                {
                    modPlayer.classChoice = choice + 1;
                    if (Main.netMode != 0)
                        NetHandler.Send(Packet.SyncClass, 256, -1, player.whoAmI, choice + 1, player.GetModPlayer<ArchaeaPlayer>().playerUID);
                    if (player == ArchaeaWorld.firstPlayer)
                    {
                        modWorld.cordonBounds = mainOptions[1].active;
                        ModeToggle.archaeaMode = mainOptions[2].active;
                    }
                }
            }
        }
        public static void ClassSelect(Player player)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                sb.Draw(mod.GetTexture("Gores/class_icons"), classOptions[i].bounds, new Rectangle(44 * i, 0, 44, 44), classOptions[i].color);
                if (classOptions[i].HoverOver())
                {
                    if (classOptions[i].LeftClick())
                    {
                        selected = classOptions[i];
                        choiceName = classes[i];
                        choice = i;
                    }
                    sb.DrawString(Main.fontMouseText, classes[i], new Vector2(classOptions[i].bounds.X, classOptions[i].bounds.Bottom), Color.White);
                }
            }
            sb.Draw(mod.GetTexture("Gores/config_icons"), back.bounds, new Rectangle(44 * 3, 0, 44, 44), selected != null ? Color.White : Color.Gray);
            if (back.HoverOver())
                sb.DrawString(Main.fontMouseText, "Go Back", new Vector2(back.bounds.X, back.bounds.Bottom), Color.White);
            if (selected != null)
            {
                if (selected.LeftClick())
                {
                    foreach (Element opt in classOptions)
                    {
                        if (opt != null && opt != selected)
                        {
                            opt.active = false;
                            opt.color = Color.White;
                        }
                    }
                    selected.active = true;
                    selected.color = Color.Blue;
                }
            }
            if (selected != null && back.LeftClick())
            {
                classSelect = false;
            }
        }
        public class Element
        {
            public Element(Rectangle bounds)
            {
                this.bounds = bounds;
            }
            public bool active;
            public Rectangle bounds;
            public Color color = Color.White;
            public Texture2D texture
            {
                get { return Main.magicPixel; }
            }
            private SpriteBatch sb
            {
                get { return Main.spriteBatch; }
            }
            public bool HoverOver()
            {
                return bounds.Contains(Main.MouseScreen.ToPoint());
            }
            public bool LeftClick()
            {
                return HoverOver() && ArchaeaPlayer.LeftClick();
            }
            public void Draw()
            {
                sb.Draw(texture, bounds, color);
            }
        }
    }
    public class TextBox
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
        public bool LeftClick()
        {
            return box.Contains(Main.MouseScreen.ToPoint()) && ArchaeaPlayer.LeftClick();
        }
        public bool HoverOver()
        {
            return box.Contains(Main.MouseScreen.ToPoint());
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
                        else if (key == Keys.OemPeriod)
                            text += ".";
                        else if (text.Length < 24 && key != Keys.OemPeriod)
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
    public class Button
    {
        public string text = "";
        public Color color(bool select = true)
        {
            if (select)
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f;
            else
            {
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.White : Color.White * 0.67f;
            }
        }
        public Rectangle box;
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        public Texture2D texture;
        public bool LeftClick()
        {
            return box.Contains(Main.MouseScreen.ToPoint()) && ArchaeaPlayer.LeftClick();
        }
        public Button(string text, Rectangle box, Texture2D texture = null)
        {
            this.texture = texture;
            if (texture == null)
                this.texture = Main.magicPixel;
            this.text = text;
            this.box = box;
        }
        public void Draw(bool select = true)
        {
            sb.Draw(texture, box, color(select));
            sb.DrawString(Main.fontMouseText, text, new Vector2(box.X + 2, box.Y + 2), Color.White * 0.90f);
        }
    }
}