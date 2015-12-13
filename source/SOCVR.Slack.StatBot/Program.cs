using MargieBot;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SOCVR.Slack.StatBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");

            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new Responders.DayStatsResponder());
            bot.Responders.Add(new Responders.HelpResponder());
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
