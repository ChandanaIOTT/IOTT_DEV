using IOTT_API.Models;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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

    public class NotificationController : ApiController
    {
        // GET: Notification

        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddNotification([FromBody] NotificationType notification)
        {
            HttpResponseMessage result = null;

            List<NotificationType> llist = new List<NotificationType>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    if (notification.MovieId != 0)
                    {
                        SqlCommand cmd = new SqlCommand("SPAddNotifications", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Text", notification.Text);
                        DateTime now1 = DateTime.Now;
                        cmd.Parameters.AddWithValue("@Day", now1.ToShortDateString());
                        cmd.Parameters.AddWithValue("@MovieId", notification.MovieId);

                        con.Open();
                        int k = cmd.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Notification added succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }
                    }
                    else
                    {
                        SqlCommand cmd1 = new SqlCommand("SPNotifications", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Text", notification.Text);
                        DateTime now1 = DateTime.Now;
                        cmd1.Parameters.AddWithValue("@Day", now1.ToShortDateString());


                        con.Open();
                        int k = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Notification added succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetNotifications(int id)
        {
            HttpResponseMessage result = null;
            List<NotificationType> llist = new List<NotificationType>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetNotifications", con);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var n = new NotificationType();

                        string url = HttpContext.Current.Request.Url.Authority;


                        n.Id = Convert.ToInt32(rdr["ID"]);

                        n.Text = rdr["Text"].ToString();
                        n.Day = rdr["Day"].ToString();
                        n.MovieId = Convert.ToInt32(rdr["MovieId"]);
                        n.MoviePoster = "http://" + url + "/Images/" + rdr["MoviePoster"].ToString();
                        llist.Add(n);



                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, llist);

                }
                else
                {
                    var m = new Message();
                    m.message = "Something went wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetWalletNotification(int id)
        {
            HttpResponseMessage result = null;
            List<NotificationType> llist = new List<NotificationType>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPNotificationType", con);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var n = new NotificationType();
                        string url = HttpContext.Current.Request.Url.Authority;
                        n.Id = Convert.ToInt32(rdr["ID"]);

                        n.Text = rdr["Text"].ToString();
                        n.Day = rdr["Day"].ToString();

                        llist.Add(n);



                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, llist);

                }
                else
                {
                    var m = new Message();
                    m.message = "Something went wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetAllNotifications()
        {
            HttpResponseMessage result = null;
            List<NotificationType> llist = new List<NotificationType>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllnotifications", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var n = new NotificationType();
                        string url = HttpContext.Current.Request.Url.Authority;
                        n.Id = Convert.ToInt32(rdr["ID"]);

                        n.Text = rdr["Text"].ToString();
                        n.Day = rdr["Day"].ToString();
                        n.MovieId = Convert.ToInt32(rdr["MovieId"]);
                        string movieurl = rdr["MoviePoster"].ToString();


                        n.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;

                        llist.Add(n);



                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, llist);

                }
                else
                {
                    var m = new Message();
                    m.message = "Something went wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        public HttpResponseMessage Delete_Notification(int MovieId)
        {
            HttpResponseMessage result = null;

            List<NotificationType> llist = new List<NotificationType>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteNotification", con);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)
                {
                    var notification = new NotificationType();
                    var m = new Message();

                    m.message = "deleted succesfully";
                    message.Add(m);
                    llist.Remove(notification);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

                }
                else
                {
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;


        }
    }
}