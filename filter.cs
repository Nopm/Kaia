using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeBot
{
    class filter
    {
        public static string[] allFiltersReplies =
        {
            "You aren't supposed to say that nya~ {mention}",
            "NO CURSIE >:C {mention}"
        };

        public class curse
        {
            public string word;
            public ulong serverID;
            public int muteTime;

            public curse(string w, ulong s, int m)
            {
                word = w;
                serverID = s;
                muteTime = m;
            }
        }

        public static List<curse> curseList = new List<curse>();

        public static void initCurses()
        {
            curseList.Clear();

            foreach(string d in Directory.GetDirectories("../servers/"))
            {
                foreach (string f in Directory.GetFiles(d + "/curses/"))
                {
                    int muteTime = 0;

                    if (File.ReadAllText(f).Length > 0)
                        int.TryParse(File.ReadAllText(f), out muteTime);

                    curseList.Add(new curse(f.Split('\\').Last().Split('/').Last(), ulong.Parse(d.Split('/')[2]), muteTime));

                    Console.WriteLine("Added curse " + f.Split('\\').Last().Split('/').Last() + " with ID " + ulong.Parse(d.Split('/')[2]) + " and mute time of " + File.ReadAllText(f));
                }
            }
        }

        public static bool isUserMod(SocketGuildUser u, SocketChannel c)
        {
            return u.GetPermissions((IGuildChannel)c).Has(ChannelPermission.ManageMessages);
        }

        public static void addCurse(SocketMessage e)
        {
            if (!isUserMod((SocketGuildUser)e.Author, (SocketChannel)e.Channel))
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " No permmies >:C");
                return;
            }

            string full = e.Content.Remove(0, (Program.getPrefix(e) + "filter add ").Length);

            int m = 0;

            try { int.TryParse(full.Split(' ')[1], out m); }
            catch { }

            File.WriteAllText("../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/curses/" + full.Split(' ')[0], m.ToString());

            initCurses();

            e.Channel.SendMessageAsync(e.Author.Mention + " Added a filter word D:");
        }

        public static void removeCurse(SocketMessage e)
        {
            if (!((SocketGuildUser)e.Author).GetPermissions((IGuildChannel)e.Channel).Has(ChannelPermission.ManageMessages))
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " No permmies >:C");
                return;
            }

            string full = e.Content.Remove(0, (Program.getPrefix(e) + "filter remove ").Length);

            if (File.Exists("../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/curses/" + full))
            {
                File.Delete("../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/curses/" + full);
                e.Channel.SendMessageAsync(e.Author.Mention + " Removed a filter word :D");
                initCurses();
                return;
            }

            

            e.Channel.SendMessageAsync(e.Author.Mention + " I couldn't find that filter word D;");
        }

        public static bool check(SocketMessage e)
        {
            foreach(curse c in curseList)
            {
                if (c.serverID == ((SocketGuildChannel)(SocketChannel)e.Channel).Guild.Id)
                {
                    if (e.Content.ToLower().Contains(c.word.ToLower()))
                    {
                        Random rnd = new Random();
                        e.DeleteAsync();
                        e.Channel.SendMessageAsync(allFiltersReplies[rnd.Next(0, allFiltersReplies.Length)].Replace("{mention}", e.Author.Mention));
                        if (c.muteTime > 0) mute.setMute(e, true, c.muteTime, e.Author, false);
                        return false;
                    }
                }
            }

            return true;

        }
    }
}
