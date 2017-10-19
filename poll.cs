using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeBot
{
    class poll
    {
        public class pollQuestion
        {
            public SocketMessage lastUserMessage;
            public RestUserMessage lastBotMessage;
            public SocketUser user;
            public string question;
            public List<string> answers = new List<string>();
        }

        public static List<pollQuestion> pollQuestions = new List<pollQuestion>();

        public static pollQuestion getQuestionClass(SocketUser u)
        {
            foreach(pollQuestion pq in pollQuestions) if (pq.user == u) return pq;
            return null;
        }

        public static bool isUserInPQ(SocketUser u)
        {
            foreach (pollQuestion pq in pollQuestions) if (pq.user == u) return true;
            return false;
        }

        public async static void ask(SocketMessage e)
        {
            pollQuestion pq = new pollQuestion();
            pq.user = e.Author;
            pq.lastUserMessage = e;
            pq.question = e.Content.Remove(0, e.Content.Split(' ')[0].Length + 1);
            pq.lastBotMessage = await e.Channel.SendMessageAsync(e.Author.Mention + " k. Now what are the possible answers?");

            pollQuestions.Add(pq);
        }

        public async static void addAnswer(SocketMessage e)
        {
            try { await getQuestionClass(e.Author).lastUserMessage.DeleteAsync(); } catch { }
            await getQuestionClass(e.Author).lastBotMessage.DeleteAsync();

            if (e.Content.ToLower() == "exit")
            {
                displayQNA(e, getQuestionClass(e.Author));
                return;
            }

            getQuestionClass(e.Author).answers.Add(e.Content);

            getQuestionClass(e.Author).lastUserMessage = e;
            getQuestionClass(e.Author).lastBotMessage = await e.Channel.SendMessageAsync(e.Author.Mention + " Added! Type \"exit\" at any point to exit the answer adding :3");
        }

        public async static void displayQNA(SocketMessage e, pollQuestion pq)
        {
            try { await e.DeleteAsync(); } catch { }

            EmbedBuilder eb = new EmbedBuilder();
            eb.Title = pq.question;

            string[] from = { "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮" };

            for (int i = 0; i < pq.answers.Count(); i++)
            {
                eb.Description += from[i] + " " + pq.answers[i] + "\n";
            }

            string[] toParse = ":regional_indicator_a: :regional_indicator_b: :regional_indicator_c: :regional_indicator_d: :regional_indicator_e: :regional_indicator_f: :regional_indicator_g: :regional_indicator_h: :regional_indicator_i:".Split(' ');

            RestUserMessage m = await e.Channel.SendMessageAsync(e.Author.Mention + " asks...", embed: eb.Build());

            pollQuestions.Remove(getQuestionClass(e.Author));

            for (int i = 0; i < pq.answers.Count(); i++)
            {
                await m.AddReactionAsync(new Emoji(from[i]));
            }
        }
    }
}
