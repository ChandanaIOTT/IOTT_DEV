using System;
using System.Web.Http.Cors;

namespace TokenBasedAuthentication.Models
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header")]
    public class User
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public string[] Roles { get; set; }
    }
}