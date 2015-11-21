using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class FullTableDataFormatter : BaseDataFormatter
    {
        protected override string GetDataSection(List<UserDayStats> userStats)
        {
            var data = userStats
                .OrderByDescending(x => x.TotalMessages)
                .ToList();

            //add in the totals row
            data.Add(new UserDayStats
            {
                Username = "Total",
                TotalMessages = data.Sum(x => x.TotalMessages),
                CloseRequests = data.Sum(x => x.CloseRequests),
                Links = data.Sum(x => x.Links),
                Images = data.Sum(x => x.Images),
                OneBoxes = data.Sum(x => x.OneBoxes),
                StarredMessages = data.Sum(x => x.StarredMessages),
                StarsGained = data.Sum(x => x.StarsGained)
            });

            var dataSection = data
                .ToStringTable(
                    new[]
                    {
                        "Username",
                        "Total Messages",
                        "cv-pls",
                        "Links",
                        "Images",
                        "One-Boxes",
                        "Starred Msgs",
                        "Stars Gained"
                    },
                    (x) => x.Username,
                    (x) => x.TotalMessages,
                    (x) => x.CloseRequests,
                    (x) => x.Links,
                    (x) => x.Images,
                    (x) => x.OneBoxes,
                    (x) => x.StarredMessages,
                    (x) => x.StarsGained);

            return "```" + dataSection + "```";
        }

        protected override string GetHeaderSection(List<UserDayStats> messages, DateTime date, int startHour, int endHour)
        {
            return "Showing summary information of all posts on {0} between {1} and {2} hours."
                .FormatInline(date.ToString("yyyy-MM-dd"), startHour, endHour);
        }
    }
}
