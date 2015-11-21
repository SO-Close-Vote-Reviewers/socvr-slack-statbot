using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class CvPlsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataSection(List<UserDayStats> userStats)
        {
            var dataSection = userStats
                .Where(x => x.CloseRequests > 0)
                .OrderByDescending(x => x.CloseRequests)
                .ThenBy(x => x.Username)
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "Close Requests",
                    },
                    x => x.Username,
                    x => x.CloseRequests);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalCloseRequests = userStats.Sum(x => x.CloseRequests);

            return "A total of {0} close requests were made on {1} between hours {2} and {3}."
                .FormatInline(totalCloseRequests, date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
