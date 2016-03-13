using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing.Url
{
    class StackOverflowChatMessageHistoryUrl : StackOverflowChatUrl
    {
        public long MessageId { get; private set; }

        public StackOverflowChatMessageHistoryUrl(long messageId)
            : base($"messages/{messageId}/history")
        {
            MessageId = messageId;
        }
    }
}
