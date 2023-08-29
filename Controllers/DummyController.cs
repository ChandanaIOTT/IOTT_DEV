using I_OTT_API.Models;
using IOTT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace IOTT_API.Controllers
{
    public class DummyController : ApiController
    {
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        // GET: User
        [Authorize]

        [HttpGet]
        public IEnumerable<UserModel> GetAllUser()
        {
            List<UserModel> userlist = new List<UserModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_User", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var user = new UserModel();

                    user.Id = Convert.ToInt32(rdr["ID"]);
                    user.PhoneNo = rdr["PhoneNo"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Location = rdr["Location"].ToString();
                    string url = HttpContext.Current.Request.Url.Authority;
                    string ProfileImg = rdr["ProfileImg"].ToString();
                    user.ProfileImg = "http://" + url + "/Images/" + ProfileImg;

                    userlist.Add(user);


                }

            }
            return userlist;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Test_UploadMovie([FromBody] TestVideoModel test)
        {
            List<VideoModel> vlist = new List<VideoModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SP_TestUpload", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MovieName", test.MovieName);
                    cmd.Parameters.AddWithValue("MoviePoster", test.MoviePoster);
                    cmd.Parameters.AddWithValue("Movie_Video", test.Movie_Video);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();

                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Movie Data  added";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }

                }
            }
            catch (Exception ex)
            {
                var m1 = new Message();
                m1.message = ex.Message;
                message.Add(m1);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }

            return result;

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_TestMovie()
        {
            HttpResponseMessage result = null;
            List<TestVideoModel> vlist = new List<TestVideoModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGet_TestMovie", con);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var test1 = new TestVideoModel();

                        test1.Id = Convert.ToInt32(rdr["ID"]);
                        test1.MovieName = rdr["MovieName"].ToString();
                        test1.MoviePoster = rdr["MoviePoster"].ToString();

                        string url = HttpContext.Current.Request.Url.Authority;

                        string posterurl = rdr["MoviePoster"].ToString();
                        test1.Movie_Video = "http://" + url + "/MoviesData/" + rdr["Movie_Video"].ToString();


                        test1.MoviePoster = "http://" + url + "/Images/" + posterurl;

                        vlist.Add(test1);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "Something went wrong";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }


        }
    }
}