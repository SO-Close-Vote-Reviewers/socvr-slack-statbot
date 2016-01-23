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

        public virtual List<Message> PostedMessages { get; set; }
    }
}
