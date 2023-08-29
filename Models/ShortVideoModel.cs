using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class ShortVideoModel
    {
        public int ShortId { get; set; }
        public int MovieId { get; set; }
        public string Description { get; set; }
        public string ShortVideo { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        

    }
    public class FavShortModel
    {
        public int ShortId { get; set; }
        public string ShortVideo { get; set; }

    }
    public class ShortLikeModel
    {

        public int UserId { get; set; }
        public int ShortId { get; set; }

        public int Likes { get; set; }

    }
    public class PersonalShort
    {
        public int Id { get; set; }

        public string Shorts { get; set; }

        public string Description { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Time { get; set; }


    }
    public class GetShort
    {
        public int ShortId { set; get; }
        public int MovieId { set; get; }
        public string Description { get; set; }
        public string Short { set; get; }
    }

    public class ShortVideoComment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ShortVideoId { get; set; }
        public  string Shortcomment { get; set; }

        public string Date { get; set; }

    }
    public class ShortCommentslikes
    {
        public int UserId { get; set; }

        public int ShortVideoId { get; set; }
        public int ShortcommentLike { get; set; }

        public int ShortcommentUnlike { get; set; }

    }
    public class ShortsCommentslikesUnlikes
    {
        public int Id { get; set; }
        public int ShortscommentLike { get; set; }
        public int ShortscommentUnlike { get; set; }


    }
    public class ShortComments
    {
        public string Comments { get; set; }
        public string Date { get; set; }
        public byte[] Profilepic { get; set; }
        public string Name { get; set; }

    }
   
    public class Sc
    {
        public int ShortId { get; set; }
 
        public int UserId { get; set; }



    }
    public class TrailerModel
    {
        public int UserId { get; set; }
        public String TrailerName { get; set; }
    }
}