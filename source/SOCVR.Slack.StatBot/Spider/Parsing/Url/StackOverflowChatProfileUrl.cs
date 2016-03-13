﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Spider.Parsing.Url
{
    class StackOverflowChatProfileUrl : StackOverflowChatUrl
    {
        public int ProfileId { get; private set; }

        public StackOverflowChatProfileUrl(int profileId)
            : base($"users/{profileId}")
        {
            ProfileId = profileId;
        }
    }
}
