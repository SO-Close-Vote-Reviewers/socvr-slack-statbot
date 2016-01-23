using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    class Message
    {
        public int Id { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public Room Room { get; set; }
        public int RoomId { get; set; }

        public DateTimeOffset PostedAt { get; set; }
        
        public OneboxType? OneboxType { get; set; }

        public bool HasLinks { get; set; }
        public bool HasTagFormatting { get; set; }
        public bool IsCloseVoteRequest { get; set; }
        public int StarCount { get; set; }
    }
}
