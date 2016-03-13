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
            using (var db = new MessageStorage())
            {
                var userMessages = db.Messages
                    .Where(x => x.OriginalPoster.User.ProfileId == userID);

                // Does the user have at least 1000 all-time messages?
                var totalMessageCount = userMessages.Count();
                if (totalMessageCount < 1000) return false;

                // In the last 90 days, has the user made at least 10 messages per day for 15 days each 30 day period?

                foreach (var periodStartDate in EnumPeriodStartDates())
                {
                    var periodEndDate = periodStartDate.AddDays(30);

                    //fetch all messages from this time period
                    var periodMessages = userMessages
                        .Where(x => x.InitialRevisionTs >= periodStartDate)
                        .Where(x => x.InitialRevisionTs <= periodEndDate);

                    //group the messages by their day
                    var numberOfActiveDays = periodMessages
                        .GroupBy(x => x.InitialRevisionTs.Date)
                        .Where(x => x.Count() >= 10) //only selected dates with 10 or more messages in that day
                        .Count(); //get the number of days in this period with 10 or more messages each day

                    if (numberOfActiveDays < 15)
                    {
                        //user was not active enough in that period
                        return false;
                    }
                }

                //passed all the tests
                return true;
            }
        }

        private IEnumerable<DateTime> EnumPeriodStartDates()
        {
            //start with today's UTC date
            var today = DateTimeOffset.UtcNow.Date;

            //now go back 30, 60, and 90 days
            //which is 30 * (1,2,3)

            var dates = Enumerable.Range(1, 3)
                .Select(x => x * 30)
                .Select(x => today.AddDays(-x));

            return dates;
        }
    }
}
