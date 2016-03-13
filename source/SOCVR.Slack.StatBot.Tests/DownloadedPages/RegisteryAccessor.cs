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
            var registeryPath = Path.Combine(testLocation, "DownloadedPages", "registery.json");
            var registeryFileContents = File.ReadAllText(registeryPath);
            var array = JArray.Parse(registeryFileContents);

            var matchingRecord = array
                .Where(x => x["Url"].Value<string>() == url.ToString())
                .SingleOrDefault();

            if (matchingRecord == null)
                return null;

            return Path.Combine(testLocation, matchingRecord["FilePath"].Value<string>());
        }
    }
}
