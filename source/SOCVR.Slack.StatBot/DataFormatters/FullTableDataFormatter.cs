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

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate)
        {
            var totalMessages = userStats.Sum(x => x.TotalMessages);
            var cvplsTotals = userStats.Sum(x => x.CloseRequests);
            var linkTotals = userStats.Sum(x => x.Links);
            var imageTotals = userStats.Sum(x => x.Images);
            var oneboxTotals = userStats.Sum(x => x.OneBoxes);
            var starredMessageTotals = userStats.Sum(x => x.StarredMessages);
            var starsGainedTotals = userStats.Sum(x => x.StarsGained);

            var returnMessage = $"Stats between {startDate.ToString("yyyy-MM-dd")} and {endDate.ToString("yyyy-MM-dd")}: ";

            returnMessage += $"{totalMessages} total messages, ";
            returnMessage += $"{cvplsTotals} close vote requests, ";
            returnMessage += $"{linkTotals} link messages, ";
            returnMessage += $"{imageTotals} images, ";
            returnMessage += $"{oneboxTotals} one-boxed messages, ";
            returnMessage += $"{starredMessageTotals} messages starred, ";
            returnMessage += $"{starsGainedTotals} stars given.";

            return returnMessage;
        }

        protected override string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var totalMessages = userStats.Sum(x=>x.TotalMessages);
            var cvplsTotals = userStats.Sum(x => x.CloseRequests);
            var linkTotals = userStats.Sum(x => x.Links);
            var imageTotals = userStats.Sum(x => x.Images);
            var oneboxTotals = userStats.Sum(x => x.OneBoxes);
            var starredMessageTotals = userStats.Sum(x => x.StarredMessages);
            var starsGainedTotals = userStats.Sum(x => x.StarsGained);

            var returnMessage = $"Stats for {date.ToString("yyyy-MM-dd")} between {startHour} and {endHour}: ";

            returnMessage += $"{totalMessages} total messages, ";
            returnMessage += $"{cvplsTotals} close vote requests, ";
            returnMessage += $"{linkTotals} link messages, ";
            returnMessage += $"{imageTotals} images, ";
            returnMessage += $"{oneboxTotals} one-boxed messages, ";
            returnMessage += $"{starredMessageTotals} messages starred, ";
            returnMessage += $"{starsGainedTotals} stars given.";

            return returnMessage;
        }
    }
}
