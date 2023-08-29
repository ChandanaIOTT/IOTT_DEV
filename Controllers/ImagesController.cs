
using IOTT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace IOTT_API.Controllers
{
    public class ImagesController : ApiController
    {
        // GET: Images
        
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllImages()
        {
            HttpResponseMessage result = null;
            List<ImageModel> ilist = new List<ImageModel>();
           
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllVideos", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var img = new ImageModel();
                        var cast1 = new Cast();

                        img.Id = Convert.ToInt32(rdr["ID"]);
                        img.Name = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        img.URL = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        

                        ilist.Add(img);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, ilist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }

            }
            return result;
        }




    }
}