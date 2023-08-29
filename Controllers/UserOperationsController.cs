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

    public class UserOperationsController : ApiController
    {

        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        // GET: UserOperations

        [System.Web.Http.HttpPost]
        public HttpResponseMessage UserHelp([System.Web.Http.FromBody] UserOperationModel issue)
        {
            HttpResponseMessage result = null;

            List<UserOperationModel> flist = new List<UserOperationModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPLoginIssues", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Issue", issue.Issue);

                    cmd.Parameters.AddWithValue("@Number", issue.Number);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Issue Added succesfully";
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
        public HttpResponseMessage GetUserHelp()
        {
            HttpResponseMessage result = null;
            List<UserOperationModel> fblist = new List<UserOperationModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetLoginIssues", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_LoginIssues ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var issue = new UserOperationModel();

                        issue.Id = Convert.ToInt32(rdr["ID"]);
                        issue.Issue = rdr["Issue"].ToString();
                        issue.Number = rdr["Number"].ToString();


                        fblist.Add(issue);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, fblist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }

        [System.Web.Http.HttpPost]

        public HttpResponseMessage AddFollower([System.Web.Http.FromBody] UserFollowModel u)
        {
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SP_AddFollower", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", u.UserId);

                    cmd.Parameters.AddWithValue("@FollowerId", u.FollowerId);

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Added to The Following";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    else
                    {
                        var m = new Message();
                        m.message = "You are already Following";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                }
            }
            catch (Exception ex)
            {
                var m = new Message();
                m.message = ex.Message;
                message.Add(m);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
            }

            return result;

        }

        public HttpResponseMessage UpdateUsername([System.Web.Http.FromBody] UserFollowModel u)
        {
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("[SP_AddUsername_UpdateUsername]", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", u.UserId);
                    cmd.Parameters.AddWithValue("@Username", u.Username);



                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Username Updated Successfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    else
                    {
                        var m = new Message();
                        m.message = "Username not Updated ";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                }
            }
            catch (Exception ex)
            {
                var m = new Message();
                m.message = ex.Message;
                message.Add(m);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
            }

            return result;

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUserProfile(int UserId)
        {
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("getFollowerFollowing", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    SqlDataAdapter adt = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adt.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        var umodel = new UserProfile();

                        umodel.following = ds.Tables[0].Rows[0]["Following"].ToString();
                        umodel.follower = ds.Tables[1].Rows[0]["Follower"].ToString();
                        umodel.Username = ds.Tables[2].Rows[0]["Username"].ToString();


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, umodel);

                    }
                    else
                    {
                        var m1 = new Message();
                        m1.message = "No Follower";
                        message.Add(m1);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
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



        [System.Web.Http.HttpDelete]
        public HttpResponseMessage RemoveFollower([System.Web.Http.FromBody] UserFollowModel u)
        {
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SPRemoveFollower", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", u.UserId);
                    cmd.Parameters.AddWithValue("@FollowerId", u.FollowerId);
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    if (i > 0)
                    {
                        var m1 = new Message();
                        m1.message = "Unfollowed";
                        message.Add(m1);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
                    }
                    else
                    {
                        var m1 = new Message();
                        m1.message = "Something went Wrong";
                        message.Add(m1);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
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





    }
}



