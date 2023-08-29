using I_OTT_API.Models;
using IOTT_API.Models;
using Microsoft.Owin.Security.Provider;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ubiety.Dns.Core;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace IOTT_API.Controllers
{
    public class FCMController : ApiController
    {
        // GET: FCM
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [HttpGet]
        public HttpResponseMessage SendNotification(int MovieId)
        {
            HttpResponseMessage result = null;

            try
            {
                string Title1 = "";
                string Body1 = "";
                SqlConnection con = new SqlConnection(CS);

                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetFCMNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    // to = "eAksqhRpSZ-y6qFn5pw7CX:APA91bGJigIQFOyu5ZF9eWDbwy0171o0Q4fR1km72ICH9XkiEnGh1DttqS3V89MO6vHWP76y0HwhcyzR8IzhQrShtWKFsdW-KJeookCRGr52F9KUblVPjC56ukpCSMXLuwgJmrwtMpp6", // Uncoment this if you want to test for single device
                    // registration_ids = singlebatch, // this is for multiple user 

                    Title1 = dt.Rows[0]["Title"].ToString();
                    Body1 = dt.Rows[0]["Body"].ToString();
                }
                con.Close();
                
                con.Open();
                SqlCommand cmd1 = new SqlCommand("GetAllFCMTokens", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable();
                sda1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {

                        string token = dt1.Rows[i]["Fcm_Token"].ToString();
                        dynamic data = new
                        {

                            registration_ids = new List<string>() { token },
                            notification = new
                            {

                                title = Title1,
                                // Notification title
                                body = Body1,

                            }
                        };
                        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var json = serializer.Serialize(data);
                        Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                        string SERVER_API_KEY = "AAAARWxwvTk:APA91bFz9sWZric8qRXxP62PxVQbA04fQIEuLcFGRzSqq6SUUT0YOCRyywLw6Z4eC0DvCNZRPtXW786VGiNFS58TcDsha6moKgGIxRXjPMWbUN-i8LCyB4a2mFXGS-WvihqR88UmiEYm";
                        string SENDER_ID = "298172071225";

                        WebRequest tRequest;
                        tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                        tRequest.Method = "post";
                        tRequest.ContentType = "application/json";
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));


                        tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                        tRequest.ContentLength = byteArray.Length;
                        Stream dataStream = tRequest.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();


                        WebResponse tResponse = tRequest.GetResponse();

                        dataStream = tResponse.GetResponseStream();

                        StreamReader tReader = new StreamReader(dataStream);

                        String sResponseFromServer = tReader.ReadToEnd();
                        var m = new Message();
                        m.message = "Sent succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK,m);
                        tReader.Close();
                        dataStream.Close();
                        tResponse.Close();
                    }
                }
            }
            catch
            {

            }
            return result;
        }
    
    [System.Web.Http.HttpPost]
        public HttpResponseMessage Fcm_Token([FromBody] FCMModel f)
        {
            List<FCMModel> flist = new List<FCMModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPFcm_Token", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fcm_Token", f.Fcm_Token);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Token added succesfully";
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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostFcm_Token([FromBody] FCM ff)
        {
            List<FCM> flist = new List<FCM>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPFcm", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fcm_Token", ff.Fcm_Token);
                    cmd.Parameters.AddWithValue("@UserId", ff.UserId);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Token added succesfully";
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
        public HttpResponseMessage EditFcm([FromBody] FCM ff)
        {
            HttpResponseMessage result = null;
            List<FCM> flist = new List<FCM>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUpdateFcm", con);
               // cmd.Parameters.AddWithValue("@Id", ff.Id);
                cmd.Parameters.AddWithValue("@Fcm_Token", ff.Fcm_Token);
                cmd.Parameters.AddWithValue("@UserId", ff.UserId);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "updated succesfully";
                    message.Add(m);
                    flist.Add(ff);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

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


         [System.Web.Http.HttpPost]
        public HttpResponseMessage PostFcmNotification([FromBody] FCMNotification ff)
        {
            List<FCMNotification> flist = new List<FCMNotification>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPPostFcmNotification", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieId", ff.MovieId);
                    cmd.Parameters.AddWithValue("@Title", ff.Title);
                    cmd.Parameters.AddWithValue("@Body", ff.Body);
                    cmd.Parameters.AddWithValue("@Poster", ff.MoviePoster);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Notification Posted Successfully";
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
        public HttpResponseMessage EditFcmNotification([FromBody] FCMNotification ff)
        {
            HttpResponseMessage result = null;
            List<FCMNotification> flist = new List<FCMNotification>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPEditFcmNotification", con);
                // cmd.Parameters.AddWithValue("@Id", ff.Id);
                cmd.Parameters.AddWithValue("@MovieId", ff.MovieId);
                cmd.Parameters.AddWithValue("@Title", ff.Title);
                cmd.Parameters.AddWithValue("@Body", ff.Body);
                cmd.Parameters.AddWithValue("@Poster", ff.MoviePoster);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "updated succesfully";
                    message.Add(m);
                    flist.Add(ff);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

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


