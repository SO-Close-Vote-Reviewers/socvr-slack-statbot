using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing
{
    class ParsedMessageData
    {
        public long MessageId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorDisplayName { get; set; }
        public int RoomId { get; set; }
        public string CurrentTest { get; set; }
        public string CurrentHtmlContent { get; set; }
        public DateTimeOffset InitialRevisionTs { get; set; }
        public string RawOneboxName { get; set; }
        public int PlainTextLinkCount { get; set; }
        public int TagsCount { get; set; }
        public bool IsCloseVoteRequest { get; set; }
        public int StarCount { get; set; }

    }

    class ParsedMessageRevision
    {
        public int RevisionNumber { get; set; }
        public int AuthorId { get; set; }
        public string DisplayName { get; set; }
        public string MessageText { get; set; }
        public string MessageHtml { get; set; }
    }
}
