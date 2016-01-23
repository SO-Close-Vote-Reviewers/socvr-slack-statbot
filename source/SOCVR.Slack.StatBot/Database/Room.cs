using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    class Room
    {
        public int RoomId { get; set; }

        public virtual List<Message> Messages { get; set; }
    }
}
