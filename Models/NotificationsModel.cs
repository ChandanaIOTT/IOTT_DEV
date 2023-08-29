using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    
    public class NotificationType
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public string Day { get; set; }
        public int MovieId { get; set; }
        public string MoviePoster { get; set; }
    }
    public class FCMNotification
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MoviePoster { get; set; }
    }

}