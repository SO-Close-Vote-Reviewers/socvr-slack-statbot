using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    public class StackOverflowChatUrl : RateLimitedUrl
    {
        public StackOverflowChatUrl(IHtmlDownloader downloader, string path)
            : base(downloader)
        {
            var baseUrl = new Uri("http://chat.stackoverflow.com/");
            Url = new Uri(baseUrl, path);
        }
    }
}
