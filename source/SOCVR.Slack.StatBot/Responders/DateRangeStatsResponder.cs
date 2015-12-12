using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;

namespace SOCVR.Slack.StatBot.Responders
{
    class DateRangeStatsResponder : IResponder
    {
        /*
         *                             the command you are running
                                       |     the user can add a filter to only return a particular column. This is optional.
                                       |     |                                                               the date the user wants data for. Can type in an ISO format,
                                       |     |                                                               or you can use "today", "yesterday", or "X days ago"
                                       |     |                                                               |                                                           The time range for the messages. Optional.*/
        Regex commandPattern = new Regex(@"(?i)day-stats (?:(totals|cv-pls|links|moved|one-box|stars|starred|stars-in) )?((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago)(?: (\d{1,2})-(\d{1,2}))?", RegexOptions.Compiled | RegexOptions.CultureInvariant);


        public bool CanRespond(ResponseContext context)
        {
            throw new NotImplementedException();
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            throw new NotImplementedException();
        }
    }
}
