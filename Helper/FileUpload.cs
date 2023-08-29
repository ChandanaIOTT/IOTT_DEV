using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Helper
{
    public class FileUpload
    {
        public IFormFile MoviePoster { get; set; }
        public IFormFile PatientImages { get; set; }
        public IFormFile OrganizationImage { get; set; }
        public IFormFile ExcelFile { get; set; }
    }
}