using System;
using System.Collections.Generic;
using System.Linq;
using SOCVR.Slack.StatBot.Database;

namespace SOCVR.Slack.StatBot
{
    /// <summary>
    /// Checks if a given chat user is considered a "regular".
    /// </summary>
    class RegularChecker
    {
        /// <summary>
        /// Checks if a user (ID) is classified as "regular" based on the current
        /// rule set.
        /// </summary>
        /// <param name="userID">The user ID to check.</param>
        /// <returns>True if the user is a regular, otherwise false.</returns>
        public bool IsUserRegular(int userID)
        {
            //using (var db = new MessageStorage())
            //{
            //    var totalMsgCount = db.Messages.Count(x => x.UserId == userID);
            //    if (totalMsgCount < 1000) return false;

            //    var recentMsgs = db.Messages
            //        .Where(x => x.UserId == userID && (DateTime.UtcNow - x.PostedAt).TotalDays < 90);
            //    var msgsByDay = new Dictionary<DateTime, int>();

            //    foreach (var m in recentMsgs)
            //    {
            //        var date = m.PostedAt.Date;
            //        if (msgsByDay.ContainsKey(date))
            //        {
            //            msgsByDay[date]++;
            //        }
            //        else
            //        {
            //            msgsByDay[date] = 1;
            //        }
            //    }

            //    return msgsByDay.Values.Count(x => x > 9) > 14;
            //}

            throw new NotImplementedException();
        }
    }
}
