using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.Frontend.CommandSettings
{
    class RangeStatsSettings : CommandSettings
    {
        public const string CommandPattern = @"(?i)range-stats (?:(totals|cv-pls|links|moved|one-box|stars|starred|stars-in) )?((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago) ((\d{4})-(\d{2})-(\d{2})|today|yesterday|(\d+) days ago)(?: (summary-only|table))?";

        public RangeStatsSettings(string userMessage) : base(userMessage) { }

        public string Filter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OutputType { get; set; }

        protected override string GetCommandRegexPattern()
        {
            return CommandPattern;
        }

        protected override void ParseUserMessage(Match userMessageMatch)
        {
            Filter = userMessageMatch.Groups[1].Value;

            StartDate = ExtractDateFromMatch(userMessageMatch, 2, 3, 4, 5, 6);
            EndDate = ExtractDateFromMatch(userMessageMatch, 7, 8, 9, 10, 11);

            //check that the start date is <= to the end date
            if (StartDate > EndDate)
            {
                throw new Exception("Start date is after the end date.");
            }

            OutputType = userMessageMatch.Groups[12].Value;
        }

        private DateTime ExtractDateFromMatch(Match match, int fullDateGroupNumber, int yearGroupNumber, int monthGroupNumber, int dayGroupNumber, int daysAgoGroupNumber)
        {
            /* The date is in one of the following formats:
             * 
             * direct - "yyyy-MM-dd"
             * single word - "today", "yesterday"
             * relative - "X days ago"
             */

            // If group direct year is used then it's in the direct format.
            if (match.Groups[yearGroupNumber].Success)
            {
                var year = match.Groups[yearGroupNumber].Value.Parse<int>();
                var month = match.Groups[monthGroupNumber].Value.Parse<int>();
                var day = match.Groups[dayGroupNumber].Value.Parse<int>();
                return new DateTime(year, month, day);
            }
            // Else, if group X days ago is used then it's relative format.
            else if (match.Groups[daysAgoGroupNumber].Success)
            {
                var daysAgo = match.Groups[daysAgoGroupNumber].Value.Parse<int>();
                return DateTime.UtcNow.Date.AddDays(-daysAgo);
            }
            // Else, it's either "today" or "yesterday" in the overall group. Check "today" first.
            else if (match.Groups[fullDateGroupNumber].Value == "today")
            {
                return DateTime.UtcNow.Date;
            }
            // Else, try "yesterday".
            else if (match.Groups[fullDateGroupNumber].Value == "yesterday")
            {
                return DateTime.UtcNow.Date.AddDays(-1);
            }
            // If none of the above were it, something weird happened.
            else
            {
                throw new Exception("Unable to determine requested date.");
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
