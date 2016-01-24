using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Parsing.Url
{
    class StackOverflowChatUrl : RateLimitedUrl
    {
        public StackOverflowChatUrl(string path)
        {
            var baseUrl = new Uri("http://chat.stackoverflow.com/");
            Url = new Uri(baseUrl, path);
        }
    }
}
