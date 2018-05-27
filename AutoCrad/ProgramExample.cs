using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public async Task StartAsync()
        {
            Console.Title = "AutoCrad";
            Console.WriteLine("Loading AutoCrad..");
            _client = new DiscordSocketClient();
            _handler = new CommandHandler(_client);

            await _client.LoginAsync(TokenType.Bot, "YOUR_BOT_TOKEN_HERE");
            await _client.StartAsync();

            Console.WriteLine("Startup complete, ready to use!");
            string currentTime = DateTime.Now.ToString();
            Console.WriteLine("CURRENT TIME:\t" + currentTime);

            int directoryCount = System.IO.Directory.GetDirectories(@"YOUR_SERVER_LOG_DIRECTORY_HERE").Length;
            string game = "| .help | " + directoryCount + " Servers";

            await _client.SetGameAsync(game);
            Console.WriteLine("INFO:\t" + directoryCount + " Servers");
            await Task.Delay(-1);
        }
    }
}