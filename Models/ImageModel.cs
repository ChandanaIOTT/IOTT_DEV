using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string WebURL { get; set; }
        public bool IsPremium { get; set; }
    }
    public class FreeModel
    {
        public int Id { get; set; }

        public String MovieName { get; set; }

        public string Genre { get; set; }



    }

}