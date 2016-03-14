using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Tests.DownloadedPages
{
    static class RegisteryAccessor
    {
        public static string GetFile(string testLocation, Uri url)
        {
            var array = LoadRegisteryArray(testLocation);

            var matchingRecord = array
                .Where(x => x["Url"].Value<string>() == url.ToString())
                .SingleOrDefault();

            if (matchingRecord == null)
                return null;

            return Path.Combine(testLocation, matchingRecord["FilePath"].Value<string>());
        }

        private static JArray LoadRegisteryArray(string testLocation)
        {
            var registeryPath = Path.Combine(testLocation, "DownloadedPages", "registery.json");
            var registeryFileContents = File.ReadAllText(registeryPath);
            var array = JArray.Parse(registeryFileContents);
            return array;
        }

        public static JObject GetRegisteryDataForMessage(string testLocation, int messageId)
        {
            var array = LoadRegisteryArray(testLocation);

            var item = array
                .Where(x => x["Type"].Value<string>() == "Message")
                .Where(x => x["MessageId"].Value<int>() == messageId)
                .SingleOrDefault();

            return (JObject)item;
        }

        public static IEnumerable<int> GetMessageIds(string testLocation)
        {
            var array = LoadRegisteryArray(testLocation);

            var messageIds = array
                .Where(x => x["Type"].Value<string>() == "Message")
                .Select(x => x["MessageId"].Value<int>());

            return messageIds;
        }
    }
}
