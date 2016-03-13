using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing
{
    public class ParsedMessageData
    {
        public string AuthorDisplayName { get; set; }
        public int AuthorId { get; set; }
        public string CurrentHtmlContent { get; set; }
        public string CurrentText { get; set; }
        public DateTimeOffset InitialRevisionTs { get; set; }
        public bool IsCloseVoteRequest { get; set; }
        public long MessageId { get; set; }
        public int PlainTextLinkCount { get; set; }
        public string RawOneboxName { get; set; }
        public int RoomId { get; set; }
        public int StarCount { get; set; }
        public int TagsCount { get; set; }

        public List<ParsedMessageRevision> Revisions { get; set; }
    }

    public class ParsedMessageRevision
    {
        public int AuthorId { get; set; }
        public string DisplayName { get; set; }
        public string MessageHtml { get; set; }
        public string MessageText { get; set; }
        public int RevisionNumber { get; set; }
    }
}
