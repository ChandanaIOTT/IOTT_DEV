using IOTT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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

    public class LanguageController : ApiController
    {

        // GET: Language
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpPost]
        public HttpResponseMessage UploadLanguage([FromBody] LanguageModel language)
        {
            HttpResponseMessage result = null;

            List<LanguageModel> llist = new List<LanguageModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPUploadLanguage", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Language", language.Language);
                    cmd.Parameters.AddWithValue("@Poster", language.Poster);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        Message m = new Message();
                        m.message = "Language uploaded succesfully";
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
        public HttpResponseMessage GetAllLanguages()
        {
            HttpResponseMessage result = null;
            List<LanguageModel> llist = new List<LanguageModel>();
            

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetallLanguages", con);

                cmd.CommandType = CommandType.StoredProcedure;
                //SqlCommand cmd = new SqlCommand("SELECT * FROM LanguageTable", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var langugae = new LanguageModel();



                        langugae.Id = Convert.ToInt32(rdr["ID"]);
                        langugae.Language = rdr["Language"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        string posterurl = rdr["Poster"].ToString();

                        langugae.Poster = "https://productionbucket.blob.core.windows.net/images/" + posterurl;

                        llist.Add(langugae);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, llist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Language Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            } 
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteLanguage(int id)
        {
            HttpResponseMessage result = null;

            List<LanguageModel> llist = new List<LanguageModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteLanguage", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)

                {
                    var language = new LanguageModel();
                    var m = new Message();

                    m.message = "Language deleted succesfully";
                    message.Add(m);
                    llist.Remove(language);

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
        public HttpResponseMessage EditLanguage([FromBody] LanguageModel language)
        {
            HttpResponseMessage result = null;
            List<LanguageModel> llist = new List<LanguageModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUpdateLanguage", con);
                cmd.Parameters.AddWithValue("@Id", language.Id);
                cmd.Parameters.AddWithValue("@Language", language.Language);
                cmd.Parameters.AddWithValue("@Poster", language.Poster);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "Language updated succesfully";
                    message.Add(m);
                    llist.Add(language);

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

    
