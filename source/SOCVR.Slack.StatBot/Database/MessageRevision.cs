using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    /// <summary>
    /// A message revision. The inital posted message is a revision.
    /// </summary>
    class MessageRevision
    {
        public Message Message { get; set; }
        public long MessageId { get; set; }

        public int RevisionNumber { get; set; }

        /// <summary>
        /// The text of this message at the revision.
        /// </summary>
        public string Text { get; set; }

        public DateTimeOffset RevisionMadeAt { get; set; }

        public UserAlias RevisionAuthor { get; set; }
        public int RevisionAuthorId { get; set; }
    }
}
