using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Download
{
    public interface IHtmlDownloader
    {
        string DownloadHtml(Uri url);
    }
}
