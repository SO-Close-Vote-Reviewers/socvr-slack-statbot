using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class OneBoxeDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
        {
            var dataSection = userStats
                .Where(x => x.OneBoxes > 0)
                .OrderByDescending(x => x.OneBoxes)
                .ThenBy(x => x.Username)
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "One-Boxed Messages",
                    },
                    x => x.Username,
                    x => x.OneBoxes);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalOneBoxes = userStats.Sum(x => x.OneBoxes);
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return $"A total of {totalOneBoxes} one-box messages (non-images) were posted between {start} and {end}.";
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalOneBoxes = userStats.Sum(x => x.OneBoxes);
            var dateValue = date.ToString("yyyy-MM-dd");

            return $"A total of {totalOneBoxes} one-box messages (non-images) were posted on {dateValue} between hours {startHour} and {endHour}.";
        }
    }
}
