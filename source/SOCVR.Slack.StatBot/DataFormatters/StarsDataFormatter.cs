using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class StarsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataSection(List<UserDayStats> userStats)
        {
            var dataSection = userStats
                .OrderByDescending(x => x.StarsGained)
                .ThenByDescending(x => x.StarredMessages)
                .Where(x => x.StarsGained > 0)
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "Stars Gained",
                        "Starred Messages"
                    },
                    x => x.Username,
                    x => x.StarsGained,
                    x => x.StarredMessages);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalStarredMessages = userStats.Sum(x => x.StarredMessages);
            var totalStarsGiven = userStats.Sum(x => x.StarsGained);

            return "A total of {0} messages were starred and {1} stars were given on {2} between hours {3} and {4}."
                .FormatInline(totalStarredMessages, totalStarsGiven, date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
