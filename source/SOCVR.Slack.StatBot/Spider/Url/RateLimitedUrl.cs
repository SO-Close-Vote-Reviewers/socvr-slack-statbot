using SOCVR.Slack.StatBot.Spider.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Url
{
    public abstract class RateLimitedUrl
    {
        public RateLimitedUrl(IHtmlDownloader downloader)
        {
            this.downloader = downloader;
        }

        private static DateTimeOffset lastFetchTime = DateTimeOffset.Now;
        private object downloadLockObject = new object();
        private IHtmlDownloader downloader;

        protected Uri Url { get; set; }

        /// <summary>
        /// Fetches the page and downloads the html as a string.
        /// A delay will be added so that the server is not called
        /// more than 10 times per second.
        /// </summary>
        /// <returns></returns>
        public string DownloadHtml()
        {
            lock (downloadLockObject)
            {
                var nextFetchTime = lastFetchTime.AddMilliseconds(100);
                var milisecondsToWait = (nextFetchTime - DateTimeOffset.Now).TotalMilliseconds;

                if (milisecondsToWait > 0)
                {
                    Thread.Sleep((int)milisecondsToWait);
                }

                lastFetchTime = DateTimeOffset.Now;
                return downloader.DownloadHtml(Url);
            }
        }

        public override string ToString()
        {
            return Url.ToString();
        }
    }
}
