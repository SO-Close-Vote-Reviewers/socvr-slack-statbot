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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalOneBoxes = userStats.Sum(x => x.OneBoxes);

            return "A total of {0} one-box messages (non-images) were posted on {1} between hours {2} and {3}."
                .FormatInline(totalOneBoxes, date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
