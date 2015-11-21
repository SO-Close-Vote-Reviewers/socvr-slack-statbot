using MargieBot;
using MargieBot.Responders;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");
            var bot = new Bot();
            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new Responders.StatsReponder());
            bot.Responders.Add(new Responders.HelpResponder());
            bot.Connect(botAPIKey);

            Console.CancelKeyPress += delegate
            {
                bot.Disconnect();
                Console.WriteLine("Got signal to shut down.");
            };

            while (true) { }
        }

    }
}
