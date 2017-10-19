using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using Discord.Commands;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Reflection.Emit;
using Newtonsoft.Json.Linq;

namespace AnimeBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public static bool nya = bool.Parse(System.IO.File.ReadAllText("../nya.txt"));
        public static string game = System.IO.File.ReadAllText("../game.txt");

        public static DiscordSocketClient bot;
        public static JsonSerializer JCon = new JsonSerializer();

        private readonly IServiceCollection map = new ServiceCollection();
        private readonly CommandService commands = new CommandService();


        private Program()
        {
            filter.initCurses();


            bot = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Critical,
                MessageCacheSize = 50,
                WebSocketProvider = WS4NetProvider.Instance
            });

            bot.Log += Logger;
            commands.Log += Logger;
        }

        private Task Logger(LogMessage e)
        {
            Console.WriteLine(e.Exception);
            return Task.CompletedTask;
        }



        private async Task MainAsync()
        {
            string token;
            using (StreamReader sr = new StreamReader("../config.json"))
            using (JsonReader jr = new JsonTextReader(sr))
            {
               var jsonObject = JCon.Deserialize(jr);
               var deserialized = (JObject)jsonObject;
               token = deserialized.SelectToken("['Token']").ToString();
            }
                await initCommands();

            await bot.LoginAsync(TokenType.Bot, token);

            await bot.StartAsync();

            //Thread t = new Thread(() => setPlaying.start());
            //t.Start();

            Thread q = new Thread(() => timers.nyaaTimer());
            q.Start();

            Thread w = new Thread(() => timers.muteTimer());
            w.Start();

            Thread e = new Thread(() => timers.setgameTimer());
            e.Start();

            Thread r = new Thread(() => levels.timer());
            r.Start();

            Console.WriteLine("Ready to go!");

            await Task.Delay(-1);
        }

        private IServiceProvider serviceProvider;

        private async Task initCommands()
        {
            //this is stupid

            //map.AddSingleton(new SomeServiceClass());
            serviceProvider = map.BuildServiceProvider();

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            //await commands.AddModuleAsync<SomeModule>;

            bot.MessageReceived += Bot_MessageReceived;
        }

        private Task Bot_MessageReceived(SocketMessage e)
        {
            Thread thread = new Thread(() => handleMessages(e));
            thread.Start();

            return Task.CompletedTask;
        }

        public static string getPrefix(SocketMessage e)
        {
            return ".";
        }

        public static string getDevPrefix(SocketMessage e)
        {
            return ";";
        }

        public async static void handleMessages(SocketMessage e)
        {
            if (e.Author.Id.ToString() == "366380540666052609" || e.Author.Id.ToString() == "366782421116649502") return;

            //check if user is not mod AND cursed
            if (!filter.isUserMod((SocketGuildUser)e.Author, (SocketChannel)e.Channel)) if (!filter.check(e)) return;


            //automated responses
            if (e.Content.ToLower().Contains("nya"))
            {
                if (nya)
                {
                    if (timers.nyaaTime <= 0)
                    {
                        Random rnd = new Random();
                        if (rnd.Next(0, 101) <= 35)
                        {
                            if (e.Content.ToLower().Contains("kaia") || e.Content.Contains("366380540666052609"))
                            {
                                await e.Channel.SendMessageAsync("Nya~");
                                timers.nyaaTime = 600;
                            }
                        }
                    }
                }
            }

            //checking
            if (poll.isUserInPQ(e.Author))
                poll.addAnswer(e);

            //commands
            if (e.Content.ToLower().StartsWith(getPrefix(e) + "anime "))
                anime.search(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "cute"))
                cute.check(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "love "))
                cute.love(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "poll "))
                poll.ask(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "iam "))
                iam.addiam(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "iamn "))
                iam.removeiam(e);


            //moderator commands
            if (e.Content.ToLower().StartsWith(getPrefix(e) + "mute "))
                mute.execute(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "unmute "))
                mute.unmute(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "filter add "))
                filter.addCurse(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "filter remove "))
                filter.removeCurse(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "ban "))
                admin.ban(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "kick "))
                admin.kick(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "addiam "))
                admin.addiam(e);

            if (e.Content.ToLower().StartsWith(getPrefix(e) + "removeiam "))
                admin.removeiam(e);


            //developer commands
            if (e.Content.ToLower().StartsWith(getDevPrefix(e) + "nya"))
                dev.switchNya(e);

            if (e.Content.ToLower().StartsWith(getDevPrefix(e) + "setgame "))
                dev.setGame(e);

            if (e.Content.ToLower().StartsWith(getDevPrefix(e) + "startinit"))
                await messages.startInit(e);

            levels.checkEXP(e);
        }
    }
}
