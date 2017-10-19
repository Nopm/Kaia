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
    class mute
    {
        public static void execute(SocketMessage e)
        {
            if (e.Content.Split(' ').Length <= 1)
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " u has 2 do \"" + Program.getPrefix(e) + "mute @user [minutes]\"");
                return;
            }

            if (!((SocketGuildUser)e.Author).GetPermissions((IGuildChannel)e.Channel).Has(Discord.ChannelPermission.ManageMessages))
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " No permmies >:C");
                return;
            }

            SocketUser toMute = getUser(e, e.Content.Split(' ')[1]);
            
            if (toMute == null)
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " I can't find that user! D:");
                return;
            }

            if (!Directory.Exists("../users/" + toMute.Id + "/")) Directory.CreateDirectory("../users/" + toMute.Id + "/");
            if (!Directory.Exists("../users/" + toMute.Id + "/mutes/")) Directory.CreateDirectory("../users/" + toMute.Id + "/mutes/");

            int time = 600;

            bool custom = false;

            if (e.Content.Split(' ').Length == 3)
            {
                try
                {
                    time = int.Parse(e.Content.Split(' ')[2]);
                    time *= 60;
                    custom = true;
                }
                catch
                {
                    Console.WriteLine("broken at: " + e.Content.Split(' ')[2]);
                }
            }

            setMute(e, custom, time, toMute);
        }

        public static void unmute(SocketMessage e)
        {
            if (e.Content.Split(' ').Length <= 1)
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " u has 2 do \"" + Program.getPrefix(e) + "unmute @user\"");
                return;
            }

            if (!((SocketGuildUser)e.Author).GetPermissions((IGuildChannel)e.Channel).Has(Discord.ChannelPermission.ManageMessages))
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " No permmies >:C");
                return;
            }

            SocketUser toMute = getUser(e, e.Content.Split(' ')[1]);

            if (toMute == null)
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " I can't find that user! D:");
                return;
            }

            string f = "../active/mutes/" + toMute.Id + ".txt"; //todo: add multi-server support

            if (File.Exists(f))
            {
                timers.unmute(((SocketGuildChannel)e.Channel).Guild, f, (SocketGuildUser)toMute);
                e.Channel.SendMessageAsync(e.Author.Mention + " unmuuuuuted");
            }
            else
            {
                e.Channel.SendMessageAsync(e.Author.Mention + " this guy ain't muted yo");
            }
        }

        public static void setMute(SocketMessage e, bool custom, int time, SocketUser toMute, bool tell = true)
        {
            if (!custom)
            {
                for (int i = 0; i < Directory.GetFiles("../users/" + toMute.Id + "/mutes/").Length; i++)
                {
                    time = (int)(time * 1.5);
                }
            }

            File.WriteAllText("../users/" + toMute.Id + "/mutes/" + e.Id + ".txt", getCurrentUnixTime().ToString());

            ((SocketGuildUser)toMute).AddRoleAsync(getRole(((SocketGuildChannel)e.Channel).Guild, "Muted"));

            string readableTimeSig = "seconds";

            int originTime = time;

            Console.WriteLine("Original time: " + time);

            if (time > 60)
            {
                time = (int)Math.Round(time / 60d);
                readableTimeSig = "minutes";
                if ((int)Math.Round((double)time) == 1d) readableTimeSig = "minute";

                if (time > 60)
                {
                    time = (int)Math.Round(time / 60d);
                    readableTimeSig = "hours";
                    if ((int)Math.Round((double)time) == 1d) readableTimeSig = "hour";

                    if (time > 24)
                    {
                        time = (int)Math.Round(time / 24d);
                        readableTimeSig = "days";
                        if ((int)Math.Round((double)time) == 1d) readableTimeSig = "day";
                    }
                }
            }

            SocketGuild g = ((SocketGuildChannel)((SocketChannel)e.Channel)).Guild;

            File.WriteAllText("../active/mutes/" + toMute.Id + ".txt", (getCurrentUnixTime() + (ulong)originTime).ToString() + "," + g.Id);

            if (tell) e.Channel.SendMessageAsync(e.Author.Mention + " " + toMute.Mention + " hath been muted for " + time + " " + readableTimeSig + "! >:3");
        }

        public static ulong getCurrentUnixTime()
        {
            return (ulong)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static IRole getRole(SocketGuild e, string name)
        {
            foreach (SocketRole r in (e.Roles))
            {
                if (r.Id.ToString() == name) return r;
                if (r.Name.ToLower().Contains(name.ToLower())) return r;
                if (name.ToLower().Contains(r.Name.ToLower())) return r;
            }

            return null;
        }

        public static SocketUser getUser(SocketMessage e, string name, bool global = false)
        {
            {
                name = name.Replace("@", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("!", "");



                foreach (SocketUser u in ((SocketGuildChannel)e.Channel).Guild.Users)
                {
                    if (u.Id.ToString() == name.ToLower())
                        return u;

                    if (((SocketGuildUser)u).Nickname != null && ((SocketGuildUser)u).Nickname.ToLower().Contains(name.ToLower()))
                        return u;

                    if (((SocketGuildUser)u).Nickname != null && name.ToLower().Contains(((SocketGuildUser)u).Nickname.ToLower()))
                        return u;

                    if (u.Username.ToLower().Contains(name.ToLower()))
                        return u;

                    if (name.ToLower().Contains(u.Username.ToLower()))
                        return u;
                }
                

                return null;
            }
        }
    }
}
