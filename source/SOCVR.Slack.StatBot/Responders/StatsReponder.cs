using MargieBot.Responders;
using SOCVR.Slack.StatBot.DataFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.Responders
{
    class StatsReponder : IResponder
    {
        //let's explain the pattern:

        /*
         *                                 the name of the bot
         *                                 |           the command you are running
                                           |           |     the user can add a filter to only return a particular column. This is optional.
                                           |           |     |                                                               the date the user wants data for. Can type in an ISO format,
                                           |           |     |                                                               or you can use "today", "yesterday", or "X days ago"
                                           |           |     |                                                               |                                               The time range for the messages. Optional.*/
        Regex commandPattern = new Regex(@"([\w-]+?):? stats (?:(totals|cv-pls|links|moved|one-box|stars|starred|stars-in) )?((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago)(?: (\d{1,2})-(\d{1,2}))?", RegexOptions.IgnoreCase);

        ChatScraper cs = new ChatScraper();

        public bool CanRespond(MargieBot.Models.ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //must match command regex
                !context.Message.User.IsSlackbot && //message must be said by a non-bot
                context.Message.MentionsBot; //message must mention the bot
        }

        public MargieBot.Models.BotMessage GetResponse(MargieBot.Models.ResponseContext context)
        {
            var match = commandPattern.Match(context.Message.Text.ToLower());

            var filter = match.Groups[2].Value;
            var startHourRaw = match.Groups[8].Value;
            var endHourRaw = match.Groups[9].Value;

            DateTime date = ExtractDateFromMatch(match);

            var startHour = startHourRaw != ""
                ? startHourRaw.Parse<int>()
                : 0;

            var endHour = endHourRaw != ""
                ? endHourRaw.Parse<int>()
                : 24;

            //get all the messages in chat
            var chatMessages = cs.GetMessagesForDate(date, startHour, endHour);

            //group by user and calculate results
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

            //depending on the filter, choose the correct data formatter

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
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetStarData(List<ChatMessageInfo> messages)
        {
            var data = messages
                .GroupBy(x => x.UserName)
                .Select(x => new
                {
                    UserName = x.Key,
                    StarredMessages = x.Count(m => m.StarCount > 0),
                    TotalStars = x.Sum(m => m.StarCount)
                })
                .Where(x => x.TotalStars > 0)
                .OrderByDescending(x => x.TotalStars)
                .ToStringTable(new[] { "Username", "Total Stars", "# Starred Msgs" },
                    (x) => x.UserName,
                    (x) => x.TotalStars,
                    (x) => x.StarredMessages);

            return "```" + data + "```";
        }

        private DateTime ExtractDateFromMatch(Match match)
        {
            /* the date is in one of the following formats:
             * 
             * direct - "yyyy-MM-dd"
             * single word - "today", "yesterday"
             * relative - "X days ago"
             */

            //if group 4 (direct year) is used then it's in the direct format
            if (match.Groups[4].Success)
            {
                var year = match.Groups[4].Value.Parse<int>();
                var month = match.Groups[5].Value.Parse<int>();
                var day = match.Groups[6].Value.Parse<int>();
                return new DateTime(year, month, day);
            }
            //else, if group 7 (X days ago) is used then it's relative format
            else if (match.Groups[7].Success)
            {
                var daysAgo = match.Groups[7].Value.Parse<int>();
                return DateTime.UtcNow.Date.AddDays(-daysAgo);
            }
            //else, it's either "today" or "yesterday" in group 3. Check "today" first
            else if (match.Groups[3].Value == "today")
            {
                return DateTime.UtcNow.Date;
            }
            //else, try "yesterday"
            else if (match.Groups[3].Value == "yesterday")
            {
                return DateTime.UtcNow.Date.AddDays(-1);
            }
            //if none of the above were it, something weird happened.
            else
            {
                throw new Exception("Unable to determine requested date.");
            }
        }
    }
}
