using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class KnowIOTTModel
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
    public class ContactUsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CallUs { get; set; }
        public string Email { get; set; }

    }
    public class VersionModel
    {
        public int Id { get; set; }

        public string Platform { get; set; }

        public int  Version { get; set; }
    }
}