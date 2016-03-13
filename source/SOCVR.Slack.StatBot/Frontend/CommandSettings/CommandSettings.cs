using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.CommandSettings
{
    abstract class CommandSettings
    {
        public CommandSettings(string userMessage)
        {
            var match = Regex.Match(userMessage, GetCommandRegexPattern(), RegexOptions.Compiled | RegexOptions.CultureInvariant);

            if (!match.Success)
                throw new ArgumentException("userMessage does not conform to the command's regex.");

            ParseUserMessage(match);
        }

        protected abstract void ParseUserMessage(Match userMessageMatch);

        protected abstract string GetCommandRegexPattern();
    }
}
