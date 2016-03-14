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

        public static IEnumerable<TestCaseData> Generate_ParseMessage_AuthorDisplayName()
        {
            return GenerateTestCases<string>("AuthorDisplayName");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_AuthorId()
        {
            return GenerateTestCases<int>("AuthorId");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_CurrentMarkdownContent()
        {
            return GenerateTestCases<string>("CurrentMarkdownContent");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_CurrentText()
        {
            return GenerateTestCases<string>("CurrentText");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_IsCloseVoteRequest()
        {
            return GenerateTestCases<bool>("IsCloseVoteRequest");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_MessageId()
        {
            return GenerateTestCases<int>("MessageId");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_PlainTextLinkCount()
        {
            return GenerateTestCases<int>("PlainTextLinkCount");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_RawOneBoxName()
        {
            return GenerateTestCases<string>("RawOneboxName");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_RoomId()
        {
            return GenerateTestCases<int>("RoomId");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_StarCount()
        {
            return GenerateTestCases<int>("StarCount");
        }

        public static IEnumerable<TestCaseData> Generate_ParseMessage_TagsCount()
        {
            return GenerateTestCases<int>("TagsCount");
        }

        [TestCaseSource("Generate_ParseMessage_AuthorDisplayName")]
        public void ParseMessage_AuthorDisplayName(int messageId, int roomId, string expectedAuthorDisplayName)
        {
            TestParseMessageReturnData(messageId, roomId, expectedAuthorDisplayName, (x) => x.AuthorDisplayName);
        }

        [TestCaseSource("Generate_ParseMessage_AuthorId")]
        public void ParseMessage_AuthorId(int messageId, int roomId, int expectedAuthorId)
        {
            TestParseMessageReturnData(messageId, roomId, expectedAuthorId, (x) => x.AuthorId);
        }

        [TestCaseSource("Generate_ParseMessage_CurrentText")]
        public void ParseMessage_CurrentText(int messageId, int roomId, string expectedCurrentText)
        {
            TestParseMessageReturnData(messageId, roomId, expectedCurrentText, (x) => x.CurrentText);
        }

        [TestCaseSource("Generate_ParseMessage_IsCloseVoteRequest")]
        public void ParseMessage_IsCloseVoteRequest(int messageId, int roomId, bool expectedValue)
        {
            TestParseMessageReturnData(messageId, roomId, expectedValue, (x) => x.IsCloseVoteRequest);
        }

        [TestCaseSource("Generate_ParseMessage_MessageId")]
        public void ParseMessage_MessageId(int messageId, int roomId)
        {
            TestParseMessageReturnData(messageId, roomId, messageId, (x) => x.MessageId);
        }

        [TestCaseSource("Generate_ParseMessage_RawOneBoxName")]
        public void ParseMessage_RawOneBoxName(int messageId, int roomId, string expectedValue)
        {
            TestParseMessageReturnData(messageId, roomId, expectedValue, (x) => x.RawOneboxName);
        }

        [SetUp]
        public void Setup()
        {
            downloader = new PreDownloadedHtmlDownloader(TestContext.CurrentContext.TestDirectory);
            cs = new ChatScraper(downloader);
            testLocation = TestContext.CurrentContext.TestDirectory;
        }
        private static IEnumerable<TestCaseData> GenerateTestCases<TExpected>(string expectedKey)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var messageId in RegisteryAccessor.GetMessageIds(dir))
            {
                var data = RegisteryAccessor.GetRegisteryDataForMessage(dir, messageId);
                yield return new TestCaseData(data["MessageId"].Value<int>(), data["RoomId"].Value<int>(), data[expectedKey].Value<TExpected>());
            }
        }

        private void TestParseMessageReturnData<TActual>(int messageId, int roomId, TActual expected, Func<ParsedMessageData, TActual> getActual)
        {
            var parsedData = cs.ParseMessage(messageId, roomId);
            var actual = getActual(parsedData);
            Assert.AreEqual(expected, actual);
        }
    }
}
