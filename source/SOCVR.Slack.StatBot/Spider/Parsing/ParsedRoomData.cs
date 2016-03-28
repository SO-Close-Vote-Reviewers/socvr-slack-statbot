using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing
{
    public class ParsedRoomData
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
