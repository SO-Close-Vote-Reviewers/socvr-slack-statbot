using MargieBot.Responders;
using MargieBot.Models;

namespace SOCVR.Slack.StatBot.Responders
{
    class HelpResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            return
                context.Message.MentionsBot &&
                !context.Message.User.IsSlackbot &&
                context.Message.Text.Contains("help");
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage()
            {
                Text = @"SOCVR Stat Slackbot, developed by gunr2171. Usage is as follows:
```sc stats [filter] <date> [hourStart-hourEnd]```
`Filter` is optional. Leave out to show the summary table, or choose from one of the following: totals, cv-pls, links, moved, one-box, stars
`Date` can be an ISO format date (`yyyy-MM-dd`), 'today', 'yesterday', or 'X days ago'.
The hour can be specified, use numbers 0-24."
            };
        }
    }
}
