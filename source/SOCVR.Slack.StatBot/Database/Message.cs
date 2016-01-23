using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    class Message
    {
        /// <summary>
        /// The id number for the message as set by Stack Overflow.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// The user (by display name) who posted the message.
        /// </summary>
        public UserAlias OriginalPoster { get; set; }
        public int OriginalPosterId { get; set; }

        /// <summary>
        /// The room this message was posted in.
        /// </summary>
        public Room Room { get; set; }
        public int RoomId { get; set; }

        /// <summary>
        /// The text of the latest revision of the message.
        /// </summary>
        public string CurrentText { get; set; }

        /// <summary>
        /// The date and time of the initial message posting.
        /// </summary>
        public DateTimeOffset InitialRevisionTs { get; set; }
        
        /// <summary>
        /// If the message only a one-box, describe the type.
        /// </summary>
        public OneboxType? OneboxType { get; set; }

        /// <summary>
        /// The number of plain text links in the mesage. Note that this excludes any clickable one-box clickable items, or links within oneboxes.
        /// </summary>
        public int PlainTextLinkCount { get; set; }

        /// <summary>
        /// The number of [tag:...] in the message
        /// </summary>
        public int Tags { get; set; }

        /// <summary>
        /// Tells if the message is a close vote request.
        /// </summary>
        public bool IsCloseVoteRequest { get; set; }

        /// <summary>
        /// The number of stars given to the message.
        /// </summary>
        public int StarCount { get; set; }

        public virtual List<MessageRevision> MessageRevisions { get; set; }
    }
}
