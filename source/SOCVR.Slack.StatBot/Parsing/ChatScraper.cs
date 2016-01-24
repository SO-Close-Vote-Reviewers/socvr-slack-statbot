using CsQuery;
using SOCVR.Slack.StatBot.Database;
using SOCVR.Slack.StatBot.Parsing.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;

namespace SOCVR.Slack.StatBot
{
    /// <summary>
    /// 
    /// </summary>
    class ChatScraper
    {
        /// <summary>
        /// Returns a collection of dates between and including a start and endpoint.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> EnumDatesInRange(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
                throw new ArgumentException("Start date is after end date");

            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                yield return currentDate;
                currentDate = currentDate.AddDays(1);
            }
        }

        private IEnumerable<DateTime> DetermineDatesToParse(int roomId)
        {
            var chatFirstDay = new DateTime(2013, 01, 01); //change this value

            var allDays = EnumDatesInRange(chatFirstDay, DateTimeOffset.UtcNow.Date);

            using (var db = new MessageStorage())
            {
                var pagesAlreadyParsed = db.ParsedTranscriptPages
                    .Where(x => x.RoomId == roomId)
                    .Select(x => x.Date.Date)
                    .ToList();

                var pagesToParse = allDays.Except(pagesAlreadyParsed);
                return pagesToParse;
            }
        }

        public Message ParseTranscriptPage(StackOverflowChatTranscriptUrl url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">The UTC date to fetch data about.</param>
        /// <returns></returns>
        public List<ChatMessageInfo> GetMessagesForDate(DateTime date, int startHour, int endHour)
        {
            var parsedMessages = new List<ChatMessageInfo>();

            var socvrTranscriptUrl = CreateTranscriptUrl(41570, date);
            var graveyardTranscriptUrl = CreateTranscriptUrl(90230, date);

            parsedMessages.AddRange(ExtractMessagesFromTranscriptPage(socvrTranscriptUrl));
            parsedMessages.AddRange(ExtractMessagesFromTranscriptPage(graveyardTranscriptUrl));

            return parsedMessages;
        }

        private List<ChatMessageInfo> ExtractMessagesFromTranscriptPage(string transcriptUrl)
        {
            List<ChatMessageInfo> parsedMessages = new List<ChatMessageInfo>();
            CQ transcriptHtml = CQ.CreateFromUrl(transcriptUrl);

            var allMessages = transcriptHtml["#transcript .message"];

            foreach (var message in allMessages)
            {
                var messageEntry = new ChatMessageInfo();

                messageEntry.UserName = message.Cq()
                    .Parent()
                    .Siblings(".signature")
                    .Find(".username a")
                    .Text();

                messageEntry.IsCloseVoteRequest = DetermineIfMessageIsCloseVoteRequest(message.Cq().Find(".content"));
                messageEntry.HasLinks = message.Cq().Find(".content").Has("a").Any() && !messageEntry.IsCloseVoteRequest;
                messageEntry.IsImage = message.Cq().Find(".content .onebox.ob-image").Any();
                messageEntry.IsOneBoxed = message.Cq().Find(".content .onebox:not(.ob-image)").Any();

                var starContainer = message.Cq().Find(".stars.vote-count-container");

                if (starContainer.Any())
                {
                    // There is at least one star on the message.
                    var rawStarCount = starContainer.Find(".times").Text();

                    // If it's an empty string then the count is one. Else parse the number.
                    messageEntry.StarCount = rawStarCount == ""
                        ? 1
                        : rawStarCount.Parse<int>();
                }
                else
                {
                    // There are no stars.
                    messageEntry.StarCount = 0;
                }

                parsedMessages.Add(messageEntry);
            }

            return parsedMessages;
        }

        private bool DetermineIfMessageIsCloseVoteRequest(CQ messageContentsNode)
        {
            //message contains "cv-pls", "cv-plz", or "close", and has a link

            var triggerPhrases = new[]
            {
                "cv-pls",
                "cv-plz",
                "close"
            };

            var text = messageContentsNode.Text();

            var hasTriggerPhrase = triggerPhrases
                .Select(x => text.Contains(x))
                .Any(x => x);

            var hasLink = messageContentsNode.Find("a").Any();

            return hasTriggerPhrase && hasLink;
        }

        private string CreateTranscriptUrl(int roomId, DateTime date)
        {
            return CreateTranscriptUrl(roomId, date, 0, 24);
        }

        private string CreateTranscriptUrl(int roomId, DateTime date, int startHour, int endHour)
        {
            // Do some error checking.

            // Date given is in the future.
            if (date.Date > DateTimeOffset.UtcNow.Date)
            {
                throw new ArgumentException("Date is in the future.");
            }

            if (startHour > endHour)
            {
                throw new ArgumentException("Start hour is after the end hour.");
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
            url = new Uri(url, $"{startHour}-{endHour}");
            return url.ToString();
        }
    }
}
