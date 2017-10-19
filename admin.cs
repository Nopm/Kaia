using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.IO;

namespace AnimeBot
{
    class admin
    {
        public async static void ban(SocketMessage e)
        {
            if (filter.isUserMod((SocketGuildUser)e.Author, (SocketGuildChannel)e.Channel))
            {
                SocketUser u = mute.getUser(e, e.Content.Split(' ')[1]);

                if (e.Content.Length > (e.Content.Split(' ')[0] + " " + e.Content.Split(' ')[1]).Length)
                {
                    string message = e.Content.Remove(0, (e.Content.Split(' ')[0] + " " + e.Content.Split(' ')[1] + " ").Length);
                    IDMChannel c = await u.GetOrCreateDMChannelAsync();
                    await c.SendMessageAsync(message);
                }

                await ((SocketGuildChannel)e.Channel).Guild.AddBanAsync(u);
                await e.Channel.SendMessageAsync("Banned " + u.Mention + " >:3");

            }
            else
            {
                await e.Channel.SendMessageAsync("NO PERMMIES >:C");
            }
        }

        public async static void kick(SocketMessage e)
        {
            if (filter.isUserMod((SocketGuildUser)e.Author, (SocketGuildChannel)e.Channel))
            {
                SocketUser u = mute.getUser(e, e.Content.Split(' ')[1]);

                if (e.Content.Length > (e.Content.Split(' ')[0] + " " + e.Content.Split(' ')[1]).Length)
                {
                    string message = e.Content.Remove(0, (e.Content.Split(' ')[0] + " " + e.Content.Split(' ')[1] + " ").Length);
                    IDMChannel c = await u.GetOrCreateDMChannelAsync();
                    await c.SendMessageAsync(message);
                }

                await ((SocketGuildUser)u).KickAsync();
                await e.Channel.SendMessageAsync("Kicked " + u.Mention + " >:3");
            }
            else
            {
                await e.Channel.SendMessageAsync("NO PERMMIES >:C");
            }
        }

        public static SocketRole getRole(SocketGuild g, string s)
        {
            foreach(SocketRole r in g.Roles)
            {
                if (r.Id.ToString() == s) return r;
                if (r.Name == s) return r;
                if (s.Contains(r.Name)) return r;
                if (r.Name.Contains(s)) return r;
            }

            return null;
        }

        public static void addiam(SocketMessage e)
        {
            if (filter.isUserMod((SocketGuildUser)e.Author, (SocketGuildChannel)e.Channel))
            {
                SocketRole r = getRole(((SocketGuildChannel)e.Channel).Guild, e.Content.Split(' ')[1]);

                if (r == null)
                {
                    e.Channel.SendMessageAsync(e.Author.Mention + " I couldn't find that role! D;");
                    return;
                }

                if (e.Content.Split(' ').Length < 3)
                {
                    e.Channel.SendMessageAsync(e.Author.Mention + " Correct usage is " + Program.getPrefix(e) + "addiam [role] [level] >:C");
                    return;
                }

                string d = "../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/";
                string c = d + "iam.txt";

                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                if (!File.Exists(c)) File.Create(c);

                File.AppendAllText(c, r.Id + " " + e.Content.Split(' ')[2] + "\n");

                e.Channel.SendMessageAsync(e.Author.Id + " I added " + r.Name + " to iam! :3");
            }
            else
            {
                e.Channel.SendMessageAsync("NO PERMMIES >:C");
            }
        }

        public static void removeiam(SocketMessage e)
        {
            if (filter.isUserMod((SocketGuildUser)e.Author, (SocketGuildChannel)e.Channel))
            {
                SocketRole r = getRole(((SocketGuildChannel)e.Channel).Guild, e.Content.Split(' ')[1]);

                if (r == null)
                {
                    e.Channel.SendMessageAsync(e.Author.Mention + " I couldn't find that role! D;");
                    return;
                }

                string[] f = File.ReadAllLines("../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/iam.txt");

                string rec = "";

                for(int i = 0; i < f.Length; i++)
                {
                    if (!f[i].StartsWith(r.Id.ToString()))
                    {
                        rec += f[i] + "\n";
                    }
                }

                File.WriteAllText("../servers/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/iam.txt", rec);

                e.Channel.SendMessageAsync(e.Author.Id + " I remove " + r.Name + " from iam! :3");
            }
            else
            {
                e.Channel.SendMessageAsync("NO PERMMIES >:C");
            }
        }
    }
}
