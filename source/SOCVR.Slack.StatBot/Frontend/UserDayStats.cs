namespace SOCVR.Slack.StatBot.Frontend
{
    class UserDayStats
    {
        public string Username { get; set; }
        public int TotalMessages { get; set; }
        public int CloseRequests { get; set; }
        public int Links { get; set; }
        public int Images { get; set; }
        public int OneBoxes { get; set; }
        public int StarredMessages { get; set; }
        public int StarsGained { get; set; }
    }
}
