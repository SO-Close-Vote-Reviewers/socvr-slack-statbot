using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class TotalsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalMessages = userStats.Sum(x => x.TotalMessages);
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return $"A total of {totalMessages} messages were posted between {start} and {end}.";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalMessages = userStats.Sum(x => x.TotalMessages);
            var dateValue = date.ToString("yyyy-MM-dd");

            return $"A total of {totalMessages} messages were posted on {dateValue} between hours {startHour} and {endHour}.";
        }
    }
}
