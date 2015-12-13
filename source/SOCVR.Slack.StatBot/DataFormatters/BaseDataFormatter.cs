using System;
using System.Collections.Generic;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    abstract class BaseDataFormatter
    {
        public string FormatDataAsOutputMessage(List<UserDayStats> userStats, DateTime date, int startHour, int endHour, string outputType)
        {
            var outputMessage = GetHeaderSection(userStats, date, startHour, endHour);

            //only append the data table if the output calls for it
            if (outputType == "table")
            {
                outputMessage += "\n" + GetDataTable(userStats);
            }

            return outputMessage;
        }

        protected abstract string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour);
        protected abstract string GetDataTable(List<UserDayStats> userStats);
    }
}
