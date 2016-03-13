using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    class StackOverflowChatProfileUrl : StackOverflowChatUrl
    {
        public int ProfileId { get; private set; }

        public StackOverflowChatProfileUrl(IHtmlDownloader downloader, int profileId)
            : base(downloader, $"users/{profileId}")
        {
            ProfileId = profileId;
        }
    }
}
