using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnimeBot
{
    class levels
    {
        public class timerInfo
        {
            public SocketUser user;
            public int time;

            public timerInfo(SocketUser u, int t)
            {
                user = u;
                time = t;
            }
        }

        public static void checkEXP(SocketMessage e)
        {
            int l = getLevel(e.Author, ((SocketGuildChannel)e.Channel).Guild);

            if (!isUserInTimer(e.Author))
            {
                giveEXP(e.Author, ((SocketGuildChannel)e.Channel).Guild);
            }

            Console.WriteLine(e.Author.Username + " is level " + l);

            if (l < getLevel(e.Author, ((SocketGuildChannel)e.Channel).Guild))
            {
                l++;

                if (l == 2)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Member", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 2 and have the member role and other fun stuff! ^_^");
                    return;
                }
                if (l == 10)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Beginner Chatter", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 10 and have access to the Beginner Chatter role! ^_^");
                    return;
                }
                if (l == 15)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("General Conversationisitisitnial", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 15 and have access to the General Conversationisitisitnial role! ^_^");
                    return;
                }
                if (l == 30)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Finally Not Bad", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 30 and have access to the Finally Not Bad role! ^_^");
                    return;
                }
                if (l == 40)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Casual Gamer", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 40 and have access to the Casual Gamer role! ^_^");
                    return;
                }
                if (l == 50)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Hardcore Gamer", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 50 and have access to the Hardcore Gamer role! ^_^");
                    return;
                }
                if (l == 60)
                {
                    ((SocketGuildUser)e.Author).AddRoleAsync(getRole("Top Chatter", ((SocketGuildChannel)e.Channel).Guild));
                    e.Channel.SendMessageAsync(e.Author.Mention + " You're now level 60 and have access to the Top Chatter role! ^_^");
                    return;
                }

                if (l < 30)
                {
                    if (l % 5 == 0)
                        e.Channel.SendMessageAsync(e.Author.Mention + " You're now level " + l + "!! ^_^");
                }
                else e.Channel.SendMessageAsync(e.Author.Mention + " You're now level " + l + "!! ^_^");
            }
        }

        public static IRole getRole(string name, SocketGuild g)
        {
            foreach (IRole r in g.Roles)
                if (r.Id.ToString() == name) return r;
            

            foreach (IRole r in g.Roles)
                if (r.Name == name) return r;
            

            foreach (IRole r in g.Roles)
                if (r.Name.Contains(name) || name.Contains(r.Name)) return r;
            

            return null;
        }

        public static List<timerInfo> allTimers = new List<timerInfo>();

        public static void timer()
        {
            while(true)
            {
                timerInfo[] copy = allTimers.ToArray();

                foreach (timerInfo t in copy)
                {
                    t.time--;
                    if (t.time <= 0) Console.WriteLine("Removing " + t.user.Username);
                    if (t.time <= 0) allTimers.Remove(t);
                }
                Thread.Sleep(1000);
            }
        }

        public static bool isUserInTimer(SocketUser u)
        {
            foreach(timerInfo t in allTimers)
            {
                if (t.user == u) return true;
            }

            return false;
        }

        public static void giveEXP(SocketUser u, SocketGuild g)
        {
            allTimers.Add(new timerInfo(u, 5));

            string a = "../users/" + u.Id + "/";
            string b = a + g.Id + "/";
            string c = b + "exp.txt";

            if (!Directory.Exists(a)) Directory.CreateDirectory(a);
            if (!Directory.Exists(b)) Directory.CreateDirectory(b);
            if (!File.Exists(c)) File.WriteAllText(c, "0");

            Random rnd = new Random();

            File.WriteAllText(c, (getEXP(u, g) + rnd.Next(5, 16)).ToString());
        }

        public static int getEXP(SocketUser u, SocketGuild g)
        {
            string a = "../users/" + u.Id + "/";
            string b = a + g.Id + "/";
            string c = b + "exp.txt";

            if (!Directory.Exists(a) || !Directory.Exists(b) || !File.Exists(c)) return 0;

            return int.Parse(File.ReadAllText(c));
        }

        public static int getLevel(SocketUser u, SocketGuild g)
        {
            return (int)Math.Floor(Math.Sqrt(Math.Sqrt(getEXP(u, g))));
        }
    }
}
