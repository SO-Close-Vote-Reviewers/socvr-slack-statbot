using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Parsing.Url
{
    class StackOverflowChatTranscriptUrl : StackOverflowChatUrl
    {
        public StackOverflowChatTranscriptUrl(int roomId, DateTime date)
            : this(roomId, date, 0, 24)
        {

        }

        public StackOverflowChatTranscriptUrl(int roomId, DateTime date, int startHour, int endHour)
            : base($"transcript/{roomId}/{date.Year}/{date.Month}/{date.Day}/{startHour}/{endHour}")
        {

        }
    }
}
