using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;
using SOCVR.Slack.StatBot.Frontend.CommandSettings;
using System.Threading;
using SOCVR.Slack.StatBot.Frontend.DataFormatters;
using SOCVR.Slack.StatBot.Spider.Parsing;

namespace SOCVR.Slack.StatBot.Frontend.Responders
{
    class DateRangeStatsResponder : IResponder
    {
        Regex commandPattern = new Regex(RangeStatsSettings.CommandPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        //ChatScraper cs = new ChatScraper();

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            //RangeStatsSettings settings;

            //try
            //{
            //    settings = new RangeStatsSettings(context.Message.Text);
            //}
            //catch (Exception ex)
            //{
            //    return new BotMessage()
            //    {
            //        Text = $"Invalid command settings: {ex.Message}"
            //    };
            //}

            //var datesToCheck = Enumerable.Range(0, 1 + settings.EndDate.Subtract(settings.StartDate).Days)
            //    .Select(offset => settings.StartDate.AddDays(offset));

            //var allChatMessages = new List<ChatMessageInfo>();

            //foreach (var dateToCheck in datesToCheck)
            //{
            //    allChatMessages.AddRange(cs.GetMessagesForDate(dateToCheck, 0, 24));
            //    Thread.Sleep(250);
            //}

            //// Group by user and calculate results.
            //var userStats = allChatMessages
            //    .GroupBy(x => x.UserName)
            //    .Select(x => new UserDayStats
            //    {
            //        Username = x.Key,
            //        TotalMessages = x.Count(),
            //        CloseRequests = x.Count(m => m.IsCloseVoteRequest),
            //        Links = x.Count(m => m.HasLinks),
            //        Images = x.Count(m => m.IsImage),
            //        OneBoxes = x.Count(m => m.IsOneBoxed),
            //        StarredMessages = x.Count(m => m.StarCount > 0),
            //        StarsGained = x.Sum(m => m.StarCount)
            //    })
            //    .ToList();

            //// Depending on the filter, choose the correct data formatter.

            //var dataFormatter = DetermineDataFormatter(settings.Filter);
            //var returnMessage = dataFormatter.FormatDataAsOutputMessage(userStats, settings.StartDate, settings.EndDate, settings.OutputType);

            //return returnMessage;

            return null;
        }

        private BaseDataFormatter DetermineDataFormatter(string filter)
        {
            switch (filter)
            {
                case "":
                    return new FullTableDataFormatter();
                case "totals":
                    return new TotalsDataFormatter();
                case "stars":
                    return new StarsDataFormatter();
                case "cv-pls":
                    return new CvPlsDataFormatter();
                case "links":
                    return new LinksDataFormatter();
                case "one-box":
                    return new OneBoxeDataFormatter();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
