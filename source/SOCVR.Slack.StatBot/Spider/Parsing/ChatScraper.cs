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

        public void ParseAllChatMessages()
        {
            var roomIdsToParse = new[] { 41570, 90230 };

            using (var db = new MessageStorage())
            {
                foreach (var roomId in roomIdsToParse)
                {
                    //add the room if it doesn't exist
                    if (!db.Rooms.Any(x => x.RoomId == roomId))
                    {
                        var roomData = GetRoomData(roomId);
                        db.Rooms.Add(new Room
                        {
                            RoomId = roomData.RoomId,
                            Description = roomData.Description,
                            Name = roomData.Name
                        });
                        db.SaveChanges();
                    }

                    foreach (var date in DetermineDatesToParse(roomId))
                    {
                        foreach (var messageId in GetMessageIdsOnTranscriptPage(roomId, date))
                        {
                            Console.WriteLine($"{roomId} {date:yyyy-MM-dd} {messageId}");
                            var parsedInformation = ParseMessage(messageId, roomId, DateTime.Now);
                            SendParsedMessageDataToDatabase(parsedInformation);
                        }

                        //add to the database that we've parsed this page
                        db.ParsedTranscriptPages.Add(new ParsedTranscriptPage
                        {
                            Date = date,
                            RoomId = roomId,
                        });
                        db.SaveChanges();
                    }
                }
            }
        }

        private void SendParsedMessageDataToDatabase(ParsedMessageData messageData)
        {
            using (var db = new MessageStorage())
            {
                //if both the username and user id are empty, this is a deleted user. Just move on
                if (messageData.AuthorDisplayName == null && messageData.AuthorId == 0)
                {
                    return;
                }

                //check if this message was already added to the db
                if (db.Messages.Any(x => x.MessageId == messageData.MessageId))
                {
                    return;
                }

                var dbMessage = new Message();

                //check if the author exists
                if (!db.Users.Any(x => x.ProfileId == messageData.AuthorId))
                {
                    db.Users.Add(new User
                    {
                        ProfileId = messageData.AuthorId,
                        JoinedChatSystemAt = DateTimeOffset.Now, //will change later
                        JoinedStackOverflowAt = DateTimeOffset.Now
                    });
                    db.SaveChanges();
                }

                //check if user alias exists
                if (!db.UserAliases.Any(x => x.UserId == messageData.AuthorId && x.DisplayName == messageData.AuthorDisplayName))
                {
                    db.UserAliases.Add(new UserAlias
                    {
                        UserId = messageData.AuthorId,
                        DisplayName = messageData.AuthorDisplayName
                    });
                    db.SaveChanges();
                }

                dbMessage.Author = db.UserAliases
                    .Where(x => x.UserId == messageData.AuthorId)
                    .Where(x => x.DisplayName == messageData.AuthorDisplayName)
                    .Single();

                dbMessage.CurrentMarkdownContent = messageData.CurrentMarkdownContent;
                dbMessage.CurrentText = messageData.CurrentText;
                dbMessage.InitialRevisionTs = messageData.InitialRevisionTs;
                dbMessage.IsCloseVoteRequest = messageData.IsCloseVoteRequest;
                dbMessage.MessageId = messageData.MessageId;

                //dbMessage.MessageRevisions =

                dbMessage.OneboxType = Enum.GetValues(typeof(OneboxType))
                    .OfType<OneboxType?>()
                    .SingleOrDefault(x => x.ToString() == messageData.RawOneboxName);

                dbMessage.PlainTextLinkCount = messageData.PlainTextLinkCount;
                dbMessage.RoomId = messageData.RoomId;
                dbMessage.StarCount = messageData.StarCount;
                dbMessage.Tags = messageData.TagsCount;

                db.Messages.Add(dbMessage);
                db.SaveChanges();
            }
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
            var chatFirstDay = new DateTime(2013, 11, 19); //change this value

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

        private IEnumerable<int> GetMessageIdsOnTranscriptPage(int roomId, DateTime date)
        {
            var transcriptUrl = new StackOverflowChatTranscriptUrl(downloader, roomId, date);
            var html = CQ.Create(transcriptUrl.DownloadHtml());

            var messageNodes = html["#main #transcript .monologue .messages .message"];

            foreach (var messageNode in messageNodes)
            {
                var messageIdRaw = messageNode.Id;
                var messageId = messageIdRaw
                    .Replace("message-", "")
                    .Parse<int>();

                yield return messageId;
            }
        }

        /// <summary>
        /// Parse the message and save info to the database.
        /// </summary>
        /// <param name="messageId"></param>
        public ParsedMessageData ParseMessage(int messageId, int roomId, DateTimeOffset currentDate)
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
            extractedMessageData.CurrentMarkdownContent = extractedMessageData.Revisions[0].MessageMarkDown;
            extractedMessageData.InitialRevisionTs = GetInitialTimestamp(historyPageHtml, currentDate);

            // Bot-specific message meta data.
            extractedMessageData.IsCloseVoteRequest = DetermineIfMessageIsCloseVoteRequest(currentVersionMessageHtml);
            extractedMessageData.RawOneboxName = currentVersionMessageHtml
                .Find(".content")
                .Children(".onebox")
                .SingleOrDefault()
                ?.Classes
                ?.Last();

            if (string.IsNullOrEmpty(extractedMessageData.RawOneboxName))
            {
                extractedMessageData.PlainTextLinkCount = currentVersionMessageHtml[".content a"].Count(e => e.ChildElements.Count() == 0);
                extractedMessageData.TagsCount = currentVersionMessageHtml[".ob-post-tag"].Count();
            }

            return extractedMessageData;
        }

        private DateTimeOffset GetInitialTimestamp(CQ dom, DateTimeOffset cd)
        {
            var tsStr = dom[".monologue .timestamp"].Last().Text();
            var m = Regex.Match(tsStr, @"^([A-Za-z]{3,3} )?(\d+ )?('\d+ )?(\d+):(\d+) (AM|PM)$");

            if (!m.Success)
            {
                throw new Exception("Invalid date specified.");
            }

            if (string.IsNullOrEmpty(m.Groups[1].Value))
            {
                return DateTimeOffset.Parse(tsStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).AddDays(-((DateTime.UtcNow.Date - cd.Date).Days));
            }

            if (m.Groups[1].Value.Trim() == "yst")
            {
                return DateTimeOffset.Parse(tsStr.Remove(0, 4), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).AddDays(-((DateTime.UtcNow.Date - cd.Date).Days + 1));
            }

            if (Regex.IsMatch(m.Groups[1].Value, @"Mon|Tue|Wed|Thu|Fri|Sat|Sun"))
            {
                var dt = cd.ToUniversalTime().Date;
                for (var i = 0; i < 7; i++)
                {
                    dt = dt.AddDays(-1);

                    if (dt.DayOfWeek.ToString().StartsWith(m.Groups[1].Value.Trim())) break;
                }
                dt = dt.AddHours(int.Parse(m.Groups[4].Value) + (m.Groups[6].Value == "PM" ? 12 : 0));
                dt = dt.AddMinutes(int.Parse(m.Groups[5].Value));

                return new DateTimeOffset(dt, TimeSpan.Zero);
            }

            var yy = string.IsNullOrEmpty(m.Groups[3].Value) ? "" : "yy ";
            var dd = m.Groups[2].Length - 1 == 2 ? "dd" : "d";
            var h = m.Groups[4].Length == 2 ? "h" : "";
            var pattern = $"MMM {dd} {yy}{h}h:mm tt";

            return DateTimeOffset.ParseExact(tsStr.Replace("'", ""), pattern, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
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
                    MessageMarkDown = RemoveSaidEdited(revDom.Find(".content").Text(), false)
                });
            }

            return revs;
        }

        private static string RemoveSaidEdited(string t, bool html)
        {
            var str = t.Trim();

            if (html)
            {
                if (str.StartsWith("<b>said:</b>"))
                {
                    return str.Remove(0, 13);
                }
                if (str.StartsWith("<b>edited:</b>"))
                {
                    return str.Remove(0, 15);
                }
            }
            else
            {
                if (str.StartsWith("said:"))
                {
                    return str.Remove(0, 6);
                }
                if (str.StartsWith("edited:"))
                {
                    return str.Remove(0, 8);
                }
            }

            return str;
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

        private ParsedRoomData GetRoomData(int roomId)
        {
            var roomUrl = new StackOverflowChatRoomInfoUrl(downloader, roomId);
            var roomHtml = CQ.Create(roomUrl.DownloadHtml());

            var room = new ParsedRoomData();
            room.RoomId = roomId;
            room.Description = roomHtml.Find($"#room-{roomId} p").First().Text();
            room.Name = roomHtml.Find($"#room-{roomId} h1").First().Text();

            return room;
        }

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
    }
}
