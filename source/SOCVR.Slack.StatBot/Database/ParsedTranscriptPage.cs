using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    /// <summary>
    /// A record of a completely parsed transcript page.
    /// This is used by the parser to determine what needs to be parsed
    /// </summary>
    class ParsedTranscriptPage
    {
        public Room Room { get; set; }
        public int RoomId { get; set; }

        public DateTime Date { get; set; }
    }
}
