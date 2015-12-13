using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.CommandSettings
{
    class DayStatsSettings : CommandSettings
    {
        public const string CommandPattern = @"(?i)day-stats (?:(totals|cv-pls|links|moved|one-box|stars|starred|stars-in) )?((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago)(?: (\d{1,2})-(\d{1,2}))?(?: (summary-only|table|csv))?";

        public DayStatsSettings(string userMessage) : base(userMessage) { }

        public string Filter { get; set; }
        public DateTime Date { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public string OutputType { get; set; }

        protected override string GetCommandRegexPattern()
        {
            return CommandPattern;
        }

        protected override void ParseUserMessage(Match userMessageMatch)
        {
            Filter = userMessageMatch.Groups[1].Value;
            var startHourRaw = userMessageMatch.Groups[7].Value;
            var endHourRaw = userMessageMatch.Groups[8].Value;

            Date = ExtractDateFromMatch(userMessageMatch);

            StartHour = startHourRaw != ""
                ? startHourRaw.Parse<int>()
                : 0;

            EndHour = endHourRaw != ""
                ? endHourRaw.Parse<int>()
                : 24;

            OutputType = userMessageMatch.Groups[9].Value;
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
