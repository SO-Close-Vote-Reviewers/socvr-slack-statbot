using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class CvPlsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalCloseRequests = userStats.Sum(x => x.CloseRequests);
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return $"A total of {totalCloseRequests} close requests were made between {start} and {end}.";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalCloseRequests = userStats.Sum(x => x.CloseRequests);
            var dateValue = date.ToString("yyyy-MM-dd");

            return $"A total of {totalCloseRequests} close requests were made on {dateValue} between hours {startHour} and {endHour}.";
        }
    }
}
