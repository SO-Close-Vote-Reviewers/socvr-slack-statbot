using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SOCVR.Slack.StatBot.Spider.Parsing;
using SOCVR.Slack.StatBot.Tests.Spider.Download;
using SOCVR.Slack.StatBot.Tests.DownloadedPages;
using Newtonsoft.Json.Linq;

namespace SOCVR.Slack.StatBot.Tests.Spider.Parsing
{
    [TestFixture]
    class ChatScraperTests
    {
        ChatScraper cs;
        PreDownloadedHtmlDownloader downloader;
        string testLocation;

        [SetUp]
        public void Setup()
        {
            downloader = new PreDownloadedHtmlDownloader(TestContext.CurrentContext.TestDirectory);
            cs = new ChatScraper(downloader);
            testLocation = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase(29290954, 41570, 5292302)]
        [TestCase(29310752, 41570, 1043380)]
        public void ParseMessage_AuthorId(int messageId, int roomId, int expectedAuthorId)
        {
            TestParseMessageReturnData(messageId, roomId, expectedAuthorId, (x) => x.AuthorId);
        }

        [TestCase(29290954, 41570, "Petter Friberg")]
        [TestCase(29310752, 41570, "gunr2171")]
        public void ParseMessage_AuthorDisplayName(int messageId, int roomId, string expectedAuthorDisplayName)
        {
            TestParseMessageReturnData(messageId, roomId, expectedAuthorDisplayName, (x) => x.AuthorDisplayName);
        }

        [TestCase(29290954, 41570, "@gunr2171 When do you elect?")]
        [TestCase(29310752, 41570, "test one two three four")]
        [TestCase(29306690, 41570, "delv-pls [turtle] Cleanup 10 11 12")]
        public void ParseMessage_CurrentText(int messageId, int roomId, string expectedCurrentText)
        {
            TestParseMessageReturnData(messageId, roomId, expectedCurrentText, (x) => x.CurrentText);
        }

        [TestCase(29290954, 41570, false)]
        [TestCase(29310752, 41570, false)]
        [TestCase(29306690, 41570, false)]
        [TestCase(29271581, 41570, true)]
        public void ParseMessage_IsCloseVoteRequest(int messageId, int roomId, bool expectedValue)
        {
            TestParseMessageReturnData(messageId, roomId, expectedValue, (x) => x.IsCloseVoteRequest);
        }

        [TestCase(29290954, 41570)]
        [TestCase(29310752, 41570)]
        [TestCase(29306690, 41570)]
        [TestCase(29271581, 41570)]
        public void ParseMessage_MessageId(int messageId, int roomId)
        {
            TestParseMessageReturnData(messageId, roomId, messageId, (x) => x.MessageId);
        }

        //[TestCase(29290954, 41570, null)]
        //[TestCase(29310752, 41570, null)]
        //[TestCase(29306690, 41570, null)]
        //[TestCase(29271581, 41570, null)]
        //[TestCase(29310780, 41570, "ob-post")]
        [TestCaseSource("Generate_ParseMessage_RawOneBoxName")]
        public void ParseMessage_RawOneBoxName(int messageId, int roomId, string expectedValue)
        {
            TestParseMessageReturnData(messageId, roomId, expectedValue, (x) => x.RawOneboxName);
        }

        private void TestParseMessageReturnData<TActual>(int messageId, int roomId, TActual expected, Func<ParsedMessageData, TActual> getActual)
        {
            var parsedData = cs.ParseMessage(messageId, roomId);
            var actual = getActual(parsedData);
            Assert.AreEqual(expected, actual);
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_RawOneBoxName
        {
            get
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;

                foreach (var messageId in RegisteryAccessor.GetMessageIds(dir))
                {
                    var data = RegisteryAccessor.GetRegisteryDataForMessage(dir, messageId);
                    yield return new TestCaseData(data["MessageId"].Value<int>(), data["RoomId"].Value<int>(), data["RawOneboxName"].Value<string>());
                }
            }
        }
    }

    class ChatScraperTestsFactory
    {
        public static IEnumerable<TestCaseData> ParseMessage_RawOneBoxName(string testDirectory)
        {
            var data = RegisteryAccessor.GetRegisteryDataForMessage(testDirectory, 29290954);

            yield return new TestCaseData(data["MessageId"], data["RoomId"], data["RawOneboxName"]);
        }
    }
}
