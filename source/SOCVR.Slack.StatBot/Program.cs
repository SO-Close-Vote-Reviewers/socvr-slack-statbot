using MargieBot;
using Microsoft.Data.Entity;
using SOCVR.Slack.StatBot.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using SOCVR.Slack.StatBot.Frontend.Responders;

namespace SOCVR.Slack.StatBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            using (var db = new MessageStorage())
            {
                Console.WriteLine("Initializing database.");
                db.Database.Migrate();
                Console.WriteLine("Database up to date.");
            }

            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");

            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new DayStatsResponder());
            bot.Responders.Add(new DateRangeStatsResponder());
            bot.Responders.Add(new HelpResponder());
            bot.Connect(botAPIKey);

            Console.CancelKeyPress += delegate
            {
                bot.Disconnect();
                Console.WriteLine("Got signal to shut down.");
                exitMre.Set();
            };

            exitMre.WaitOne();
        }

    }

}
