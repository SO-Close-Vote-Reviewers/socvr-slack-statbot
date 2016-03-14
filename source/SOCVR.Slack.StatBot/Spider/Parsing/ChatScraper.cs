using CsQuery;
using Microsoft.Data.Entity;
using SOCVR.Slack.StatBot.Database;
using SOCVR.Slack.StatBot.Frontend;
using SOCVR.Slack.StatBot.Spider.Download;
using SOCVR.Slack.StatBot.Spider.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using TCL.Extensions;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SOCVR.Slack.StatBot.Spider.Parsing
{
    public class ChatScraper
    {
        private IHtmlDownloader downloader;

        public ChatScraper(IHtmlDownloader downloader)
        {
            this.downloader = downloader;
        }

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

        //public void ParseTranscriptPage(StackOverflowChatTranscriptUrl url)
        //{
        //    var documentHtml = url.DownloadHtml();
        //    var parsedDoc = CQ.Create(documentHtml);

        //    using (var db = new MessageStorage())
        //    {
        //        var monologues = parsedDoc.Find("#transcript > .monologue");

        //        foreach (var monologue in monologues)
        //        {
        //            foreach (var message in monologue.Cq().Find(".messages .message"))
        //            {
        //                var messageId = message.Attributes["id"].Replace("message-", "").Parse<int>();

        //                //have we already parsed this message?
        //                var messageAlreadyParsed = db.Messages.Any(x => x.MessageId == messageId);

        //                if (!messageAlreadyParsed)
        //                {
        //                    ParseMessage(messageId, url);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Parse the message and save info to the database.
        /// </summary>
        /// <param name="messageId"></param>
        public ParsedMessageData ParseMessage(int messageId, int roomId)
        {
            //get the history page
            var historyUrl = new StackOverflowChatMessageHistoryUrl(downloader, messageId);
            var historyPageHtml = CQ.Create(historyUrl.DownloadHtml());

            var extractedMessageData = new ParsedMessageData();

            var currentVersionMonologue = historyPageHtml.Find("#content .monologue").First();
            var currentVersionMessageHtml = currentVersionMonologue.Find($"#message-{messageId}");

            // Basic message meta info.
            extractedMessageData.RoomId = roomId;
            extractedMessageData.MessageId = messageId;
            extractedMessageData.AuthorId = currentVersionMonologue.Single().Classes.Last().Replace("user-", "").Parse<int>();
            extractedMessageData.AuthorDisplayName = currentVersionMonologue.Find(".signature .username a").Attr("title");
            extractedMessageData.StarCount = GetStarsOrPins(historyPageHtml);
            extractedMessageData.Revisions = GetRevisions(historyPageHtml);
            extractedMessageData.CurrentText = currentVersionMessageHtml.Find(".content").Text().Trim();
            extractedMessageData.CurrentMarkdownContent = extractedMessageData.Revisions.Last().MessageText;
            extractedMessageData.InitialRevisionTs = GetInitialTimestamp(historyPageHtml);

            // Bot-specific message meta data.
            extractedMessageData.PlainTextLinkCount = currentVersionMessageHtml.Find(".content").Children("a").Count();
            extractedMessageData.IsCloseVoteRequest = DetermineIfMessageIsCloseVoteRequest(currentVersionMessageHtml);
            extractedMessageData.RawOneboxName = currentVersionMessageHtml
                .Find(".content")
                .Children(".onebox")
                .SingleOrDefault()
                ?.Classes
                ?.Last();

            //extractedMessageData.TagsCount

            return extractedMessageData;
        }

        private DateTime GetInitialTimestamp(CQ dom)
        {
            var tsStr = dom[".monologue .timestamp"].Last().Text();
            var m = Regex.Match(tsStr, @"^([A-Za-z]{3,3} )?(\d+ )?('\d+ )?(\d+:\d+) (AM|PM)$");

            if (!m.Success)
            {
                throw new Exception("Invalid date specified.");
            }

            if (string.IsNullOrEmpty(m.Groups[1].Value))
            {
                return DateTime.Parse(tsStr);
            }

            if (m.Groups[1].Value == "yst")
            {
                return DateTime.Parse(tsStr.Remove(0, 4)).AddDays(-1);
            }

            if (Regex.IsMatch(m.Groups[1].Value, @"Mon|Tue|Wed|Thu|Fri|Sat|Sun"))
            {
                var dt = DateTime.UtcNow.Date;
                for (var i = 0; i < 7; i++)
                {
                    dt = dt.AddDays(-1);

                    if (dt.DayOfWeek.ToString().StartsWith(m.Groups[1].Value)) break;
                }
                dt.AddHours(m.Groups[4].Value.Parse<int>() + (m.Groups[6].Value == "PM" ? 12 : 0));
                dt.AddMinutes(m.Groups[5].Value.Parse<int>());

                return DateTime.Parse(tsStr);
            }

            var yy = string.IsNullOrEmpty(m.Groups[3].Value) ? "" : "yy ";
            var h = m.Groups[4].Length == 2 ? "h" : "";
            var pattern = $"MMM dd {yy}{h}h:mm tt";

            return DateTime.ParseExact(tsStr.Replace("'", ""), pattern, CultureInfo.InvariantCulture);
        }

        private List<ParsedMessageRevision> GetRevisions(CQ dom)
        {
            var revs = new List<ParsedMessageRevision>();
            var revsDom = dom[".monologue"];

            for (var i = 1; i < revsDom.Length; i++)
            {
                var revDom = revsDom[i].Cq();

                revs.Add(new ParsedMessageRevision
                {
                    RevisionNumber = (revsDom.Length - 1) - i, // 0-based index.
                    AuthorId = revsDom[i].Classes.Last().Replace("user-", "").Parse<int>(),
                    DisplayName = revDom.Find(".signature .username a").Attr("title"),
                    MessageText = RemoveSaidEdited(revDom.Find(".content").Text(), true),

                    //TODO: There's no difference in output between this and the line above.
                    //MessageHtml = RemoveSaidEdited(revDom.Find(".content").Html(), false)
                });
            }

            return revs;
        }

        private static string RemoveSaidEdited(string t, bool html)
        {
            if (html)
            {
                if (t.StartsWith("<b>said:</b>"))
                {
                    return t.Remove(0, 13);
                }
                if (t.StartsWith("<b>edited:</b>"))
                {
                    return t.Remove(0, 15);
                }
            }
            else
            {
                if (t.StartsWith("said:"))
                {
                    return t.Remove(0, 6);
                }
                if (t.StartsWith("edited:"))
                {
                    return t.Remove(0, 8);
                }
            }

            return t;
        }

        private int GetStarsOrPins(CQ dom, bool stars = true)
        {
            var starSpan = dom[$"{(stars ? ".stars" : ".owner-star")}.vote-count-container"];

            if (starSpan != null && starSpan.Length != 0)
            {
                var c = starSpan[".times"]?.Text();

                return string.IsNullOrEmpty(c) ? 1 : int.Parse(c);
            }
            else
            {
                if (!stars) return 0;

                return GetStarsOrPins(dom, false);
            }
        }

        //private Room GetRoomData(int roomId)
        //{
        //    var roomUrl = new StackOverflowChatRoomInfoUrl(roomId);
        //    var roomHtml = CQ.Create(roomUrl.DownloadHtml());

        //    var room = new Room();
        //    room.RoomId = roomId;
        //    room.Description = roomHtml.Find($"#room-{roomId} p").First().Text();
        //    room.Name = roomHtml.Find($"#room-{roomId} h1").First().Text();

        //    return room;
        //}

        private OneboxType? GetOneboxTypeForMessage(CQ messageHtml)
        {
            var contentElement = messageHtml.Find(".content");
            var oneboxElement = contentElement.Children(".onebox").SingleOrDefault();

            if (oneboxElement == null)
                return null;

            var rawOneboxType = oneboxElement.Classes.Last();

            var matchingOneboxType = Enum.GetValues(typeof(OneboxType))
                .OfType<OneboxType>()
                .Where(x => x.GetAttributeValue<CssClassValueAttribute>().ClassName == rawOneboxType)
                .Select(x => (OneboxType?)x)
                .SingleOrDefault();

            return matchingOneboxType;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="date">The UTC date to fetch data about.</param>
        ///// <returns></returns>
        //public List<ChatMessageInfo> GetMessagesForDate(DateTime date, int startHour, int endHour)
        //{
        //    var parsedMessages = new List<ChatMessageInfo>();

        //    var socvrTranscriptUrl = CreateTranscriptUrl(41570, date);
        //    var graveyardTranscriptUrl = CreateTranscriptUrl(90230, date);

        //    parsedMessages.AddRange(ExtractMessagesFromTranscriptPage(socvrTranscriptUrl));
        //    parsedMessages.AddRange(ExtractMessagesFromTranscriptPage(graveyardTranscriptUrl));

        //    return parsedMessages;
        //}

        //private List<ChatMessageInfo> ExtractMessagesFromTranscriptPage(string transcriptUrl)
        //{
        //    List<ChatMessageInfo> parsedMessages = new List<ChatMessageInfo>();
        //    CQ transcriptHtml = CQ.CreateFromUrl(transcriptUrl);

        //    var allMessages = transcriptHtml["#transcript .message"];

        //    foreach (var message in allMessages)
        //    {
        //        var messageEntry = new ChatMessageInfo();

        //        messageEntry.UserName = message.Cq()
        //            .Parent()
        //            .Siblings(".signature")
        //            .Find(".username a")
        //            .Text();

        //        messageEntry.IsCloseVoteRequest = DetermineIfMessageIsCloseVoteRequest(message.Cq().Find(".content"));
        //        messageEntry.HasLinks = message.Cq().Find(".content").Has("a").Any() && !messageEntry.IsCloseVoteRequest;
        //        messageEntry.IsImage = message.Cq().Find(".content .onebox.ob-image").Any();
        //        messageEntry.IsOneBoxed = message.Cq().Find(".content .onebox:not(.ob-image)").Any();

        //        var starContainer = message.Cq().Find(".stars.vote-count-container");

        //        if (starContainer.Any())
        //        {
        //            // There is at least one star on the message.
        //            var rawStarCount = starContainer.Find(".times").Text();

        //            // If it's an empty string then the count is one. Else parse the number.
        //            messageEntry.StarCount = rawStarCount == ""
        //                ? 1
        //                : rawStarCount.Parse<int>();
        //        }
        //        else
        //        {
        //            // There are no stars.
        //            messageEntry.StarCount = 0;
        //        }

        //        parsedMessages.Add(messageEntry);
        //    }

        //    return parsedMessages;
        //}

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

        //private string CreateTranscriptUrl(int roomId, DateTime date)
        //{
        //    return CreateTranscriptUrl(roomId, date, 0, 24);
        //}

        //private string CreateTranscriptUrl(int roomId, DateTime date, int startHour, int endHour)
        //{
        //    // Do some error checking.

        //    // Date given is in the future.
        //    if (date.Date > DateTimeOffset.UtcNow.Date)
        //    {
        //        throw new ArgumentException("Date is in the future.");
        //    }

        //    if (startHour > endHour)
        //    {
        //        throw new ArgumentException("Start hour is after the end hour.");
        //    }

        //    var baseUrl = new Uri("http://chat.stackoverflow.com/transcript/");

        //    var roomAndDateSection = new int[]
        //    {
        //         roomId,
        //         date.Year,
        //         date.Month,
        //         date.Day
        //    }.ToCSV("/") + "/";

        //    //add in the room and date info
        //    var url = new Uri(baseUrl, roomAndDateSection);

        //    //add in the start and end time info
        //    url = new Uri(url, $"{startHour}-{endHour}");
        //    return url.ToString();
        //}
    }
}
