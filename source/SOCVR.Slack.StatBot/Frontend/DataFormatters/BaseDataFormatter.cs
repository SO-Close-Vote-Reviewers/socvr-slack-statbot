using MargieBot.Models;
using System;
using System.Collections.Generic;

namespace SOCVR.Slack.StatBot.Frontend.DataFormatters
{
    abstract class BaseDataFormatter
    {
        public BotMessage FormatDataAsOutputMessage(List<UserDayStats> userStats, DateTime date, int startHour, int endHour, string outputType)
        {
            var outputText = GetHeaderSection(userStats, date, startHour, endHour);

            //only append the data table if the output calls for it
            if (outputType == "table")
            {
                outputText += "\n" + GetDataTable(userStats);
            }

            return new BotMessage()
            {
                Text = outputText
            };
        }

        public BotMessage FormatDataAsOutputMessage(List<UserDayStats> userStats, DateTime startDate, DateTime endDate, string outputType)
        {
            var outputText = GetHeaderSection(userStats, startDate, endDate);

            //only append the data table if the output calls for it
            if (outputType == "table")
            {
                outputText += "\n" + GetDataTable(userStats);
            }

            return new BotMessage()
            {
                Text = outputText
            };
        }

        protected abstract string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour);
        protected abstract string GetHeaderSection(List<UserDayStats> userStats, DateTime startDate, DateTime endDate);
        protected abstract string GetDataTable(List<UserDayStats> userStats);
    }
}
