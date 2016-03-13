using SOCVR.Slack.StatBot.Spider.Download;
using SOCVR.Slack.StatBot.Tests.DownloadedPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Tests.Spider.Download
{
    class PreDownloadedHtmlDownloader : IHtmlDownloader
    {
        private string testLocation;

        public PreDownloadedHtmlDownloader(string testLocation)
        {
            this.testLocation = testLocation;
        }

        public string DownloadHtml(Uri url)
        {
            var filePath = RegisteryAccessor.GetFile(testLocation, url);

            if (filePath == null)
                throw new Exception($"Could not find file for url {url}");

            var fileContents = File.ReadAllText(filePath);
            return fileContents;
        }
    }
}
