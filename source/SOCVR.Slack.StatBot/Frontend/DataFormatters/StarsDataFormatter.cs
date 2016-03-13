using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.Frontend.DataFormatters
{
    class StarsDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalStarredMessages = userStats.Sum(x => x.StarredMessages);
            var totalStarsGiven = userStats.Sum(x => x.StarsGained);
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return $"A total of {totalStarredMessages} messages were starred and {totalStarsGiven} stars were given between {start} and {end}.";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalStarredMessages = userStats.Sum(x => x.StarredMessages);
            var totalStarsGiven = userStats.Sum(x => x.StarsGained);
            var dateValue = date.ToString("yyyy-MM-dd");

            return $"A total of {totalStarredMessages} messages were starred and {totalStarsGiven} stars were given on {dateValue} between hours {startHour} and {endHour}.";
        }
    }
}
