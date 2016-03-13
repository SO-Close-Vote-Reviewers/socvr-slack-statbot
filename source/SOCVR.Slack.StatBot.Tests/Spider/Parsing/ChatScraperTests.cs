using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SOCVR.Slack.StatBot.Spider.Parsing;
using SOCVR.Slack.StatBot.Tests.Spider.Download;

namespace SOCVR.Slack.StatBot.Tests.Spider.Parsing
{
    [TestFixture]
    class ChatScraperTests
    {
        ChatScraper cs;
        PreDownloadedHtmlDownloader downloader;

        [SetUp]
        public void Setup()
        {
            downloader = new PreDownloadedHtmlDownloader(TestContext.CurrentContext.TestDirectory);
            cs = new ChatScraper(downloader);
        }

        [TestCase(29290954, 41570, 5292302)]
        [TestCase(29310752, 41570, 1043380)]
        public void ParseMessage_AuthorId(int messageId, int roomId, int expectedAuthorId)
        {
            var parsedData = cs.ParseMessage(messageId, roomId);
            var actualAuthorId = parsedData.AuthorId;
            Assert.AreEqual(expectedAuthorId, actualAuthorId);
        }

        [TestCase(29290954, 41570, "Petter Friberg")]
        [TestCase(29310752, 41570, "gunr2171")]
        public void ParseMessage_AuthorDisplayName(int messageId, int roomId, string expectedAuthorDisplayName)
        {
            var parsedData = cs.ParseMessage(messageId, roomId);
            var actualAuthorDisplayName = parsedData.AuthorDisplayName;
            Assert.AreEqual(expectedAuthorDisplayName, actualAuthorDisplayName);
        }
    }
}
