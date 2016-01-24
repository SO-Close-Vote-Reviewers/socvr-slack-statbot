using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Parsing.Url
{
    abstract class RateLimitedUrl
    {
        private static DateTimeOffset lastFetchTime = DateTimeOffset.Now;
        private object downloadLockObject = new object();

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

                //download the html
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return Url.ToString();
        }
    }
}
