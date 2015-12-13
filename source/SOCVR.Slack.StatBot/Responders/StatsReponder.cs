using MargieBot.Responders;
using SOCVR.Slack.StatBot.DataFormatters;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.Responders
{
    class StatsReponder : IResponder
    {
        Regex commandPattern = new Regex(@"(?i)day-stats (?:(totals|cv-pls|links|moved|one-box|stars|starred|stars-in) )?((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago)(?: (\d{1,2})-(\d{1,2}))?(?: (summary-only|table|csv))?", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        ChatScraper cs = new ChatScraper();

        public bool CanRespond(MargieBot.Models.ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public MargieBot.Models.BotMessage GetResponse(MargieBot.Models.ResponseContext context)
        {
            var match = commandPattern.Match(context.Message.Text.ToLower());

            var filter = match.Groups[1].Value;
            var startHourRaw = match.Groups[7].Value;
            var endHourRaw = match.Groups[8].Value;

            DateTime date = ExtractDateFromMatch(match);

            var startHour = startHourRaw != ""
                ? startHourRaw.Parse<int>()
                : 0;

            var endHour = endHourRaw != ""
                ? endHourRaw.Parse<int>()
                : 24;

            // Get all the messages in chat.
            var chatMessages = cs.GetMessagesForDate(date, startHour, endHour);

            // Group by user and calculate results.
            var userStats = chatMessages
                .GroupBy(x => x.UserName)
                .Select(x => new UserDayStats
                {
                    Username = x.Key,
                    TotalMessages = x.Count(),
                    CloseRequests = x.Count(m => m.IsCloseVoteRequest),
                    Links = x.Count(m => m.HasLinks),
                    Images = x.Count(m => m.IsImage),
                    OneBoxes = x.Count(m => m.IsOneBoxed),
                    StarredMessages = x.Count(m => m.StarCount > 0),
                    StarsGained = x.Sum(m => m.StarCount)
                })
                .ToList();

            // Depending on the filter, choose the correct data formatter.

            var dataFormatter = DetermineDataFormatter(filter);
            var returnMessage = dataFormatter.FormatDataAsOutputMessage(userStats, date, startHour, endHour);

            return new MargieBot.Models.BotMessage()
            {
                Text = returnMessage,
            };
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

        private DateTime ExtractDateFromMatch(Match match)
        {
            /* The date is in one of the following formats:
             * 
             * direct - "yyyy-MM-dd"
             * single word - "today", "yesterday"
             * relative - "X days ago"
             */

            // If group 3 (direct year) is used then it's in the direct format.
            if (match.Groups[3].Success)
            {
                var year = match.Groups[3].Value.Parse<int>();
                var month = match.Groups[4].Value.Parse<int>();
                var day = match.Groups[5].Value.Parse<int>();
                return new DateTime(year, month, day);
            }
            // Else, if group 6 (X days ago) is used then it's relative format.
            else if (match.Groups[6].Success)
            {
                var daysAgo = match.Groups[7].Value.Parse<int>();
                return DateTime.UtcNow.Date.AddDays(-daysAgo);
            }
            // Else, it's either "today" or "yesterday" in group 2. Check "today" first.
            else if (match.Groups[2].Value == "today")
            {
                return DateTime.UtcNow.Date;
            }
            // Else, try "yesterday".
            else if (match.Groups[2].Value == "yesterday")
            {
                return DateTime.UtcNow.Date.AddDays(-1);
            }
            // If none of the above were it, something weird happened.
            else
            {
                throw new Exception("Unable to determine requested date.");
            }
        }
    }
}
