using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    class UserAlias
    {
        /// <summary>
        /// The unique user attached to this display name.
        /// </summary>
        public User User { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// The non-unique shown as the display name for the user.
        /// </summary>
        public string DisplayName { get; set; }

        public virtual List<Message> Messages { get; set; }
        public virtual List<MessageRevision> MessageRevisions { get; set; }
    }
}
