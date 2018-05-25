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
    public class AdminCommands : AutoCrad.Modules.Getters
    {
        /// <summary>
        /// Select a joke via its joke number
        /// </summary>
        /// <param name="jokeNumber"></param>
        /// <returns></returns>
        [Command("findJoke")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task FindJoke(int jokeNumber = -1)
        {
            int numOfJokes = GetNumJokes();
            if (jokeNumber <= 0)
            {
                throw new ArgumentException(Context.User.Mention + ", please enter a number between 1 and " + numOfJokes + "\n **Command Usage: **.findJoke <number> ");
            }
                

            if ((jokeNumber) <= numOfJokes && (jokeNumber) > 0)
            {
                string joke = GetJoke(jokeNumber-1);
                await Context.Channel.SendMessageAsync("Joke #" + (jokeNumber) + " is: \n" + joke);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Joke #" + (jokeNumber) + " does not exist");
            }

            string input = (jokeNumber).ToString();
            string method = "FindJoke";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Clears a specified amount of previous messages from the discord chat channel
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Command("clear")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [Alias("purge, delete")]
        public async Task Clear(int amount = -1)
        {
            if (amount <= 0)
            {
                throw new ArgumentException(Context.User.Mention + ", please enter an amount more than 0 \n**Command Usage: **.clear <**amount**>");
            }
            string user = Context.User.Mention;
            string txt = "messages";

            if (amount == 1)
            {
                txt = "message";
            }

            if (amount <= 100)
            {
                var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();

                await this.Context.Channel.DeleteMessagesAsync(messages);
                await this.ReplyAsync(user + " removed " + amount + " " + txt + " 🗑");
            }
            else
            {
                var msg = await Context.Channel.SendMessageAsync(user + ", you cannot remove more than 100 messages at once");
                int delay = 5000;
                await Task.Delay(delay);
                await msg.DeleteAsync();
            }

            string input = amount.ToString();
            string method = "Clear";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
        }

        /// <summary>
        /// Mutes the selected user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="time"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            var gld = Context.Guild as SocketGuild;
            if (user == null)
            {
                throw new ArgumentException(Context.User.Mention + ", please try again\n**Command Usage: **.kick <**player**> <reason>");
            }

            else if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(Context.User.Mention + ", please try again\n**Command Usage: **.mute <player> <**reason**>");
            }
            else
            {
                await user.KickAsync(reason);
            }

            string title = $"**{user.Username}** was kicked from the server";
            string description = $"**User: **{user.Mention}\n**Reason: **{reason}\n**Kicked by: **{Context.User.Mention}\n";
            string chooseColor = "red";
            embedThis(title, description, chooseColor);

            string input = user.Username + "," + reason;
            string inputForConsole = user.Username + "," + @"""" + reason + @"""";
            string method = "Kick";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, inputForConsole);
        }
        
        /// <summary>
        /// Bans the selected user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                throw new ArgumentException(Context.User.Mention + ", please try again\n**Command Usage: **.ban <**player**> <reason>");
            }
            else if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(Context.User.Mention + ", please try again\n**Command Usage: **.ban <player> <**reason**>");
            }

            var gld = Context.Guild as SocketGuild;
            await gld.AddBanAsync(user);
            string title = $"**{user.Username}** was banned";
            string description = $"**User: **{user.Mention}\n**Reason: **{reason}\n**Banned by: **{Context.User.Mention}\n";
            string chooseColor = "red";
            embedThis(title, description, chooseColor);


            // Log the ban to a ban_history csv file
            string serverName = Context.Guild.Id.ToString();
            string output = GetDate() + "," + GetTime() + "," + Context.User.Username + "," + user.Username + "," + @"""" + reason + @"""";
            string path = string.Concat(Environment.CurrentDirectory, @"\Logs\Servers\");
            System.IO.Directory.CreateDirectory(path + serverName);
            string fileName = string.Concat(path, serverName) + @"\ban_history.csv";

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "Date,Time,User,Banned User,Reason" + Environment.NewLine);
                File.AppendAllText(fileName, output + Environment.NewLine);

            }
            else
            {
                File.AppendAllText(fileName, output + Environment.NewLine);
            }

            string input = user.Username + "," + reason;
            string method = "Ban";
            LogCommand(GetDate(), GetTime(), Context.User.Username, method, input);
            ToConsole(method, input);
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
            switch(chooseColor)
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