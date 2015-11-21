using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.DataFormatters
{
    abstract class BaseDataFormatter
    {
        public string FormatDataAsOutputMessage(List<UserDayStats> userStats, DateTime date, int startHour, int endHour)
        {
            var outputMessage = GetHeaderSection(userStats, date, startHour, endHour) + "\n" + GetDataSection(userStats);
            return outputMessage;
        }

        protected abstract string GetHeaderSection(List<UserDayStats> userStats, DateTime date, int startHour, int endHour);
        protected abstract string GetDataSection(List<UserDayStats> userStats);
    }
}
