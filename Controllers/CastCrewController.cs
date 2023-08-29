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
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace IOTT_API.Controllers
{
    [Authorize]
    public class CastCrewController : ApiController
    {
        // GET: CastandCrew
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddCast([FromBody] CastModel cast)
        {
            List<CastModel> clist = new List<CastModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPUploadCast", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OriginalName", cast.OriginalName);
                    cmd.Parameters.AddWithValue("@Image", cast.Image);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Cast uploaded succesfully";
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
        public HttpResponseMessage GetAllCast()
        {
            HttpResponseMessage result = null;
            List<CastModel> clist = new List<CastModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetallCast", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_CastandCrew", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var cast = new CastModel();

                        cast.Id = Convert.ToInt32(rdr["ID"]);
                        cast.OriginalName = rdr["OriginalName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        string imageurl = rdr["Image"].ToString();

                        cast.Image = "http://" + url + "/Images/" + imageurl;

                        clist.Add(cast);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, clist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Cast Details  Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetCastName(string originalname)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<CastModel> clist = new List<CastModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetByOriginalName", con);
                cmd.Parameters.AddWithValue("@OriginalName", originalname);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  tbl_CastandCrew", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var cast = new CastModel();

                        cast.Id = Convert.ToInt32(rdr["ID"]);
                        cast.OriginalName = rdr["OriginalName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string imageurl = rdr["Image"].ToString();
                        cast.Image = "http://" + url + "/Images/" + imageurl;

                        // clist.Add(cast);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, cast);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Actor image Found in this Name";
                    message.Add(m);
                    result= ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteCast(int id)
        {
            HttpResponseMessage result = null;

            List<CastModel> clist = new List<CastModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteCast", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int k  = cmd.ExecuteNonQuery();
                if (k==1)
                {
                        var cast = new CastModel();
                        var m = new Message();

                    //  cast.Id = Convert.ToInt32(rdr["id"]);
                    //   cast.OriginalName = rdr["OriginalName"].ToString();
                    //   string url = HttpContext.Current.Request.Url.Authority;
                    //   string imageurl = rdr["Image"].ToString();
                    // cast.Image = "http://" + url + "/Images/" + imageurl;
                    m.message = "cast deleted succesfully";
                    message.Add(m);
                    clist.Remove(cast);

                   result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                   
                }
                else
                {
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
                return result;
            }
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage EditCast([FromBody] CastModel cast)
        {
            HttpResponseMessage result = null;
            List<CastModel> clist = new List<CastModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUpdateCast", con);
                cmd.Parameters.AddWithValue("@Id", cast.Id);
                cmd.Parameters.AddWithValue("@OriginalName", cast.OriginalName);
                cmd.Parameters.AddWithValue("@Image", cast.Image);
                 
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if(rdr.HasRows)
                {
                    
                    var m = new Message();
                    m.message = "cast updated succesfully";
                    message.Add(m);
                    clist.Add(cast);

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

    }
}



   