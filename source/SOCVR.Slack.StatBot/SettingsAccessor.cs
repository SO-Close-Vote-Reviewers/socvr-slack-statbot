using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot
{
    static class SettingsAccessor
    {
        /// <summary>
        /// Fetches the setting that is specifed by the Key.
        /// First tries to get the value from an environment variable.
        /// If it doesn't exist, tries to get the value from the the command line arguments.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSetting<T>(string key)
        {
            var envValue = Environment.GetEnvironmentVariable(key);

            if (!envValue.IsNullOrWhiteSpace())
            {
                return envValue.Parse<T>();
            }

            var allCommandLineArgs = Environment.GetCommandLineArgs();

            var cmdEntries = allCommandLineArgs
                .Skip(1)
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

            var hasCmdEntry = cmdEntries.ContainsKey(key);

            if (hasCmdEntry)
            {
                return cmdEntries[key].Parse<T>();
            }

            throw new Exception("Unable to locate setting.");
        }
    }
}
