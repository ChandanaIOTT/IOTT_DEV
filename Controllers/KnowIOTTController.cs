using IOTT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace IOTT_API.Controllers
{
    public class KnowIOTTController : ApiController
    {
        // GET: KnowIIOTT
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AboutIOtt([FromBody] KnowIOTTModel aboutus)
        {
            HttpResponseMessage result = null;

            List<KnowIOTTModel> llist = new List<KnowIOTTModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPAboutIott", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", aboutus.Title);
                    cmd.Parameters.AddWithValue("@Description", aboutus.Description);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "uploaded succesfully";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
            }

            return result;

        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAboutIott(string Title)
        {
            HttpResponseMessage result = null;
            List<KnowIOTTModel> llist = new List<KnowIOTTModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAboutIott", con);
                cmd.Parameters.AddWithValue("@Title",Title);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_LoginIssues ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var aboutus = new KnowIOTTModel();

                        aboutus.Id = Convert.ToInt32(rdr["ID"]);
                        aboutus.Title = rdr["Title"].ToString();
                        aboutus.Description = rdr["Description"].ToString();



                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, aboutus);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;

        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage ContactUs([FromBody] ContactUsModel contactus)
        {
            HttpResponseMessage result = null;

            List<ContactUsModel> llist = new List<ContactUsModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPContactUs", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", contactus.Title);
                    cmd.Parameters.AddWithValue("@Description", contactus.Description);
                    cmd.Parameters.AddWithValue("@CallUs", contactus.CallUs);
                    cmd.Parameters.AddWithValue("@Email", contactus.Email);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "uploaded succesfully";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
            }

            return result;

        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetContactUs()
        {
            HttpResponseMessage result = null;
            List<ContactUsModel> llist = new List<ContactUsModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetContactUs", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_LoginIssues ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var contactus = new ContactUsModel();

                        contactus.Id = Convert.ToInt32(rdr["ID"]);
                        contactus.Title = rdr["Title"].ToString();
                        contactus.Description = rdr["Description"].ToString();
                        contactus.CallUs = rdr["CallUs"].ToString();
                        contactus.Email = rdr["Email"].ToString();



                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, contactus);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;

        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Latest_api_version ([FromBody] VersionModel  v)
        {
            List<VersionModel> vlist = new List<VersionModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("[SPAppVersion]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Platform", v.Platform);
                    cmd.Parameters.AddWithValue("@Version", v.Version);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "App Information added succesfully";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
            }
            return result;
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage Edit_apiversion([FromBody] VersionModel v)
        {
            HttpResponseMessage result = null;
            List<VersionModel> vlist = new List<VersionModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUpdateVersion", con);
                cmd.Parameters.AddWithValue("@Id", v.Id);
                cmd.Parameters.AddWithValue("@Platform", v.Platform);
                cmd.Parameters.AddWithValue("@version", v.Version);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "updated succesfully";
                    message.Add(m);
                    vlist.Add(v);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

                }
                else
                {
                    var m = new Message();
                    m.message = "Something Went Wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();
            }
            return result;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_Latest_version(string Platform)
        {
            HttpResponseMessage result = null;
            List<VersionModel> vlist = new List<VersionModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGet_Latest_Version", con);
                cmd.Parameters.AddWithValue("Platform", Platform);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_App_Information ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var v = new VersionModel();

                        v.Id = Convert.ToInt32(rdr["ID"]);
                       // v.Version = rdr["Version"].ToString();

                       v.Platform = rdr["Platform"].ToString();
                        v.Version = Convert.ToInt32(rdr["Version"]);

                       // v.Version = float.Parse(rdr["Version"].ToString());

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, v);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;

        }
    }

}