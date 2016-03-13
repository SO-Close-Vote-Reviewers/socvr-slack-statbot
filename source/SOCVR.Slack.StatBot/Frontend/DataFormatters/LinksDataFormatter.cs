using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.Frontend.DataFormatters
{
    class LinksDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
        {
            var dataSection = userStats
                .Where(x => x.Links > 0)
                .OrderByDescending(x => x.Links)
                .ThenBy(x => x.Username)
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "Messages with Links"
                    },
                    x => x.Username,
                    x => x.Links);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalLinkMessages = userStats.Sum(x => x.Links);
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return $"A total of {totalLinkMessages} link messages (not cv-pls, not images, not one-box) were posted between {start} and {end}.";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalLinkMessages = userStats.Sum(x => x.Links);
            var dateValue = date.ToString("yyyy-MM-dd");

            return $"A total of {totalLinkMessages} link messages (not cv-pls, not images, not one-box) were posted on {dateValue} between hours {startHour} and {endHour}.";
        }
    }
}
