using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    class User
    {
        public int ProfileId { get; set; }

        /// <summary>
        /// The date and time the user joined Stack Overflow.
        /// </summary>
        public DateTimeOffset JoinedStackOverflowAt { get; set; }

        /// <summary>
        /// The date the user joined Stack Overflow Chat.
        /// </summary>
        public DateTimeOffset JoinedChatSystemAt { get; set; }

        public virtual List<UserAlias> Aliases { get; set; }
    }
}
