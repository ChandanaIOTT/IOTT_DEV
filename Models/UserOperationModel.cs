using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class UserOperationModel
    {
        public int Id { get; set; }

        public string Issue { get; set; }

        public string Number { get; set; }

    }
    public class UserFollowModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string UserId { get; set; }
        public string FollowerId { get; set; }


    }
    public class UserProfile
    {
        public string Username { get; set; }
        public string follower { get; set; }
        public string following { get; set; }
    }
}