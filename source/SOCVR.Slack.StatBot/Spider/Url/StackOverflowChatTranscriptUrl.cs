using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    public class StackOverflowChatTranscriptUrl : StackOverflowChatUrl
    {
        public int RoomId { get; private set; }

        public DateTime Date { get; private set; }

        public int StartHour { get; private set; }

        public int EndHour { get; private set; }

        public StackOverflowChatTranscriptUrl(IHtmlDownloader downloader, int roomId, DateTime date) : this(downloader, roomId, date, 0, 24) { }

        public StackOverflowChatTranscriptUrl(IHtmlDownloader downloader, int roomId, DateTime date, int startHour, int endHour)
            : base(downloader, $"transcript/{roomId}/{date.Year}/{date.Month}/{date.Day}/{startHour}-{endHour}")
        {
            RoomId = roomId;
            Date = date;
            StartHour = startHour;
            EndHour = endHour;
        }
    }
}
