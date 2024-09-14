using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Hub
{
    public class Comment
    {
        public int CommentID { get; set; }
        public string CommentText { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string NotificationPreferences { get; set; }
    }
}
