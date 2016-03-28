using MargieBot;
using Microsoft.Data.Entity;
using SOCVR.Slack.StatBot.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using SOCVR.Slack.StatBot.Frontend.Responders;
using SOCVR.Slack.StatBot.Spider.Parsing;
using SOCVR.Slack.StatBot.Spider.Url;
using SOCVR.Slack.StatBot.Spider.Download;

namespace SOCVR.Slack.StatBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            //testing

            //var cs = new ChatScraper(new WebClientDownloader());
            //var data = cs.ParseMessage(29290954, 41570);

            //end testing


            using (var db = new MessageStorage())
            {
                Console.WriteLine("Initializing database.");
                db.Database.Migrate();
                Console.WriteLine("Database up to date.");
            }

            var cs = new ChatScraper(new WebClientDownloader());
            cs.ParseAllChatMessages();



            return;

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
