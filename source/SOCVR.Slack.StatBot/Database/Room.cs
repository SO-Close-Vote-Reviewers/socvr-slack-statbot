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

        /// <summary>
        /// The title of the room. For example, "SO Close Vote Reviewers"
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Message> Messages { get; set; }
        public virtual List<ParsedTranscriptPage> ParsedTranscriptPages { get; set; }
    }
}
