using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;
using SOCVR.Slack.StatBot.CommandSettings;
using SOCVR.Slack.StatBot.DataFormatters;
using SOCVR.Slack.StatBot.Database;

namespace SOCVR.Slack.StatBot.Responders
{
    class DayStatsResponder : IResponder
    {
        Regex commandPattern = new Regex(DayStatsSettings.CommandPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        ChatScraper cs = new ChatScraper();

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            DayStatsSettings settings;

            try
            {
                settings = new DayStatsSettings(context.Message.Text);
            }
            catch (Exception ex)
            {
                return new BotMessage()
                {
                    Text = $"Invalid command settings: {ex.Message}"
                };
            }

            // Get all the messages in chat.
            //var chatMessages = cs.GetMessagesForDate(settings.Date, settings.StartHour, settings.EndHour);

            using (var db = new MessageStorage())
            {
                var chatMessages = db.Messages
                    .Where(x => x.PostedAt.Date == settings.Date)
                    .Where(x => x.PostedAt.Hour >= settings.StartHour)
                    .Where(x => x.PostedAt.Hour <= settings.EndHour)
                    .ToList();

                // Group by user and calculate results.
                var userStats = chatMessages
                    .GroupBy(x => x.User)
                    .Select(x => new UserDayStats
                    {
                        Username = x.Key.DisplayName,
                        TotalMessages = x.Count(),
                        CloseRequests = x.Count(m => m.IsCloseVoteRequest),
                        Links = x.Count(m => m.HasLinks),
                        Images = x.Count(m => m.OneboxType == OneboxType.Image),
                        OneBoxes = x.Count(m => m.OneboxType != null),
                        StarredMessages = x.Count(m => m.StarCount > 0),
                        StarsGained = x.Sum(m => m.StarCount)
                    })
                    .ToList();

                // Depending on the filter, choose the correct data formatter.

                var dataFormatter = DetermineDataFormatter(settings.Filter);
                var returnMessage = dataFormatter.FormatDataAsOutputMessage(userStats, settings.Date, settings.StartHour, settings.EndHour, settings.OutputType);

                return returnMessage;
            }
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
