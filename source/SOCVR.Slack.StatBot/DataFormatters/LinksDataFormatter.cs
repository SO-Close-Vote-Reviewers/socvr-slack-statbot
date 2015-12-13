using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalLinkMessages = userStats.Sum(x => x.Links);

            return "A total of {0} link messages (not cv-pls, not images, not one-box) were posted on {1} between hours {2} and {3}."
                .FormatInline(totalLinkMessages, date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
