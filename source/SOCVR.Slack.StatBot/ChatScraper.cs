using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot
{
    /// <summary>
    /// 
    /// </summary>
    class ChatScraper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">The UTC date to fetch data about.</param>
        /// <returns></returns>
        public List<ChatMessageInfo> GetMessagesForDate(DateTime date, int startHour, int endHour)
        {
            var socvrTranscriptUrl = CreateTranscriptUrl(41570, date);

            CQ transcriptHtml = CQ.CreateFromUrl(socvrTranscriptUrl);

            var allMessages = transcriptHtml["#transcript .message"];

            var parsedMessages = new List<ChatMessageInfo>();

            foreach (var message in allMessages)
            {
                var messageEntry = new ChatMessageInfo();

                messageEntry.UserName = message.Cq()
                    .Parent()
                    .Siblings(".signature")
                    .Find(".username a")
                    .Text();

                messageEntry.HasLinks = message.Cq().Find(".content").Has("a").Any();
                messageEntry.IsCloseVoteRequest = false;
                messageEntry.IsImage = message.Cq().Find(".content .onebox.ob-image").Any();
                messageEntry.IsOneBoxed = message.Cq().Find(".content .onebox:not(.ob-image)").Any();

                var starContainer = message.Cq().Find(".stars.vote-count-container");

                if (starContainer.Any())
                {
                    //there is at least one star on the message
                    var rawStarCount = starContainer.Find(".times").Text();

                    //if it's an empty string then the count is one. Else parse the number
                    messageEntry.StarCount = rawStarCount == ""
                        ? 1
                        : rawStarCount.Parse<int>();
                }
                else
                {
                    //there are no stars
                    messageEntry.StarCount = 0;
                }

                parsedMessages.Add(messageEntry);
            }

            return parsedMessages;
        }

        private string CreateTranscriptUrl(int roomId, DateTime date)
        {
            return CreateTranscriptUrl(roomId, date, 0, 24);
        }

        private string CreateTranscriptUrl(int roomId, DateTime date, int startHour, int endHour)
        {
            //do some error checking

            //date given is in the future
            if (date.Date > DateTimeOffset.UtcNow.Date)
            {
                throw new ArgumentException("Date is in the future.");
            }

            if (startHour > endHour)
            {
                throw new ArgumentException("Start hour is after the end hour");
            }

            var baseUrl = new Uri("http://chat.stackoverflow.com/transcript/");

            var roomAndDateSection = new int[]
            {
                 roomId,
                 date.Year,
                 date.Month,
                 date.Day
            }.ToCSV("/") + "/";

            //add in the room and date info
            var url = new Uri(baseUrl, roomAndDateSection);

            //add in the start and end time info
            url = new Uri(url, "{0}-{1}".FormatInline(startHour, endHour));
            return url.ToString();
        }
    }
}
