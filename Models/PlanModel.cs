using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class PlanModel
    {
        public int Id { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public string Duration { get; set; }
       
    }
}