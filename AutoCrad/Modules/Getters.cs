using Discord.Commands;
using System;
using System.IO;
using System.Linq;

namespace AutoCrad.Modules
{
    public class Getters : ModuleBase<SocketCommandContext>
    {

        public static int GetNumJokes()
        {
            int numOfJokes = 0;
            string fileName = string.Concat(Environment.CurrentDirectory, (@"\jokes.txt"));
            var reader = File.OpenText(fileName);

            while (reader.ReadLine() != null)
            {
                numOfJokes++;
            }

            return numOfJokes;
        }

        public static string GetJoke(int value)
        {
            string fileName = "jokes.txt";
            string joke = File.ReadLines(fileName).Skip(value).Take(1).First();

            return joke;
        }


        public static string GetMeme()
        {
            var rand = new Random();
            string folder = string.Concat(Environment.CurrentDirectory, (@"\images\memes\"));
            var files = Directory.GetFiles((folder), "*", SearchOption.AllDirectories);

            int ran = rand.Next(files.Length);
            return files[ran];
        }

        public static string Get4chan()
        {
            var rand = new Random();
            string folder = string.Concat(Environment.CurrentDirectory, (@"\images\4chan\"));
            var files = Directory.GetFiles((folder), "*", SearchOption.AllDirectories);

            int ran = rand.Next(files.Length);
            return files[ran];
        }


        public static string GetTime()
        {
            string time = DateTime.Now.ToLongTimeString();
            return time;
        }

        public static string GetDate()
        {

            string date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            return date;
        }

    }
}
