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

        [Test]
        public void ParseMessage_AuthorId()
        {
            var messageId = 29290954;
            var roomId = 41570;
            var parsedData = cs.ParseMessage(messageId, roomId);

            var expectedAuthorId = 5292302;
            var actualAuthorId = parsedData.AuthorId;

            Assert.AreEqual(expectedAuthorId, actualAuthorId);
        }
    }
}
