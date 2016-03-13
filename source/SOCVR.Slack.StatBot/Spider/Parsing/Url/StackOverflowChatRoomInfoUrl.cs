using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing.Url
{
    class StackOverflowChatRoomInfoUrl : StackOverflowChatUrl
    {
        public int RoomId { get; private set; }

        public StackOverflowChatRoomInfoUrl(int roomId)
            : base($"rooms/info/{roomId}")
        {
            RoomId = roomId;
        }
    }
}
