using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;
using SOCVR.Slack.StatBot.Frontend.CommandSettings;
using SOCVR.Slack.StatBot.Frontend.DataFormatters;
using SOCVR.Slack.StatBot.Database;
using SOCVR.Slack.StatBot.Spider.Parsing;

namespace SOCVR.Slack.StatBot.Frontend.Responders
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
                    .Where(x => x.InitialRevisionTs.Date == settings.Date)
                    .Where(x => x.InitialRevisionTs.Hour >= settings.StartHour)
                    .Where(x => x.InitialRevisionTs.Hour <= settings.EndHour)
                    .ToList();

                // Group by user.
                var messagesGroupedByUser = chatMessages
                    .GroupBy(x => x.OriginalPoster.User)
                    .Select(x => new
                    {
                        User = x.Key,
                        Messages = x.Key.Aliases.SelectMany(a => a.Messages)
                    });

                // Calculate results.
                var userStats = messagesGroupedByUser
                    .Select(x => new UserDayStats
                    {
                        Username = x.User.Aliases.Last().DisplayName,
                        TotalMessages = x.Messages.Count(),
                        CloseRequests = x.Messages.Count(m => m.IsCloseVoteRequest),
                        Links = x.Messages.Count(m => m.PlainTextLinkCount > 0),
                        Images = x.Messages.Count(m => m.OneboxType == OneboxType.Image),
                        OneBoxes = x.Messages.Count(m => m.OneboxType != null),
                        StarredMessages = x.Messages.Count(m => m.StarCount > 0),
                        StarsGained = x.Messages.Sum(m => m.StarCount)
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
