using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    class FullTableDataFormatter : BaseDataFormatter
    {
        protected override string GetDataTable(List<UserDayStats> userStats)
        {
            var data = userStats
                .OrderByDescending(x => x.TotalMessages)
                .ToList();

            // Add in the totals row.
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
                    x => x.Username,
                    x => x.TotalMessages,
                    x => x.CloseRequests,
                    x => x.Links,
                    x => x.Images,
                    x => x.OneBoxes,
                    x => x.StarredMessages,
                    x => x.StarsGained);

            return $"```{dataSection}```";
        }

        protected override string GetHeaderSection(List<UserDayStats> messages, DateTime date, int startHour, int endHour)
        {
            var totalMessages = messages.Sum(x=>x.TotalMessages);
            var cvplsTotals = messages.Sum(x => x.CloseRequests);
            var linkTotals = messages.Sum(x => x.Links);
            var imageTotals = messages.Sum(x => x.Images);
            var oneboxTotals = messages.Sum(x => x.OneBoxes);
            var starredMessageTotals = messages.Sum(x => x.StarredMessages);
            var starsGainedTotals = messages.Sum(x => x.StarsGained);

            var returnMessage = $"Stats for {date.ToString("yyyy-MM-dd")} between {startHour} and {endHour}: ";

            returnMessage += $"{totalMessages} total messages, ";
            returnMessage += $"{cvplsTotals} close vote requests, ";
            returnMessage += $"{linkTotals} link messages, ";
            returnMessage += $"{oneboxTotals} one-boxed messages, ";
            returnMessage += $"{starredMessageTotals} messages starred, ";
            returnMessage += $"{starsGainedTotals} stars given.";

            return returnMessage;
        }
    }
}
