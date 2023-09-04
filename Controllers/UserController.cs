using Firebase.Auth;
using I_OTT_API.Models;
using IOTT_API.Models;
using MaxMind.Db;
using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
//using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Net.Http;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;
//using Microsoft.Owin.Security;
using System.Xml.Linq;
//using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
//using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using AuthenticationProperties = Microsoft.Owin.Security.AuthenticationProperties;
using Microsoft.Owin;
using System.Linq.Expressions;
//using MySqlX.XDevAPI.Common;
using TokenBasedAuthentication.Models;
using FirebaseAdmin;
using System.Web.Http.Results;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;
//using Microsoft.AspNetCore.Identity;
using Microsoft.Owin.Security;


namespace IOTT_API.Controllers
{
   

    public class UserController : ApiController
    {
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        private static string ApiKey = "AIzaSyDFa0Gr-1DVIMyjxUqOx1fJj6kpQ21lthI";


        // GET: User

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
                    user.Name = rdr["Name"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Location = rdr["Location"].ToString();
                    if (!Convert.IsDBNull(rdr["SubDateTime"]))
                    {
                        user.SubDateTime = Convert.ToDateTime(rdr["SubDateTime"]);

                    }
                    else
                    {
                        user.SubDateTime = DateTime.MinValue;

                    }
                    userlist.Add(user);

                }

            }
            return userlist;
        }
        /*private Random random = new Random();

         public string RandomString(string phoneNo, int length)
         {
             const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
             return new string(Enumerable.Repeat(chars, length)
                 .Select(s => s[random.Next(s.Length)]).ToArray());
         }
        */
        //[Authorize]

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> signUpUser([System.Web.Http.FromBody] UserModel user)    //chandana
        {
            HttpResponseMessage result = null;
            var m = new Message();
            string otp = "";
            string rc = "";
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string sOTP = String.Empty;
                    // string rCD = String.Empty;
                    int min = 1000;
                    int max = 9999;

                    string datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    DateTime sdt = Convert.ToDateTime(datetime);

                    Random rdm = new Random();

                    sOTP = rdm.Next(min, max).ToString();
                    otp = sOTP;
                    //SenEmail(otp);
                    if (user.PhoneNo != null && user.Name != null && user.Password != null && user.Email != null)
                    {
                        SqlCommand cmd = new SqlCommand("USP_SignUpUser", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("PhoneNo", user.PhoneNo);
                        cmd.Parameters.AddWithValue("Name", user.Name);
                        cmd.Parameters.AddWithValue("Email", user.Email);
                        //cmd.Parameters.AddWithValue("OTP", otp);
                        cmd.Parameters.AddWithValue("Password", user.Password);
                        cmd.Parameters.AddWithValue("SubDateTime", sdt);
                        cmd.Parameters.AddWithValue("@RegStatus", "0");

                        //  cmd.Parameters.AddWithValue("UserId", user.UserId);
                        //  cmd.Parameters.AddWithValue("DeviceModel", user.DeviceModel);

                        con.Open();
                        // int r = cmd.ExecuteNonQuery();
                        SqlDataReader r = cmd.ExecuteReader();




                        if (r.HasRows)
                        {

                            string Message = "";
                            while (r.Read())
                            {
                                Message = (r["Message1"]).ToString();
                            }

                            m.message = Message;
                            message.Add(m);
                            //SendSMS(user.PhoneNo, otp);
                            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                            var a = await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Name, true);
                            m.message = "mail sent";

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }

                        /*else
                        {

                            m.message = "User Exist";
                            message.Add(m);
                            //SendSMS(user.PhoneNo, otp);
                            //GetRandomAlphaNumeric(user.PhoneNo, rc);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }*/
                        con.Close();

                    }
                    else
                    {
                        if (user.Password != null && user.Email != null)
                        {
                            SqlCommand cmd1 = new SqlCommand("USP_SignInWithEmail", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@Email", user.Email);
                            cmd1.Parameters.AddWithValue("@Password", user.Password);

                            // cmd1.Parameters.AddWithValue("UserId", user.UserId);
                            //  cmd1.Parameters.AddWithValue("DeviceModel", user.DeviceModel);

                            //cmd1.Parameters.AddWithValue("OTP", otp);

                            con.Open();
                            int k = cmd1.ExecuteNonQuery();
                            con.Close();
                            if (k <= 0)
                            {

                                m.message = "User Not Exist";
                                message.Add(m);
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                            }
                            else
                            {
                                /* m.message = "OTP is sent to your register mobile numebr";
                                 message.Add(m);
                                 SendSMS(user.PhoneNo, otp);*/
                                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                                var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                                string token = ab.FirebaseToken;
                                var userr = ab.User;
                                if (token != "")
                                {

                                    this.SignInUser(userr.Email, token, false);


                                }
                                else
                                {
                                    // Setting.
                                    m.message = "Invalid Email or Password";
                                    message.Add(m);
                                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                                }


                                m.message = "verified your email (login successful)";
                                message.Add(m);


                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                            }
                        }


                        else
                        {
                            SqlCommand cmd2 = new SqlCommand("USP_SignIn", con);
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("PhoneNo", user.PhoneNo);
                            cmd2.Parameters.AddWithValue("OTP", otp);
                            con.Open();
                            int e = cmd2.ExecuteNonQuery();
                            con.Close();

                            if (e <= 0)
                            {

                                m.message = "User not exist";
                                message.Add(m);
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                            }
                            else
                            {
                                m.message = "OTP is sent to your mobile number";
                                message.Add(m);
                                SendSMS(user.PhoneNo, otp);
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                            }
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                m.message = ex.Message;
                message.Add(m);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }
            return result;
        }



        private void SignInUser(string email, string token, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();


            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }


        private void ClaimIdentities(string username)
        {
            // Initialization.
            var claims = new List<Claim>();
            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetEmailById(int Id)   //chandana
        {
            var m = new Message();
            HttpResponseMessage result = null;
            List<UserModel> user = new List<UserModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Email FROM tbl_User where Id=@Id", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var email = new UserModel();
                        email.Email = rdr["Email"].ToString();

                        user.Add(email);
                        m.message = "Email Exist";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }

                }
                else
                {
                    m.message = "doesnot exist";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;
        }


        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] ForgotPasswordRequest request)   //chandana
        {

            var m = new Message();

            HttpResponseMessage result=null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    if (request == null || string.IsNullOrWhiteSpace(request.Email))
                    {
                        m.message = "Provide Email";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    else
                    {
                        SqlCommand cmd = new SqlCommand("USP_ForgotPassword", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("Email", request.Email);
                        cmd.Parameters.AddWithValue("Password", request.Password);
                        var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                        await auth.SendPasswordResetEmailAsync(request.Email);
                        con.Open();

                        int r = cmd.ExecuteNonQuery();
                        if (r <= 0)
                        {
                            m.message = "User not exist";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                        }
                        else
                        {
                            m.message = "Password reset instructions sent";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                            
                        }
                        con.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    /*con.Open();
                    SqlCommand cmd = new SqlCommand("SPAddError", con);
                    cmd.CommandType = CommandType.StoredProcedure;*/

                    m.message = ex.Message;
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }

            }
            return result;
        }




        [System.Web.Http.HttpGet]
        public HttpResponseMessage Is_user(string Phoneno)
        {

            HttpResponseMessage result = null;
           

            List<Phno> pflist = new List<Phno>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUser", con);
                cmd.Parameters.AddWithValue("@PhoneNo", Phoneno);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var m = new Message();
                        m.message = "user already exist";

                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 1);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "New user ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                con.Close();
            }
            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddFavorite([System.Web.Http.FromBody] FavoriteModel favorite)
        {
            HttpResponseMessage result = null;

            List<FavoriteModel> flist = new List<FavoriteModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {

                    SqlCommand cmd1 = new SqlCommand("select * from tbl_FavMovie where UserId=@UserId and MovieId=@MovieId ", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@UserId", favorite.UserId);
                    cmd1.Parameters.AddWithValue("@MovieId", favorite.MovieId);

                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand("SPUnFavorite", con);
                        cmd.Parameters.AddWithValue("@UserId", favorite.UserId);
                        cmd.Parameters.AddWithValue("@MovieId", favorite.MovieId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                        if (i == 1)
                        {
                            var getfavorite = new favModel();
                            var m = new Message();
                            m.message = "Removed from favorite succesfully";
                            message.Add(m);

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

                        }
                    }
                    else
                    {


                        SqlCommand cmd = new SqlCommand("SPAddFavorite", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", favorite.UserId);

                        cmd.Parameters.AddWithValue("@MovieId", favorite.MovieId);

                        con.Open();
                        int k = cmd.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Added to Favorites";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }

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
        // get favorite by Name
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetFavorite(string UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<favModel> flist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetFavoriteMovies", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var getfavorite = new favModel();


                        getfavorite.Id = Convert.ToInt32(rdr["ID"]);
                        getfavorite.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        getfavorite.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        getfavorite.Language = rdr["Language"].ToString();
                        getfavorite.Certificate = rdr["Certificate"].ToString();
                        getfavorite.IMDbRating = rdr["IMDbRating"].ToString();
                        getfavorite.Description = rdr["Description"].ToString();
                        getfavorite.ReleasedYear = rdr["ReleasedYear"].ToString();
                        getfavorite.Duration = rdr["Duration"].ToString();




                        flist.Add(getfavorite);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, flist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Favorite Movies  Found for this UserId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage UnFavorite(int userid, int movieid)
        {
            HttpResponseMessage result = null;

            List<favModel> flist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPUnFavorite", con);
                cmd.Parameters.AddWithValue("@UserId", userid);
                cmd.Parameters.AddWithValue("@MovieId", movieid);
                cmd.CommandType = CommandType.StoredProcedure;

                int i = cmd.ExecuteNonQuery();
                if (i == 1)
                {
                    var getfavorite = new favModel();
                    var m = new Message();
                    m.message = "Removed from favorite succesfully";
                    message.Add(m);
                    flist.Remove(getfavorite);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

                }
                else
                {
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;


        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetFavoritebyMovieid(int UserId, int MovieId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<favModel> flist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetFavoritebyMovieId", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 1);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Favorite Movies  Found for this UserId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddWatchHistory([System.Web.Http.FromBody] FavoriteModel history)
        {
            HttpResponseMessage result = null;

            List<FavoriteModel> hlist = new List<FavoriteModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd1 = new SqlCommand("select * from tbl_WatchHistory where UserId=@UserId and MovieId=@MovieId ", con);
                    cmd1.CommandType = CommandType.Text;

                    cmd1.Parameters.AddWithValue("@UserId", history.UserId);

                    cmd1.Parameters.AddWithValue("@MovieId", history.MovieId);

                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        var m = new Message();
                        m.message = "exist";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("SPWatchHistory", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", history.UserId);

                        cmd.Parameters.AddWithValue("@MovieId", history.MovieId);


                        con.Open();
                        int k = cmd.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Added to WatchHistory";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }

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
        public HttpResponseMessage GetWatchHistory(string UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<favModel> hlist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetWatchHistory", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var Watchhistory = new favModel();


                        Watchhistory.Id = Convert.ToInt32(rdr["ID"]);
                        Watchhistory.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        Watchhistory.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        Watchhistory.Language = rdr["Language"].ToString();
                        Watchhistory.Certificate = rdr["Certificate"].ToString();
                        Watchhistory.IMDbRating = rdr["IMDbRating"].ToString();
                        Watchhistory.Description = rdr["Description"].ToString();
                        Watchhistory.ReleasedYear = rdr["ReleasedYear"].ToString();
                        Watchhistory.Duration = rdr["Duration"].ToString();




                        hlist.Add(Watchhistory);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, hlist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage ClearWatchHistory(int userid, int movieid)
        {
            HttpResponseMessage result = null;

            List<favModel> hlist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPClearHistory", con);
                cmd.Parameters.AddWithValue("@UserId", userid);
                cmd.Parameters.AddWithValue("@MovieId", movieid);
                cmd.CommandType = CommandType.StoredProcedure;

                int i = cmd.ExecuteNonQuery();
                if (i == 1)
                {
                    var Watchhistory = new favModel();
                    var m = new Message();
                    m.message = "Removed from watch history succesfully";
                    message.Add(m);
                    hlist.Remove(Watchhistory);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

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
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage ClearallWatchHistory(int userid)
        {
            HttpResponseMessage result = null;

            List<favModel> hlist = new List<favModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPClearAllHistory", con);
                cmd.Parameters.AddWithValue("@UserId", userid);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();
                if (k > 0)
                {
                    var Watchhistory = new favModel();
                    var m = new Message();
                    m.message = "Cleared WatchHistory";
                    message.Add(m);
                    hlist.Remove(Watchhistory);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage FeedBack([System.Web.Http.FromBody] FeedBackModel feedBack)
        {
            HttpResponseMessage result = null;

            List<FeedBackModel> fblist = new List<FeedBackModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPFeedBack", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", feedBack.UserId);
                    cmd.Parameters.AddWithValue("@FeedBack", feedBack.FeedBack);
                    cmd.Parameters.AddWithValue("@Rating", feedBack.Rating);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Feedback Added Succesfully";
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
        public HttpResponseMessage GetFeedBack(string UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<FeedBackModel> fblist = new List<FeedBackModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPFeedBackByUser", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var feedback = new FeedBackModel();


                        feedback.Id = Convert.ToInt32(rdr["ID"]);
                        feedback.FeedBack = rdr["FeedBack"].ToString();
                        feedback.Rating = Convert.ToInt32(rdr["Rating"]);
                        feedback.UserId = Convert.ToInt32(rdr["UserId"]);


                        fblist.Add(feedback);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, fblist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No FeedBack Found for this User";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllFeedBack()
        {
            HttpResponseMessage result = null;
            List<FeedBackModel> fblist = new List<FeedBackModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllFeedBack", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var feedback = new FeedBackModel();



                        feedback.Id = Convert.ToInt32(rdr["ID"]);
                        feedback.FeedBack = rdr["FeedBack"].ToString();
                        feedback.Rating = Convert.ToInt32(rdr["Rating"]);
                        feedback.UserId = Convert.ToInt32(rdr["UserId"]);


                        fblist.Add(feedback);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, fblist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "no FeedBack Found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        //[Authorize]


        [System.Web.Http.HttpPost]
        public HttpResponseMessage VerifyUser([System.Web.Http.FromBody] UserModel user)
        {
            HttpResponseMessage result = null;
            List<ProfileModel> userlist = new List<ProfileModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    byte[] data = new byte[1000];

                    SqlCommand cmd = new SqlCommand("USPVarifyOTP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);

                    cmd.Parameters.AddWithValue("@OTP", user.OTP);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var p = new ProfileModel();
                            string url = HttpContext.Current.Request.Url.Authority;

                            p.Id = Convert.ToInt32(rdr["Id"].ToString());

                            p.PhoneNo = rdr["PhoneNo"].ToString();
                            p.Name = rdr["Name"].ToString();

                          // p.DeviceModel = rdr["DeviceModel"].ToString();
                          //  p.UserId = Convert.ToInt32(rdr["UserId"].ToString());


                            //p.ReferralCode = rdr["ReferralCode"].ToString();  referred by 
                            // p.ProfilePic = (byte[])rdr["Pic"];

                            p.ProfileImg = "http://" + url + "/Images/" + rdr["ProfileImg"].ToString();
                            // p.ProfilePic = (byte[])rdr.GetValue(rdr.GetOrdinal("Pic"));
                            if (rdr["Pic"] == DBNull.Value)
                            {
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                            }
                            else
                            {
                                p.ProfilePic = (byte[])rdr["Pic"];
                            }
                            userlist.Add(p);

                        }
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);
                    }
                    else
                    {
                        var m = new Message();
                        m.message = "Invalid OTP";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                    }
                    con.Close();
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
       // [Authorize]

        [System.Web.Http.HttpPost]
        public HttpResponseMessage VerifyUserSignUp([System.Web.Http.FromBody] UserModel user)
        {
            List<Message2> mess = new List<Message2>();
            HttpResponseMessage result = null;
            List<ProfileModel> userlist = new List<ProfileModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    byte[] data = new byte[1000];
                    int Userid = 0;
                    SqlCommand cmd = new SqlCommand("USPVarifyOTPTestSignUp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);

                    cmd.Parameters.AddWithValue("@OTP", user.OTP);
                   
                    SqlDataReader rdr = cmd.ExecuteReader();

                   
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var p = new ProfileModel();
                            string url = HttpContext.Current.Request.Url.Authority;

                            p.Id = Convert.ToInt32(rdr["Id"].ToString());
                            Userid = p.Id;
                            p.PhoneNo = rdr["PhoneNo"].ToString();
                            p.Name = rdr["Name"].ToString();



                            p.ProfileImg = "http://" + url + "/Images/" + rdr["ProfileImg"].ToString();

                            if (rdr["Pic"] == DBNull.Value)
                            {
                                // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                            }
                            else
                            {
                                p.ProfilePic = (byte[])rdr["Pic"];
                            }
                            userlist.Add(p);

                        }
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);
                        //SqlCommand cmd1 = new SqlCommand("[USPVerifyDevice]", con);
                        //cmd1.CommandType = CommandType.StoredProcedure;

                        //cmd1.Parameters.AddWithValue("@UserId", Userid);


                        //con.Close();

                        //con.Open();
                        //SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                        //DataTable dt = new DataTable();
                        //sda.Fill(dt);
                        //con.Close();
                        //if (dt.Rows.Count > 0)
                        //{
                        //    int TotalDevices = Convert.ToInt32(dt.Rows[0]["TotalDevices"].ToString());
                        //    string DeviceName = dt.Rows[0]["DeviceName"].ToString();

                        //    if (TotalDevices >= 1)
                        //    {
                        //        var m2 = new Message2();
                        //        m2.Message = "Another Device Logeged in, Do you want Logout other Devices?";
                        //        m2.Id = Userid;
                        //        m2.DeviceName = DeviceName;
                        //        mess.Add(m2);
                        //        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, mess);

                        //    }
                        //    else
                        //    {
                        //        SqlCommand cmd2 = new SqlCommand("[USPAddDevice]", con);
                        //        cmd2.CommandType = CommandType.StoredProcedure;
                        //        cmd2.Parameters.AddWithValue("@UserId", Userid);

                        //        cmd2.Parameters.AddWithValue("@DeviceName", user.DeviceName);
                        //        con.Open();
                        //        int i = cmd2.ExecuteNonQuery();
                        //        con.Close();
                        //        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                        //    }
                        //}
                        //else
                        //{
                        //    SqlCommand cmd2 = new SqlCommand("[USPAddDevice]", con);
                        //    cmd2.CommandType = CommandType.StoredProcedure;
                        //    cmd2.Parameters.AddWithValue("@UserId", Userid);

                        //    cmd2.Parameters.AddWithValue("@DeviceName", user.DeviceName);
                        //    con.Open();
                        //    int i = cmd2.ExecuteNonQuery();
                        //    con.Close();
                        //    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                        //}
                    }
                    else
                    {
                        var m = new Message2();
                        m.message = "Invalid OTP";
                        mess.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, mess);
                    }
                    con.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                var m1 = new Message();
                m1.message = ex.Message;
                message.Add(m1);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                return result;
            }

        }
       // [Authorize]

        [System.Web.Http.HttpPost]
        public HttpResponseMessage VerifyUserTest([System.Web.Http.FromBody] UserModel user)
        {
            List<Message2> mess = new List<Message2>();
            HttpResponseMessage result = null;
            List<ProfileModel> userlist = new List<ProfileModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    byte[] data = new byte[1000];
                    int Userid = 0;
                    string Username = " ";
                    byte[] ProfilePic = null;
                    SqlCommand cmd = new SqlCommand("USPVarifyOTPTest", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);

                    cmd.Parameters.AddWithValue("@OTP", user.OTP);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var p = new ProfileModel();
                            string url = HttpContext.Current.Request.Url.Authority;

                            p.Id = Convert.ToInt32(rdr["Id"].ToString());
                            Userid = p.Id;
                            p.PhoneNo = rdr["PhoneNo"].ToString();
                            p.Name = rdr["Name"].ToString();
                            Username = p.Name;



                            p.ProfileImg = "http://" + url + "/Images/" + rdr["ProfileImg"].ToString();

                            if (rdr["Pic"] == DBNull.Value)
                            {
                                // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                            }
                            else
                            {
                                p.ProfilePic = (byte[])rdr["Pic"];
                            }
                            ProfilePic = p.ProfilePic;
                            userlist.Add(p);

                        }
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);
                        con.Close();







                        SqlCommand cmd1 = new SqlCommand("[USPVerifyDevice]", con);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.AddWithValue("@UserId", Userid);


                        con.Close();

                        con.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        con.Close();
                        if (dt.Rows.Count > 0)
                        {
                            int TotalDevices = Convert.ToInt32(dt.Rows[0]["TotalDevices"].ToString());
                            string DeviceName = dt.Rows[0]["DeviceName"].ToString();

                            if (TotalDevices >= 1)
                            {
                                var m2 = new Message2();
                                m2.message = "Another Device Logeged in, Do you want Logout other Devices?";
                                m2.Id = Userid;
                                m2.Name = Username;
                                m2.DeviceName = DeviceName;
                                m2.PhoneNo = user.PhoneNo;
                                m2.ProfilePic = ProfilePic;
                                mess.Add(m2);
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, mess);

                            }
                            else
                            {
                                SqlCommand cmd2 = new SqlCommand("[USPAddDevice]", con);
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Parameters.AddWithValue("@UserId", Userid);

                                cmd2.Parameters.AddWithValue("@DeviceName", user.DeviceName);
                                con.Open();
                                int i = cmd2.ExecuteNonQuery();
                                con.Close();
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                            }
                        }
                        else
                        {
                            SqlCommand cmd2 = new SqlCommand("[USPAddDevice]", con);
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("@UserId", Userid);

                            cmd2.Parameters.AddWithValue("@DeviceName", user.DeviceName);
                            con.Open();
                            int i = cmd2.ExecuteNonQuery();
                            con.Close();
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);
                        }
                    }



                    else
                    {
                        var m = new Message2();
                        m.message = "Invalid OTP";
                        mess.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, mess);
                    }
                    // con.Close();
                }
            }
            catch (Exception ex)
            {
                var m1 = new Message();
                m1.message = ex.Message;
                message.Add(m1);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                return result;
            }
            return result;
        }
        // [Authorize]
        private void LogoutDynamic(int UserId)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("USPLogout", con);
                cmd.Parameters.AddWithValue("@Userid ", UserId);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)
                {

                    var sds = new UserDevice();
                    var m = new Message();

                    m.message = "Logout succesfully";
                    message.Add(m);


                }
                else
                {
                    var m = new Message();
                    m.message = "Logged Out already";
                    message.Add(m);


                }
                con.Close();
            }
        }
        // [Authorize]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Forced_Login([FromBody] UserDevice ud)
        {
            List<UserDevice> ub = new List<UserDevice>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    LogoutDynamic(ud.UserId);
                    SqlCommand cmd = new SqlCommand("USPAddDevice", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Userid", ud.UserId);
                    cmd.Parameters.AddWithValue("@DeviceName", ud.DeviceName);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k > 0)
                    {
                        var m = new Message();
                        m.message = "login successful";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }      //  [Authorize]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Logout(int UserId)
        {
            HttpResponseMessage result = null;

            List<UserDevice> ub = new List<UserDevice>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("USPLogout", con);
                cmd.Parameters.AddWithValue("@Userid ", UserId);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)
                {

                    var sds = new UserDevice();
                    var m = new Message();

                    m.message = "Logout succesfully";
                    message.Add(m);
                    ub.Remove(sds);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

                }
                else
                {
                    var m = new Message();
                    m.message = "Logged Out already";
                    message.Add(m);
                  
                    result=ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                }
                con.Close();
            }
            return result;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUserById(int Userid)
        {
            HttpResponseMessage result = null;

            List<ProfileModel> userlist = new List<ProfileModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    byte[] data = new byte[1000];

                    SqlCommand cmd = new SqlCommand("SPGetUserByID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", Userid);

                  
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var p = new ProfileModel();
                            string url = HttpContext.Current.Request.Url.Authority;

                            p.Id = Convert.ToInt32(rdr["Id"].ToString());

                            p.PhoneNo = rdr["PhoneNo"].ToString();
                            p.Name = rdr["Name"].ToString();

                            // p.DeviceModel = rdr["DeviceModel"].ToString();
                            //  p.UserId = Convert.ToInt32(rdr["UserId"].ToString());


                            //p.ReferralCode = rdr["ReferralCode"].ToString();  referred by 
                            // p.ProfilePic = (byte[])rdr["Pic"];

                            p.ProfileImg = "http://" + url + "/Images/" + rdr["ProfileImg"].ToString();
                            // p.ProfilePic = (byte[])rdr.GetValue(rdr.GetOrdinal("Pic"));
                            if (rdr["Pic"] == DBNull.Value)
                            {
                               // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                            }
                            else
                            {
                                p.ProfilePic = (byte[])rdr["Pic"];
                            }
                            userlist.Add(p);

                        }
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;

        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage EditProfile([System.Web.Http.FromBody] ProfileModel user)
        {
            HttpResponseMessage result = null;
            var requestBody = new
            {
                ProfileModel = new string[] { }
            };

            List<ProfileModel> userlist = new List<ProfileModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                byte[] data = new byte[1000];

                con.Open();
                SqlCommand cmd = new SqlCommand("SPEditProfile",con);
                cmd.Parameters.AddWithValue("@Id", user.Id);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@ProfileImg", user.ProfileImg);
                cmd.Parameters.AddWithValue("@Pic",         user.ProfilePic);
                //SqlParameter photo = new SqlParameter("@Pic", SqlDbType.VarBinary);


                cmd.CommandType = CommandType.StoredProcedure;

                int k = cmd.ExecuteNonQuery();
                if (k == 1)
                {

                    var m = new Message { };
                    m.message = "Profile updated succesfully";
                    message.Add(m);
                    userlist.Add(user);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);

                }
                else
                {
                    var m = new Message { };
                    m.message = "Something Went Wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteProfile(int UserId)
        {
            HttpResponseMessage result = null;

            List<Profile> vlist = new List<Profile>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteProfile", con);
                cmd.Parameters.AddWithValue("@Id", UserId);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();
                if (k >= 0)
                {
                    var pr = new Profile();
                    var m = new Message();


                    m.message = "Profile  deleted succesfully";
                    message.Add(m);
                    vlist.Remove(pr);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

                }
                else
                {
                   result= ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
                return result;
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsLogin(int UserId, string DeviceName)
        {
            HttpResponseMessage result = null;

            List<UserDevice> usb = new List<UserDevice>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SPIsLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId ", UserId);
                    cmd.Parameters.AddWithValue("@DeviceName ", DeviceName);


                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                     /*   var user = new UserDevice();
                        user. = dt.Rows[0].Rows[0]["Following"].ToString();
                        user.follower = dt.Tables[1].Rows[0]["Follower"].ToString();
                        */



                        /* string Status = "";
                         Status = dt.Rows[0]["isLogin"].ToString(); ;*/

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK,1);

                    }
                    else
                    {
                        string Status = "False";

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 0);
                       // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 0);
                    }
                    con.Close();
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
        public HttpResponseMessage post_ViewsCount([System.Web.Http.FromBody] FavoriteModel views)
        {
            HttpResponseMessage result = null;
            
            List<FavoriteModel> vlist = new List<FavoriteModel>();
            try
            {
                string datetime = DateTime.Now.ToString("dddd, dd-MM-yyyy HH:mm:ss");
                DateTime sdt = Convert.ToDateTime(datetime);
                using (SqlConnection con = new SqlConnection(CS))
                {
                    
                    Random rdm = new Random();
                    SqlCommand cmd1 = new SqlCommand("select * from tbl_ViewsCount where UserId=@UserId and MovieId=@MovieId and Date=@Date", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@MovieId", views.MovieId);
                    cmd1.Parameters.AddWithValue("@UserId", views.UserId);
                    cmd1.Parameters.AddWithValue("@Date", sdt);
                    // cmd1.Parameters.AddWithValue("@MovieName", views.MovieName);

                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {

                        var m = new Message();
                        m.message = "exist";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                       
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("[SpViewscount]", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MovieId", views.MovieId);
                        cmd.Parameters.AddWithValue("@UserId", views.UserId);
                        cmd.Parameters.AddWithValue("@Date", sdt);
                        //  DateTime now = DateTime.Now;
                        //cmd.Parameters.AddWithValue("@Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

                        // cmd.Parameters.AddWithValue("@MovieName", views.MovieName);

                        //cmd.Parameters.AddWithValue("@Count", views.Count);

                        con.Open();
                        int k = cmd.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Added Succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                        }
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
        [HttpGet]
        public IEnumerable<UserModel> GetAllUserbyDate(DateTime dateFrom, DateTime DateTo)
        {
            List<UserModel> userlist = new List<UserModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("SPUserReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateFrom", dateFrom);
                cmd.Parameters.AddWithValue("@DateTo", DateTo);
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var user = new UserModel();

                    user.Id = Convert.ToInt32(rdr["ID"]);
                    user.PhoneNo = rdr["PhoneNo"].ToString();
                    user.Name = rdr["Name"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Location = rdr["Location"].ToString();

                    userlist.Add(user);


                }

            }
            return userlist;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_ViewsCountbydate(DateTime dateFrom, DateTime dateTo)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<ViewsCount> vlist = new List<ViewsCount>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[SPGetviewsCountbyDate]", con);
                // cmd.Parameters.AddWithValue("@MovieName", MovieName);
                cmd.Parameters.AddWithValue("@DateFrom", dateFrom);
                cmd.Parameters.AddWithValue("@DateTo", dateTo);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var v = new ViewsCount();
                        v.Count = Convert.ToInt32(rdr["Count"]);
                        //v.MovieName =rdr["MovieName"].ToString();


                        vlist.Add(v);


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, v);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Views  Found for this MovieId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_ViewsCount( int MovieId) 
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<ViewsCount> vlist = new List<ViewsCount>();
            using (SqlConnection con = new SqlConnection(CS))
            {
               con.Open();
                SqlCommand cmd = new SqlCommand("SPGetviewsCount", con);
               // cmd.Parameters.AddWithValue("@MovieName", MovieName);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var v = new ViewsCount();
                        v.Count = Convert.ToInt32(rdr["Count"]);
                        //v.MovieName =rdr["MovieName"].ToString();


                        vlist.Add(v);


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, v);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Views  Found for this MovieId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_Suggestedmovies()
        {
            HttpResponseMessage result = null;
            List<ShortModel> slist = new List<ShortModel>();


            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetSuggestedMovies", con);

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var sug = new ShortModel();

                        sug.Id = Convert.ToInt32(rdr["ID"]);
                        string movieurl = rdr["MoviePoster"].ToString();
                        sug.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        sug.MovieName= rdr["MovieName"].ToString();
                        slist.Add(sug);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, slist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "Some Thing went Wrong";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();

            }
            return result;

        }


        //public string GenerateOtp()
        //{
        //    string sOTP = String.Empty;

        //    int min = 1000;
        //    int max = 9999;


        //    Random rdm = new Random();
        //    sOTP = rdm.Next(min, max).ToString();

        //    return sOTP;
        //}
        //public void SenEmail(string otp)
        //{
        //string to = "alishaezad8805@gmail.com"; //To address    
        //string from = "alishaezad666@gmail.com"; //From address    
        //MailMessage message = new MailMessage(from, to);

        //string mailbody = "Your one time password is " + otp;
        //message.Subject = "Sending Email Using Asp.Net & C#";
        //    message.Body = mailbody;
        //    message.BodyEncoding = Encoding.UTF8;
        //    message.IsBodyHtml = true;
        //    SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
        //System.Net.NetworkCredential basicCredential1 = new
        //System.Net.NetworkCredential("alishaezad666@gmail.com", "Ali$$$444");
        //client.EnableSsl = true;
        //    client.UseDefaultCredentials = false;
        //    client.Credentials = basicCredential1;
        //    try
        //    {
        //        client.Send(message);
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        private WebProxy objProxy1 = null;
        string User = "IOTT";
        string password = "Nikku@1929";
        public string SendSMS(string Mobile_Number, string OTP)
        {
            string Message = "Your OTP is " + OTP + " to verify your number with us Enjoy the content in Harshitha production's IOTT";
            string stringpost = null;

            stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message;

            HttpWebRequest objWebRequest = null;
            HttpWebResponse objWebResponse = null;
            StreamWriter objStreamWriter = null;
            StreamReader objStreamReader = null;

            try
            {

                string stringResult = null;

                objWebRequest = (HttpWebRequest)WebRequest.Create("http://www.smscountry.com/SMSCwebservice_bulk.aspx");
                objWebRequest.Method = "POST";

                if ((objProxy1 != null))
                {
                    objWebRequest.Proxy = objProxy1;
                }


                // Use below code if you want to SETUP PROXY.
                //Parameters to pass: 1. ProxyAddress 2. Port
                //You can find both the parameters in Connection settings of your internet explorer.

                //WebProxy myProxy = new WebProxy("YOUR PROXY", PROXPORT);
                //myProxy.BypassProxyOnLocal = true;
                //wrGETURL.Proxy = myProxy;

                objWebRequest.ContentType = "application/x-www-form-urlencoded";

                objStreamWriter = new StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(stringpost);
                objStreamWriter.Flush();
                objStreamWriter.Close();

                objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();

                objStreamReader.Close();
                return stringResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
                objProxy1 = null;
            }
        }
      
        private static Random random = new Random();
        public static string GetRandomAlphaNumeric(string phoneNo)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray());
        }

        /*
        public static string RandomString(string Mobile_Number, int rc)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, rc)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        */
    }
}