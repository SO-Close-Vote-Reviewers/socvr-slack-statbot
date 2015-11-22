using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class TotalsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataSection(List<UserDayStats> userStats)
        {
            var dataSection = userStats
                .OrderByDescending(x => x.TotalMessages)
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "Total Messages",
                    },
                    x => x.Username,
                    x => x.TotalMessages);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalMessages = userStats.Sum(x => x.TotalMessages);

            return "A total of {0} messages were posted on {1} between hours {2} and {3}."
                .FormatInline(totalMessages, date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
