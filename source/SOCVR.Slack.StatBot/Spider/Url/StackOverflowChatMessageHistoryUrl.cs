using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    class StackOverflowChatMessageHistoryUrl : StackOverflowChatUrl
    {
        public long MessageId { get; private set; }

        public StackOverflowChatMessageHistoryUrl(IHtmlDownloader downloader, long messageId)
            : base(downloader, $"messages/{messageId}/history")
        {
            MessageId = messageId;
        }
    }
}
