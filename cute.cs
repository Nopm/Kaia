using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Drawing;
using System.Net;
using System.IO;

namespace AnimeBot
{
    class cute
    {
        public static double[] getRGBofProfilePicture(SocketUser u)
        {
            //[0] = red
            //[1] = green
            //[2] = blue

            Bitmap bmp = new Bitmap(1, 1);

            try
            {
                WebRequest request = WebRequest.Create(
                    u.GetAvatarUrl());


                WebResponse response = request.GetResponse();
                Stream responseStream =
                    response.GetResponseStream();
                bmp = new Bitmap(responseStream);
            }
            catch
            {
                try
                {
                    int na = u.Id.ToString().ToCharArray()[0];
                    int nb = u.Id.ToString().ToCharArray()[1];
                    int nc = u.Id.ToString().ToCharArray()[2];

                    double[] d = new double[3];
                    d[0] = na / 10;
                    d[1] = nb / 10;
                    d[2] = nc / 10;

                    return d;
                }
                catch
                {
                    Random rnd = new Random();

                    double[] d = new double[3];
                    d[0] = rnd.NextDouble();
                    d[1] = rnd.NextDouble();
                    d[2] = rnd.NextDouble();
                    return d;
                }
            }

            List<double> rList = new List<double>();
            List<double> gList = new List<double>();
            List<double> bList = new List<double>();

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y).R > 0 &&
                        bmp.GetPixel(x, y).G > 0 &&
                        bmp.GetPixel(x, y).B > 0)
                    {
                        rList.Add((double)bmp.GetPixel(x, y).R / 255d);
                        gList.Add((double)bmp.GetPixel(x, y).G / 255d);
                        bList.Add((double)bmp.GetPixel(x, y).B / 255d);
                    }
                }
            }

            double r = 0;
            double g = 0;
            double b = 0;

            Console.WriteLine("r: " + r);


            foreach (double i in rList) r += i;
            foreach (double i in gList) g += i;
            foreach (double i in bList) b += i;

            Console.WriteLine("r: " + r);

            r /= rList.Count();
            g /= gList.Count();
            b /= bList.Count();

            Console.WriteLine("r: " + r);
            Console.WriteLine("g: " + g);
            Console.WriteLine("b: " + b);

            double[] ir = { r, g, b };

            return ir;
        }

        public static SocketUser getUserFromSegmentString(string s, SocketGuild g)
        {
            s = s.ToLower();
            s = s.Replace("<", "")
                .Replace(">", "")
                .Replace("@", "")
                .Replace("!", "");

            for (int i = 0; i < s.Length; i++)
            {
                foreach (SocketUser u in g.Users)
                {
                    if (u.Id.ToString() == s) return u;
                    if (u.Username.ToLower().Contains(s)) return u;
                    if (s.ToLower().Contains(u.Username.ToLower())) return u;

                    if (((SocketGuildUser)u).Nickname != null)
                    {
                        if (((SocketGuildUser)u).Nickname.ToLower().Contains(s)) return u;
                        if (s.ToLower().Contains(((SocketGuildUser)u).Nickname.ToLower())) return u;
                    }
                }

                s = s.Remove(s.Length - 1, 1);
            }

            Random rnd = new Random();

            return null;
        }


        public static void check(SocketMessage e)
        {
            IDisposable typing = e.Channel.EnterTypingState();

            Random rnd = new Random();

            string output = "";

            double[] rgb = new double[3];

            if (!e.Content.StartsWith(Program.getPrefix(e) + "cute ")) rgb = getRGBofProfilePicture(e.Author);
            else rgb = getRGBofProfilePicture(getUserFromSegmentString(e.Content.Remove(0, (Program.getPrefix(e) + "cute ").Length), ((SocketGuildChannel)e.Channel).Guild));

            double r = Math.Abs(rgb[0] - 1);
            double g = Math.Abs(rgb[1] - 0.6);
            double b = Math.Abs(rgb[2] - 0.8);

            r = 1 - r;
            g = 1 - g;
            b = 1 - b;

            double p = r + g + b;
            p /= 3;
            p = Math.Round(p * 100);

            Console.WriteLine("p: " + p);

            string name = e.Author.Mention;
            if (e.Content.Split(' ').Length > 1) name = e.Content.Remove(0, e.Content.Split(' ')[0].Length + 1);
            if (name.Contains("everyone") || name.Contains("here")) name = name.Replace("@", "");

            if (p <= 5) output = "Um.";
            else if (p <= 25) output = "I think " + name + " and I should just be friends. (" + p + "%)";
            else if (p <= 50) output = "I'd rate " + name + "... maybe you shouldn't be asking me to rate them. (" + p + "%)";
            else if (p <= 75) output = "Hmm... I'd rate " + name + " " + p + "% cute";
            else if (p <= 90) output = "I would snuggle " + name + " so hard. (" + p + "%)";
            else if (p <= 100) output = "Can I date you, " + name + "? (" + p + "%)";

            if (name.ToLower().Contains("87734350556196864") || name.ToLower().Contains("swash"))
                output = "I- I don't... like Swash th- that much! >///>";

            if (name.ToLower().Contains("366380540666052609") || name.ToLower().Contains("kaia"))
                output = "Nobody's cuter :3";

            if (name.ToLower().Contains("130769947583447040") || name.ToLower().Contains("quantum"))
                output = "Only the cutest ;3";

            e.Channel.SendMessageAsync(e.Author.Mention + " " + output);

            typing.Dispose();
        }

        public static void love(SocketMessage e)
        {
            IDisposable typing = e.Channel.EnterTypingState();


            Random rnd = new Random();

            string output = "";

            string firstName = e.Author.Mention;
            string secondName = e.Author.Mention;

            if (e.Content.Split(' ').Length == 2)
                secondName = e.Content.Split(' ')[1];
            else
            {
                firstName = e.Content.Split(' ')[1];
                secondName = e.Content.Split(' ')[2];
            }

            double[] rgb = getRGBofProfilePicture(getUserFromSegmentString(firstName, ((SocketGuildChannel)e.Channel).Guild));
            double[] secondRGB = getRGBofProfilePicture(getUserFromSegmentString(secondName, ((SocketGuildChannel)e.Channel).Guild));

            double diff1 = Math.Abs(rgb[0] - secondRGB[0]);
            double diff2 = Math.Abs(rgb[1] - secondRGB[1]);
            double diff3 = Math.Abs(rgb[2] - secondRGB[2]);

            double difference = (diff1 + diff2 + diff3) / 3;

            double p = Math.Round(difference * 100);

            Console.WriteLine("p: " + p);

            if (getUserFromSegmentString(firstName, ((SocketGuildChannel)e.Channel).Guild).Id.ToString() == "130769947583447040" &&
                getUserFromSegmentString(secondName, ((SocketGuildChannel)e.Channel).Guild).Id.ToString() == "87734350556196864")
                output = "I- i think th... I THINK THESE TWO SHOULD DATE! >\\\\\\\\\\\\\\\\\\\\\\\\\\\\>";
            else if (p <= 5) output = "no";
            else if (p <= 10) output = "Erm... no.";
            else if (p <= 25) output = "I think " + firstName + " and " + secondName + " should just be friends. (" + p + "%)";
            else if (p <= 50) output = "I... don't think these two are very compatible. (" + p + "%)";
            else if (p <= 75) output = "Hmm... I'd rate " + firstName + " and " + secondName + "'s love " + p + "% cute.";
            else if (p <= 90) output = "Hmm... I'd rate " + firstName + " and " + secondName + "'s love " + p + "% adorable.";
            else if (p <= 100) output = firstName + " and " + secondName + " should totes date! ^w^ (" + p + "%)";

            if (!output.Contains(e.Author.Mention)) e.Channel.SendMessageAsync(e.Author.Mention + " " + output);
            else e.Channel.SendMessageAsync(output);

            typing.Dispose();
        }
    }
}
