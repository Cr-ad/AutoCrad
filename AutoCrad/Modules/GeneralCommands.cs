using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoCrad.Modules;

namespace DiscordBot.Modules
{
    public class Commands : AutoCrad.Modules.Getters
    {

        /// <summary>
        /// Displays the commands available to users
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("help")]
        public async Task Help([Remainder] string userInput = null)
        {
            var generalHelp = new EmbedBuilder();
            generalHelp.WithTitle("AutoCrad Commands");
            generalHelp.WithDescription("General:");
            generalHelp.AddInlineField("Help", "Shows this command");
            generalHelp.AddInlineField("Meme", "Shows a dank meme");
            generalHelp.AddInlineField("4chan", "Shows a 4chan post");
            generalHelp.AddInlineField("Joke", "Shows a joke");
            generalHelp.AddInlineField("8ball (question)", "Ask Magic 8 ball anything");
            generalHelp.AddInlineField("Coin", "Flips a coin");
            generalHelp.AddInlineField("Dice", "Rolls a dice");
            generalHelp.AddInlineField("French", "'Translates' to French");
            generalHelp.AddInlineField("Coin", "Flips a coin");
            generalHelp.AddInlineField("Wed", "It's Wednesday my dudes");
            generalHelp.AddInlineField("City", "England is my city");
            generalHelp.AddInlineField("Invite", "Add AutoCrad to a server");
            generalHelp.AddInlineField("Suggest", "Suggest a feature for AutoCrad");
            generalHelp.WithFooter("Page 1/2");

            generalHelp.WithThumbnailUrl("https://i.imgur.com/AecH2Ym.jpg");
            generalHelp.WithColor(Color.Blue);

            var adminHelp = new EmbedBuilder();
            adminHelp.WithTitle("AutoCrad Commands");
            adminHelp.WithDescription("Admin:");
            adminHelp.AddInlineField("Clear <x>", "Clears x messages");
            adminHelp.AddInlineField("FindJoke <x>", "Find corresponding joke");
            adminHelp.AddInlineField("Mute <user>", "Mutes the selected user");
            adminHelp.AddInlineField("Ban <user>", "Bans the selected user");
            adminHelp.WithFooter("Page 2/2");

            adminHelp.WithThumbnailUrl("https://i.imgur.com/AecH2Ym.jpg");
            adminHelp.WithColor(Color.Red);

            switch (userInput)
            {
                case "1":
                    await Context.Channel.SendMessageAsync("", false, generalHelp);
                    break;
                case "2":
                    await Context.Channel.SendMessageAsync("", false, adminHelp);
                    break;
                default:
                    await Context.Channel.SendMessageAsync("", false, generalHelp);
                    await Context.Channel.SendMessageAsync("", false, adminHelp);
                    break;
            }          

            string input = userInput;
            string method = "Help";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Sends a random joke to the discord chat
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("joke")]
        public async Task Joke([Remainder] string userInput = null)
        {
            int numOfJokes = GetNumJokes();

            Random rand = new Random();
            int randomJoke = rand.Next(0, (numOfJokes - 1));

            string joke = GetJoke(randomJoke);
            await Context.Channel.SendMessageAsync(joke);

            string input = randomJoke.ToString();
            string method = "Joke";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Shows the 'its wednesday my dudes' meme
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("wed")]
        [Alias("wednesday")]
        public async Task Wednesday([Remainder] string userInput = null)
        {
            await Context.Channel.SendMessageAsync("***It is Wednesday,***\n" + "***my dudes***\n");
            string fileName = string.Concat(Environment.CurrentDirectory, (@"\images\individual\wed.jpg"));
            await Context.Channel.SendFileAsync(fileName);
            string input = "";
            string method = "Wednesday";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Shows the 'england is my city' meme
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("city")]
        public async Task City([Remainder] string userInput = null)
        {
            await Context.Channel.SendMessageAsync("**Pochinki is my city**");
            string fileName = string.Concat(Environment.CurrentDirectory, (@"\images\individual\city.png"));
            await Context.Channel.SendFileAsync(fileName);

            string input = "";
            string method = "City";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Sends a random meme to the discord chat
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("meme")]
        public async Task Meme([Remainder] string userInput = null)
        {
            string user = Context.User.Mention;
            string fileName = GetMeme();

            await Context.Channel.SendMessageAsync("" + user + "***, here's your dank meme*** 🔥");
            await Context.Channel.SendFileAsync(fileName);

            string input = fileName;
            string method = "Meme";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Sens a random 4chan post screenshot to the discord chat
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("4chan")]
        [Alias("greentext")]
        public async Task FourChan([Remainder] string userInput = null)
        {
            string user = Context.User.Mention;
            string fileName = Get4chan();

            await Context.Channel.SendMessageAsync("" + user + "***, here's your 4chan post*** 👻");
            await Context.Channel.SendFileAsync(fileName);

            string input = fileName;
            string method = "FourChan";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Responds to a user question, as if it was a 'magic 8 ball'
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("8ball")]
        public async Task EightBall([Remainder] string userInput = null)
        {
            string user = Context.User.Mention;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ArgumentException(user + " 🎱 Please enter a question to ask 8ball" + "\n **Command Usage: **.8ball <question> ");
            }
            
            Random rand = new Random();
            int value = rand.Next(0, 6);

            string response = "";
            switch (value)
            {
                case 0:
                    response = "Yeah";
                    break;
                case 1:
                    response = "Nah, I don't think so";
                    break;
                case 2:
                    response = "Probably lol";
                    break;
                case 3:
                    response = "100% dude";
                    break;
                case 4:
                    response = "Nope";
                    break;
                case 5:
                    response = "lmao, why??";
                    break;
                case 6:
                    response = "Absolutely not";
                    break;
            }

            string title = "🎱 Magic 8 Ball 🎱";
            string description = $"**Question:** {userInput}\n**Asked by: **{user}\n**Answer:** {response}";

            embedThis(title, description);

            string input = userInput;
            string method = "EightBall";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// 'translates' the user input to french by adding 'le' infront of it
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("translate")]
        [Alias("french")]
        public async Task Translate([Remainder] string userInput = null)
        {
            string user = Context.User.Mention;
            if (userInput == null)
            {
                await Context.Channel.SendMessageAsync("**" + user + "** 🇫🇷 The French translation is: '**le you didn't type anything..**' 👌");
            }
            else if ((userInput[0] == 'l' && userInput[1] == 'e') && userInput[2] == ' ')
            {
                await ReplyAsync("**" + user + "**, that's clearly already in French.. 🤔");
            }
            else
            {
                await Context.Channel.SendMessageAsync("**" + user + "** 🇫🇷 The French translation is: '**le " + userInput + "**'");
            }

            string input = userInput;
            string method = "Translate";
            LogCommand(GetDate(), GetTime(), user, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Sends a discord invite link to the chat so a user can add the bot to their server
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("invite")]
        public async Task Invite([Remainder] string userInput = null)
        {
            string title = "👾 AutoCrad Invite 👾";
            string link = "https://discordbots.org/bot/351821932544393218";
            
            string description = "";
            string chooseColor = "gold";
            string user = Context.User.Mention;
            embedThis(title, description, chooseColor);
            await Context.Channel.SendMessageAsync(user + " " + link);

            string input = "";
            string method = "Invite";
            LogCommand(GetDate(), GetTime(), user, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Allows the user to provide a suggestion of a feature or change for the bot
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("suggest")]
        [Alias("suggestion")]
        public async Task Suggest([Remainder] string userInput = null)
        {
            string user = Context.User.Mention;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ArgumentException(user + ", please enter a suggestion"+ "\n **Command Usage: **.suggest <suggestion> ");
            }

            string serverName = Context.Guild.Id.ToString();

            string suggestion = @"""" + userInput + @"""";
            string output = GetDate() + "," + GetTime() + "," + Context.User.Username + "," + suggestion;
            string path = string.Concat(Environment.CurrentDirectory, @"\Logs\Servers\");
            System.IO.Directory.CreateDirectory(path + serverName);
            string fileName = string.Concat(path, serverName) + @"\suggestions.csv";

            if (!suggestion.Equals(""))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + ", thanks for your suggestion!");
                if (!File.Exists(fileName))
                {
                    File.WriteAllText(fileName, "Date,Time,User,Suggestion" + Environment.NewLine);
                    File.AppendAllText(fileName, output + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(fileName, output + "" + Environment.NewLine);
                }
            }
            suggestion = userInput;
            string method = "Suggest";
            ToConsole(method, userInput);
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, userInput);
        }

        /// <summary>
        /// Simulates a dice roll and sends to the discord chat
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("roll")]
        [Alias("dice")]
        public async Task Roll([Remainder] string userInput = null)
        {
            await Context.Channel.SendMessageAsync("*Rolling the dice...* 🎲");

            Random rand = new Random();

            int dice = rand.Next(1, 7);

            Thread.Sleep(1000);
            string user = Context.User.Mention;
            await Context.Channel.SendMessageAsync(user + "*, the dice rolled a " + dice + "*");

            string method = "Roll";
            ToConsole(method, dice.ToString());
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, dice.ToString());
        }

        /// <summary>
        /// Simulates a coin flip and sends it to the discord chat
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [Command("flip")]
        [Alias("coin")]
        public async Task Flip([Remainder] string userInput = null)
        {
            await Context.Channel.SendMessageAsync("Flipping a coin... ⚖️");
            string coin = "";

            Random rand = new Random();
            int result = rand.Next(0, 2);
            string user = Context.User.Mention;

            Thread.Sleep(1000);
            if (result == 0)
            {
                await Context.Channel.SendMessageAsync(user + ", it's tails!");
                coin = "heads";
            }
            else
            {
                await Context.Channel.SendMessageAsync(user + ", it's heads!");
                coin = "tails";
            }

            string method = "Flip";
            ToConsole(method, coin);
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, coin);

        }

        /// <summary>
        /// Creates an embeded message to display on Discord using inputs provided
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="chooseColor"></param>
        public void embedThis(string title = null, string description = null, string chooseColor = "")
        {
            var toEmebed = new EmbedBuilder();
            toEmebed.WithTitle(title);
            toEmebed.WithDescription(description);
            chooseColor.ToLower();
            switch (chooseColor)
            {
                case "red":
                    toEmebed.WithColor(Color.DarkRed);
                    break;
                case "blue":
                    toEmebed.WithColor(Color.Blue);
                    break;
                case "gold":
                    toEmebed.WithColor(Color.Gold);
                    break;
                default:
                    toEmebed.WithColor(Color.Green);
                    break;
            }

            Context.Channel.SendMessageAsync("", false, toEmebed);
        }

        /// <summary>
        /// Logs the command to the console
        /// </summary>
        /// <param name="method"></param>
        /// <param name="input"></param>
        public void ToConsole(string method, string input)
        {
            Console.WriteLine(GetDate() + " " + GetTime() + "\t" + Context.Guild.ToString() + "\t" + Context.User.Username + "\t" + method + "\t" + @"""" + input + @"""");
        }

        /// <summary>
        /// Logs the commands run into individual server folders in a csv file
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="user"></param>
        /// <param name="command"></param>
        /// <param name="input"></param>
        private void LogCommand(string date, string time, string user, string command, string input)
        {
            string serverName = Context.Guild.Id.ToString();

            string path = string.Concat(Environment.CurrentDirectory, @"\Logs\Servers\");
            System.IO.Directory.CreateDirectory(path + serverName);
            string fileName = string.Concat(path, serverName) + @"\commands.csv";

            string output = date + "," + time + "," + user + "," + command + "," + @"""" + input + @"""";

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "Date,Time,User,Command,Input" + Environment.NewLine);
                File.AppendAllText(fileName, output + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(fileName, output + "" + Environment.NewLine);
            }

        }
    }
}