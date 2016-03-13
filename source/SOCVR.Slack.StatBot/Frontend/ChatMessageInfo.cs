namespace SOCVR.Slack.StatBot
{
    /// <summary>
    /// Holds summary info about a chat message
    /// </summary>
    class ChatMessageInfo
    {
        public string UserName { get; set; }

        /// <summary>
        /// Tells if this message is a request for post closure.
        /// </summary>
        public bool IsCloseVoteRequest { get; set; }

        /// <summary>
        /// Tells if the message contains a link. Excludes close vote requests, images, and one-boxed messages.
        /// </summary>
        public bool HasLinks { get; set; }

        /// <summary>
        /// Tells if the message contains an inline image (and only the image).
        /// </summary>
        public bool IsImage { get; set; }

        /// <summary>
        /// Tells if the message is one-boxed. Excludes pure images.
        /// </summary>
        public bool IsOneBoxed { get; set; }

        /// <summary>
        /// Tells the number of stars this message received.
        /// </summary>
        public int StarCount { get; set; }
    }
}
