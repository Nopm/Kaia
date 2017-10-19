using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeBot
{
    class dev
    {
        public static bool isUserDev(SocketUser u)
        {
            if (u.Id.ToString() == "87734350556196864" || u.Id.ToString() == "130769947583447040") return true;
            return false;
        }


        public static void switchNya(SocketMessage e)
        {
            if (!isUserDev(e.Author)) return;

            Program.nya = !Program.nya;

            string to = "disabled :C";
            if (Program.nya) to = "enabled :D";

            File.WriteAllText("../nya.txt", Program.nya.ToString());

            e.Channel.SendMessageAsync(e.Author.Mention + " Nya has been " + to);
        }

        public static void setGame(SocketMessage e)
        {
            if (!isUserDev(e.Author)) return;

            string to = e.Content.Remove(0, e.Content.Split(' ')[0].Length + 1);

            File.WriteAllText("../game.txt", to);

            Program.game = to;
            e.Channel.SendMessageAsync(e.Author.Mention + " Done! ^_^");
        }
    }
}
