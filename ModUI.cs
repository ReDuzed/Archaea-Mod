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
        private static string[] classes = new string[] { "All", "Melee", "Magic", "Ranged", "Throwing", "Summoner" };
        private static string[] categories = new string[] { "Class Select", "Archaea Mode", "Cordoned Biomes" };
        private static int choice = 0;
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
            int offset = 32;
            apply = new Element(new Rectangle(Main.screenWidth / 2 - offset, Main.screenHeight / 2 + 48, 64, 64));
            back = new Element(new Rectangle(Main.screenWidth / 2 - offset, Main.screenHeight / 2 + 48, 64, 64));
            for (int i = 0; i < classes.Length; i++)
            {
                float angle = (float)Math.PI * 2f / classes.Length;
                int cos = (int)(Main.screenWidth / 2 + 196f * Math.Cos(angle * i - Math.PI / 2f));
                int sine = (int)(Main.screenHeight / 2 + 196f * Math.Sin(angle * i - Math.PI / 2f));
                classOptions[i] = new Element(new Rectangle(cos - offset, sine - offset, 64, 64));
            }
            for (int j = 0; j < categories.Length; j++)
            {
                float angle = (float)Math.PI * 2f / categories.Length;
                int cos = (int)(Main.screenWidth / 2 + 196f * Math.Cos(angle * j - Math.PI / 2f));
                int sine = (int)(Main.screenHeight / 2 + 196f * Math.Sin(angle * j - Math.PI / 2f));
                mainOptions[j] = new Element(new Rectangle(cos - offset, sine - offset, 64, 64));
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
                    sb.Draw(Main.magicPixel, mainOptions[i].bounds, mainOptions[i].color);
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
                sb.Draw(Main.magicPixel, apply.bounds, apply.color = selected != null ? Color.White : Color.Gray);
                if (apply.HoverOver())
                    sb.DrawString(Main.fontMouseText, "Apply", new Vector2(apply.bounds.X, apply.bounds.Bottom), Color.White);
                if (apply.LeftClick() && apply.color != Color.Gray)
                {
                    modPlayer.classChoice = choice + 1;
                    if (Main.netMode != 0)
                        NetHandler.Send(Packet.SyncClass, 256, -1, player.whoAmI, choice + 1, player.GetModPlayer<ArchaeaPlayer>().playerUID);
                    if (player == ArchaeaWorld.firstPlayer)
                    {
                        ModeToggle.archaeaMode = mainOptions[1].active;
                        modWorld.cordonBounds = mainOptions[2].active;
                    }
                }
            }
        }
        public static void ClassSelect(Player player)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                sb.Draw(Main.magicPixel, classOptions[i].bounds, classOptions[i].color);
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
            sb.Draw(Main.magicPixel, back.bounds, selected != null ? Color.White : Color.Gray);
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
}