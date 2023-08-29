using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{

    public class VideoModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MoviePoster { get; set; }
        public string WebPoster { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string IMDbRating { get; set; }
        public string Language { get; set; }
        public string PriceOfTheMovie { get; set; }
        public string Certificate { get; set; }
        public string Category { get; set; }
        public string Subtitles { get; set; }
        public string ReleasedYear { get; set; }
        public bool IsPremium { get; set; }
        public int IsFavorite { get; set; }
        public int IsDashBoardBanner { get; set; }
        public string MovieTrailer { get; set; }
        public string MovieQuality { get; set; }
        public string Quality360 { get; set; }
        public string Quality480 { get; set; }
        public string Quality720 { get; set; }
        public string Quality1080 { get; set; }
        public List<Cast> cast { get; set; }
       
    }
    public class TestVideoModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MoviePoster { get; set; }

        public string Movie_Video { get; set; }


    }
    public class Cast
    {
       
        public string Charactor { get; set; }
        public string Role { get; set; }
        public string ImageURL { get; set; }
        
    }
    public class FileDetails
    {
        public string File { get; set; }
        public string URL { get; set; }

    }
    public class MovieCountModel
    {
        public int TotalMoviesCount { get; set; }
        public int FreeMoviesCount { get; set; }
    }
    public class ShortModel
    {

        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MoviePoster { get; set; }
    }
    public class UpcomingBanners
    {
        public int Id { get; set; }

        public String Poster { get; set; }

    }

   
    public class MovieTrailer
    {
        public int Id { get; set; }

        public string MovieName { get; set; }
        public string TrailerVideo { get; set; }
       
   }
    public class favModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MoviePoster { get; set; }
        public string Language { get; set; }
        public string IMDbRating { get; set; }
        public string Certificate { get; set; }
        public string Description { get; set; }
        public string ReleasedYear { get; set; }
        public string Duration { get; set; }

    }
    public class ImageType
    {
        public string Image { get; set; }

        public string Type { get; set; }
    }

    public class Message
    {
        public string message { get; set; }
    }
    public class Message2
    {
        public string message { get; set; }
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string PhoneNo { get; set; }
        public string Name { get; set; }
        public byte[] ProfilePic { get; set; }

    }
}