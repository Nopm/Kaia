using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Discord.Rest;

namespace AnimeBot
{
    class messages
    {
        public async static Task startInit(SocketMessage e)
        {
            await e.Channel.SendMessageAsync("Starting startInit (test version).....");

            var dsClient = Program.bot;

            foreach (SocketChannel c in ((SocketGuildChannel)e.Channel).Guild.Channels)
            {
                try
                {
                    var channel = (dsClient as IDiscordClient).GetChannelAsync(c.Id).Result;
                    var msgs = (channel as IMessageChannel).GetMessagesAsync().Flatten().Result;
                    await Task.Yield();

                    while (true)
                    {
                        var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result; //error here
                        msgs = msgs.Concat(newmsgs);
                        if (newmsgs.Count() < 100)
                        {
                            Console.WriteLine("Done with channel.");
                            break;
                        }
                        
                        foreach (RestUserMessage m in msgs.ToArray()) //error here
                        {
                            markMessage(m);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("error: " + ex.ToString());
                }
            }
        }

        public static void markMessage(RestUserMessage e)
        {
            string d = ("../messages/" + ((SocketGuildChannel)e.Channel).Guild.Id + "/");
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);
            if (!Directory.Exists(d + "/" + e.Author.Id + "/")) Directory.CreateDirectory(d + "/" + e.Author.Id + "/");
            File.WriteAllText((d + "/" + e.Author.Id + "/" + e.Id + ".txt"), dateToUnix(e.CreatedAt) + "\n" + e.Channel.Id + "\n" + e.Content);
        }

        public static ulong dateToUnix(DateTimeOffset d)
        {
            return (ulong)d.ToUnixTimeSeconds();
        }
    }
}
