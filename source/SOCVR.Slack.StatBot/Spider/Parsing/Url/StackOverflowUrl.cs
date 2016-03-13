using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing.Url
{
    /// <summary>
    /// Urls that begin with http://stackoverflow.com/
    /// </summary>
    abstract class StackOverflowUrl : RateLimitedUrl
    {
        public StackOverflowUrl(string path)
        {
            var baseUrl = new Uri("http://stackoverflow.com/");
            Url = new Uri(baseUrl, path);
        }
    }
}
