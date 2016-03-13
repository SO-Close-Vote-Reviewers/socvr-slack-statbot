using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    /// <summary>
    /// Urls that begin with http://stackoverflow.com/
    /// </summary>
    abstract class StackOverflowUrl : RateLimitedUrl
    {
        public StackOverflowUrl(IHtmlDownloader downloader, string path)
            : base(downloader)
        {
            var baseUrl = new Uri("http://stackoverflow.com/");
            Url = new Uri(baseUrl, path);
        }
    }
}
