using System;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Terraria;
using TShockAPI;
using Hooks;

namespace Butterfingers
{
    [APIVersion(1, 11)]
    public class Butterfingers : TerrariaPlugin
    {
        Random rnd = new Random();
        public static List<Player> Players = new List<Player>();

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override string Name
        {
            get { return "Butterfingers"; }
        }

        public override string Author
        {
            get { return "Ported from TMod to TShock by Erik"; }
        }

        public override string Description
        {
            get { return ""; }
        }

        public Butterfingers(Main game)
            : base(game)
        {
            Order = -1;
        }

        public override void Initialize()
        {
            GameHooks.Update += OnUpdate;
            NetHooks.GreetPlayer += OnGreetPlayer;
            ServerHooks.Chat += OnChat;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Update -= OnUpdate;
                NetHooks.GreetPlayer -= OnGreetPlayer;
                ServerHooks.Chat -= OnChat;
            }

            base.Dispose(disposing);
        }

        private void OnUpdate()
        {
            Load();            
        }

        public static string randomString(params string[] args)
        {
            Random rnd = new Random();
            return args[rnd.Next(args.Length + 1)];
        }

        public static void Load()
        {
            Commands.ChatCommands.Add(new Command("stupidize", Butterfinger, "butterfingers"));
            Commands.ChatCommands.Add(new Command("stupidize", Stupidize, "stupidize"));
        }

        public void OnGreetPlayer(int ply, HandledEventArgs e)
        {
            lock (Players)
                Players.Add(new Player(ply));
        }

        public class Player
        {
            public int Index { get; set; }
            public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
            public bool butterfingered { get; set; }
            public bool stupidized { get; set; }
            public Player(int index)
            {
                Index = index;
                butterfingered = false;
                stupidized = false;
            }
        }

        private static int GetPlayerIndex(int ply)
        {
            lock (Players)
            {
                int index = -1;
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i].Index == ply)
                        index = i;
                }
                return index;
            }
        }

        public static TSPlayer GetTSPlayerByIndex(int index)
        {
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Index == index)
                    return player;
            }
            return null;
        }

        public static void Butterfinger(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /butterfingers [player]", Color.Red);
                return;
            }
            var foundplr = TShock.Utils.FindPlayer(args.Parameters[0]);
            if (foundplr.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
                return;
            }
            else if (foundplr.Count > 1)
            {
                args.Player.SendMessage(string.Format("More than one ({0}) player matched!", args.Parameters.Count), Color.Red);
                return;
            }
            var plr = foundplr[0];
            if (Players[GetPlayerIndex(plr.Index)].butterfingered)
            {
                Players[GetPlayerIndex(plr.Index)].butterfingered = false;
                args.Player.SendMessage("Player is no longer butterfingered.");
                return;
            }
            else
            {
                Players[GetPlayerIndex(plr.Index)].butterfingered = true;
                args.Player.SendMessage("Butterfingered " + plr.Name + "!");
                return;
            }
        }

        public static void Stupidize(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /stupidize [player]", Color.Red);
                return;
            }
            var foundplr = TShock.Utils.FindPlayer(args.Parameters[0]);
            if (foundplr.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
                return;
            }
            else if (foundplr.Count > 1)
            {
                args.Player.SendMessage(string.Format("More than one ({0}) player matched!", args.Parameters.Count), Color.Red);
                return;
            }
            var plr = foundplr[0];
            if (Players[GetPlayerIndex(plr.Index)].stupidized)
            {
                Players[GetPlayerIndex(plr.Index)].stupidized = false;
                args.Player.SendMessage("Player is no longer stupid.");
                return;
            }
            else
            {
                Players[GetPlayerIndex(plr.Index)].stupidized = true;
                args.Player.SendMessage("Stupidized " + plr.Name + "!");
                return;
            }
        }

        public void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            char[] text2 = text.ToCharArray();
            if (text2[0].ToString() == "/")
            {
                return;
            }
            TSPlayer player = GetTSPlayerByIndex(ply);

            if (Players[GetPlayerIndex(ply)].butterfingered)
            {
                for (int i = 0; i < text2.Length; i++)
                {
                    try
                    {
                        if (rnd.Next(10) == 3) text2[i] = (char)(((int)text[i]) + 1);
                    }
                    catch { } // meh, lazy mood
                }
                string textR = new string(text2);
                TShock.Utils.Broadcast(
                    String.Format(TShock.Config.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, textR),
                    player.Group.R, player.Group.G, player.Group.B);
                e.Handled = true;
                return;
            }
            if (Players[GetPlayerIndex(ply)].stupidized)
            {
                string[] words = text.ToLower().Replace(",", "").Replace(".", "").Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    switch (words[i])
                    {
                        case "hi":
                        case "hello":
                        case "greetings":
                        case "hai":
                            words[i] = randomString("hallo", "ello", "hullo");
                            break;

                        case "you":
                            if (words.Length - 1 > i && words[i + 1] == "are")
                            {
                                words[i + 1] = "";
                                words[i] = randomString("u", "ur", "yur", "yer", "er", "you'r");
                            }
                            else
                            {
                                words[i] = randomString("u", "yu", "uu", "uze");
                            }
                            break;

                        case "your":
                        case "you're":
                        case "youre":
                            words[i] = randomString("u", "ur", "yur", "yer", "er", "you'r");
                            break;

                        case "is":
                            words[i] = "am";
                            break;

                        case "am":
                            words[i] = "em";
                            break;

                        case "why":
                            words[i] = randomString("y", "wy", "wai");
                            break;

                        case "computer":
                            words[i] = randomString("puter", "pooter", "comp", "comptooter", "macintosh");
                            break;

                        case "how":
                            words[i] = "hw";
                            break;

                        case "are":
                            words[i] = randomString("r", "be");
                            break;

                        case "code":
                        case "codes":
                        case "word":
                        case "words":
                        case "sentance":
                        case "message":
                        case "messages":
                        case "sentances":
                            words[i] = randomString("squiggles", "squigglies", "squiggly lines");
                            break;

                        case "the":
                            words[i] = randomString("teh", "tuh", "duh", "d", "de");
                            break;

                        case "hate":
                        case "evil":
                        case "dislike":
                        case "bad":
                            words[i] = "no like";
                            break;

                        case "don't":
                        case "donot":
                        case "dont":
                            words[i] = "no";
                            break;

                        case "do":
                            if (words.Length - 1 > i && words[i + 1] == "not")
                            {
                                words[i + 1] = "";
                                words[i] = "no";
                            }
                            else
                            {
                                words[i] = "ye";
                            }
                            break;

                        case "lol":
                        case "rofl":
                        case "haha":
                        case "lmfao":
                        case "lmao":
                        case "roflmao":
                            words[i] = randomString("hahahahaha", "ha ha ha", "haaaaaaaaaaaa", "hahahahahahahaaaaa", "iggdigaiohgagdi");
                            break;

                        case "yes":
                        case "yeah":
                        case "yep":
                        case "yeh":
                        case "yuh":
                            words[i] = randomString("yah", "yeh", "noo");
                            break;

                        case "off":
                            words[i] = "of";
                            break;

                        case "this":
                            words[i] = "dis";
                            break;

                        case "my":
                            words[i] = "me";
                            break;

                        default:
                            char[] text3 = words[i].ToCharArray();
                            for (int i1 = 0; i1 < text.Length; i1++)
                            {
                                try
                                {
                                    if (rnd.Next(20) == 3) text3[i1] = (char)(((int)text[i1]) + 1);
                                }
                                catch { } // meh, lazy mood
                            }
                            words[i] = new string(text3);
                            break;
                    }
                } 
                string textR = string.Join(" ", words);
                TShock.Utils.Broadcast(
                    String.Format(TShock.Config.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, textR),
                    player.Group.R, player.Group.G, player.Group.B);
                e.Handled = true;
                return;
            }
        }
    }
}
