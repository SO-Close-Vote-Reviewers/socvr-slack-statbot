using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot
{
    static class SettingsAccessor
    {
        /// <summary>
        /// Fetches the setting that is specified by the Key.
        /// First tries to get the value from an environment variable.
        /// If it doesn't exist, tries to get the value from the "settings.json" file
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

            if (File.Exists("settings.json"))
            {
                var content = File.ReadAllText("settings.json");
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                if (values.ContainsKey(key))
                {
                    return values[key].Parse<T>();
                }
            }

            throw new Exception("Unable to locate setting.");
        }
    }
}
