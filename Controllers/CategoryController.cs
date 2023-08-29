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

    public class CategoryController : ApiController
    {
        // GET: Category
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpPost]

        public HttpResponseMessage AddCategory([FromBody] CategoryModel category)
        {

            List<CategoryModel> cclist = new List<CategoryModel>();
            HttpResponseMessage result = null;

            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPAddCategory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    cmd.Parameters.AddWithValue("@CategoryUrl", category.CategoryUrl);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = " Genre uploaded succesfully";
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
        public HttpResponseMessage GetAllCategory()
        {
            HttpResponseMessage result = null;
            List<CategoryModel> cclist = new List<CategoryModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllCategory", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var category = new CategoryModel();

                        category.Id = Convert.ToInt32(rdr["ID"]);
                        category.CategoryName = rdr["CategoryName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        string categoryurl = rdr["CategoryUrl"].ToString();

                        category.CategoryUrl = "https://productionbucket.blob.core.windows.net/images/" + categoryurl;

                        cclist.Add(category);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, cclist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Genres Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage EditCategory([FromBody] CategoryModel category)
        {
            HttpResponseMessage result = null;
            List<CategoryModel> cclist = new List<CategoryModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUpdateGenre", con);
                cmd.Parameters.AddWithValue("@Id", category.Id);
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                cmd.Parameters.AddWithValue("@CategoryUrl", category.CategoryUrl);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "Genre updated succesfully";
                    message.Add(m);
                    cclist.Add(category);

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

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteCategory(int id)
        {
            HttpResponseMessage result = null;

            List<CategoryModel> cclist = new List<CategoryModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteGenre", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                int k  = cmd.ExecuteNonQuery();
                if (k==1)
                {
                    var category = new CategoryModel();
                    var m = new Message();

                    m.message = "Genre deleted succesfully";
                    message.Add(m);
                    cclist.Remove(category);

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
    }
}
