using MargieBot.Responders;
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
            var startHourRaw = match.Groups[5].Value;
            var endHourRaw = match.Groups[6].Value;

            DateTime date = ExtractDateFromMatch(match);

            var startHour = startHourRaw != ""
                ? startHourRaw.Parse<int>()
                : 0;

            var endHour = endHourRaw != ""
                ? endHourRaw.Parse<int>()
                : 24;

            //get all the messages in chat
            var chatMessages = cs.GetMessagesForDate(date, startHour, endHour);

            var messagesGroupByUser = chatMessages.GroupBy(x => x.UserName);

            var totalCountMessage = "```" + messagesGroupByUser
                .Select(x => new
                {
                    UserName = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .Select(x => "{0} - {1}".FormatInline(x.UserName, x.Count))
                .ToCSV("\n") + "```";

            return new MargieBot.Models.BotMessage()
            {
                Text = totalCountMessage
            };
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
