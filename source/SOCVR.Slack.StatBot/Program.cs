using MargieBot;
using MargieBot.Responders;
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
            var botRunningTask = Task.Run(() => RunBot());
            Task.WaitAll(botRunningTask);
                   
            while(true)
            {
                //need to copy how Closey does this
            }
        }

        private static async Task RunBot()
        {
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");
            var bot = new Bot();
            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new Responders.StatsReponder());
            await bot.Connect(botAPIKey);
        }
    }
}
