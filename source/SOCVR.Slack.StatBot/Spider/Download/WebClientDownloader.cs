using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Download
{
    class WebClientDownloader : IHtmlDownloader
    {
        public string DownloadHtml(Uri url)
        {
            using (var htmlClient = new WebClient())
            {
                return htmlClient.DownloadString(url);
            }
        }
    }
}
