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
    class timers
    {
        public static int nyaaTime = 0;


        public static void nyaaTimer()
        {
            while (true)
            {
                nyaaTime--;
                Thread.Sleep(1000);
            }
        }


        //public static void pfpChangeTimer()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(21600000);
        //        Program.bot.
        //    }
        //}


        public static void setgameTimer()
        {
            while (true)
            {
                Program.bot.SetGameAsync(Program.game);
                Thread.Sleep(600000);
            }
        }

        public static void muteTimer()
        {
            while(true)
            {
                foreach (string f in Directory.GetFiles("../active/mutes/"))
                {
                    if (!f.Contains("please note") && ulong.Parse(File.ReadAllText(f).Split(',')[0]) >= mute.getCurrentUnixTime())
                    {
                        SocketGuild g = Program.bot.GetGuild(ulong.Parse(File.ReadAllText(f).Split(',')[1]));

                        foreach(SocketGuildUser u in g.Users)
                        {
                            if (u.Id.ToString() == f.Split('/').Last().Split('\\').Last().Replace(".txt", ""))
                            {
                                unmute(g, f, u);
                                Console.WriteLine("Unmuted " + u.Username);
                            }
                        }
                    }
                }

                Thread.Sleep(10000); //check every 10sec

            }
        }

        public static void unmute(SocketGuild g, string f, SocketGuildUser u)
        {
            u.RemoveRoleAsync(mute.getRole(g, "Muted"));
            File.Delete(f);
        }
    }
}
