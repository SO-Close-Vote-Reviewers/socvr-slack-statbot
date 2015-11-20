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
            var scraper = new ChatScraper();
            scraper.GetMessagesForDate(DateTime.Parse("2015-11-20"));

            var botRunningTask = Task.Run(() => RunBot());
            Task.WaitAll(botRunningTask);
                   
            while(true)
            {
                //need to copy how Closey does this
            }
        }

        private static async Task RunBot()
        {
            var botAPIKey = Environment.GetEnvironmentVariable("SlackBotAPIKey", EnvironmentVariableTarget.User);
            var bot = new Bot();
            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new EchoResponder());
            await bot.Connect(botAPIKey);
        }
    }

    public class EchoResponder : IResponder
    {
        public bool CanRespond(MargieBot.Models.ResponseContext context)
        {
            return
                !context.Message.User.IsSlackbot &&
                context.Message.ChatHub.Type == MargieBot.Models.SlackChatHubType.Channel &&
                context.Message.ChatHub.Name == "#bot-testing" &&
                context.Message.Text.StartsWith("echo ");
        }

        public MargieBot.Models.BotMessage GetResponse(MargieBot.Models.ResponseContext context)
        {
            return new MargieBot.Models.BotMessage()
            {
                Text = "Echoing {0}'s message: {1}".FormatInline(context.Message.User.FormattedUserID, context.Message.Text)
            };
        }
    }

}
