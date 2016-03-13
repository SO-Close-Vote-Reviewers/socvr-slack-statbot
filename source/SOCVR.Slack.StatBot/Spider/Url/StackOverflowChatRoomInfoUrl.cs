using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    class StackOverflowChatRoomInfoUrl : StackOverflowChatUrl
    {
        public int RoomId { get; private set; }

        public StackOverflowChatRoomInfoUrl(IHtmlDownloader downloader, int roomId)
            : base(downloader, $"rooms/info/{roomId}")
        {
            RoomId = roomId;
        }
    }
}
