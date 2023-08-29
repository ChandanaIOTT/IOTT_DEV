using I_OTT_API.Models;
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
using System.Xml.Linq;
using TokenBasedAuthentication.Models;

namespace IOTT_API.Controllers
{
     [Authorize]

    public class ShortVideoController : ApiController
    {
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllShortVideo()
        {
            HttpResponseMessage result = null;
            List<GetShort> sclist = new List<GetShort>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAlllShortVideo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var sf = new GetShort();
                        sf.ShortId = Convert.ToInt32(rdr["Id"]);
                        if (rdr["Movieid"] == DBNull.Value)
                        {

                        }
                        else
                        {
                            sf.MovieId = Convert.ToInt32(rdr["Movieid"]);

                        }
                        string url = HttpContext.Current.Request.Url.Authority;

                        string shortmovieurl = rdr["ShortVideo"].ToString();
                        sf.Short = "https://productionbucket.blob.core.windows.net/shortvideos" + shortmovieurl;
                        if(rdr["ShortVideo"]== DBNull.Value)
                        {

                        }
                        else
                        {
                            sf.Description = rdr["ShortVideo"].ToString();

                        }
                        sclist.Add(sf);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, sclist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }


       [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_ShortVideo([FromBody] ShortVideoModel sv)
        {
            List<ShortVideoModel> svlist = new List<ShortVideoModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPPostShortVideo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MovieId", sv.MovieId);
                    cmd.Parameters.AddWithValue("ShortVideo", sv.ShortVideo);
                  

                    //   cmd.Parameters.AddWithValue("Likes", sv.Likes);
                    //  cmd.Parameters.AddWithValue("Comments", sv.Comments);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "uploaded succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddShortFavorite([System.Web.Http.FromBody] Sc fav)
        {
            HttpResponseMessage result = null;

            List<Sc> sflist = new List<Sc>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {

                    SqlCommand cmd1 = new SqlCommand("select * from tbl_FavShorts where UserId=@UserId and ShortId=@ShortId ", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@UserId", fav.UserId);
                    cmd1.Parameters.AddWithValue("@ShortId", fav.ShortId);

                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand("SPShortUnFavorite", con);
                        cmd.Parameters.AddWithValue("@UserId", fav.UserId);
                        cmd.Parameters.AddWithValue("@ShortId", fav.ShortId);
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

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);

                        }
                    }
                    else
                    {


                        SqlCommand cmd = new SqlCommand("SPAddShortFavorite", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", fav.UserId);

                        cmd.Parameters.AddWithValue("@ShortId", fav.ShortId);


                        con.Open();
                        int k = cmd.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = " Short Added to Favorites";
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
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetShortsFavorite(string UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<FavShortModel> sflist = new List<FavShortModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetFavoriteShorts", con);
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
                        var getfavorite = new FavShortModel();


                        getfavorite.ShortId = Convert.ToInt32(rdr["ID"]);
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["ShortVideo"].ToString();
                        getfavorite.ShortVideo = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        

                        sflist.Add(getfavorite);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, sflist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Favorite Shorts  Found for this UserId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_Shortcomment([FromBody] ShortVideoComment svc)
        {
            List<ShortVideoComment> svclist = new List<ShortVideoComment>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPComment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ShortVideoId", svc.ShortVideoId);
                    cmd.Parameters.AddWithValue("@UserId", svc.UserId);
                    cmd.Parameters.AddWithValue("@ShortComment", svc.Shortcomment);
                    DateTime now11 = DateTime.Now;
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Comment added  succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage DeleteComment([System.Web.Http.FromBody] Sc DeleteComment)
        {
            HttpResponseMessage result = null;
            List<Sc> clist = new List<Sc>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("select Id from tblShortVideoData where UserId=@userId", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", DeleteComment.UserId);

                    con.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand cmd1 = new SqlCommand("SPDeleteComment", con);
                        cmd1.Parameters.AddWithValue("@UserId", DeleteComment.UserId);
                        cmd1.Parameters.AddWithValue("@ShortVideoId", DeleteComment.ShortId);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        int i = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (i == 1)
                        {
                            var deletecomment = new Sc();
                            var m = new Message();
                            m.message = "Deleted Comment succesfully";
                            message.Add(m);

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                        var m = new Message();
                        m.message = "No data";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

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
        // get favorite 
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllShortVideoComments(int Id)
        {
            HttpResponseMessage result = null;
            List<ShortComments> scvlist = new List<ShortComments>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetShortComments", con);
                cmd.Parameters.AddWithValue("@VideoId", Id);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var ss = new ShortComments();

                        ss.Comments = rdr["ShortComment"].ToString();
                        ss.Name = rdr["Name"].ToString();
                        if (rdr["Pic"] == DBNull.Value)
                        {
                            // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, userlist);

                        }
                        else
                        {
                            ss.Profilepic = (byte[])rdr["Pic"];
                        }
                        ss.Date = rdr["Date"].ToString();


                        scvlist.Add(ss);


                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, scvlist);

                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllCommentsCount(int Id)
        {
            HttpResponseMessage result = null;
            List<ViewsCount> scflist = new List<ViewsCount>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPCommentcount", con);
                cmd.Parameters.AddWithValue("@VideoId", Id);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var slf = new ViewsCount();

                        slf.Count = Convert.ToInt32(rdr["Comments"]);



                        scflist.Add(slf);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, slf);

                    }

                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_PersonalShort([FromBody] PersonalShort ps)
        {
            List<PersonalShort> pslist = new List<PersonalShort>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPAddPersonalshort", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Shorts", ps.Shorts);
                    cmd.Parameters.AddWithValue("@Description", ps.Description);
                    cmd.Parameters.AddWithValue("@UserId", ps.UserId);
                    DateTime now11 = DateTime.Now;
                    cmd.Parameters.AddWithValue("@Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "uploaded succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
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
        public HttpResponseMessage GetPersonal_shortbyuserid(int UserId)
        {
            HttpResponseMessage result = null;
            List<PersonalShort> pgs = new List<PersonalShort>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetPsbyuserid", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);


                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var ps = new PersonalShort();

                        ps.Id = Convert.ToInt32(rdr["ID"]);
                        string url = HttpContext.Current.Request.Url.Authority;

                        string shortmovieurl = rdr["Shortvideo"].ToString();

                        ps.Shorts = "https://productionbucket.blob.core.windows.net/shortvideos" + shortmovieurl;
                        ps.Description = rdr["Description"].ToString();
                        ps.Time = rdr["Time"].ToString();
                        ps.UserId = Convert.ToInt32(rdr["UserId"]);
                        ps.UserName = rdr["Name"].ToString();



                        pgs.Add(ps);


                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, pgs);

                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    result=ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllShortLikes(int Id)
        {
            HttpResponseMessage result = null;
            List<ViewsCount> scllist = new List<ViewsCount>();
            try
            {

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SPGetShortLikes", con);
                    cmd.Parameters.AddWithValue("@VideoId", Id);

                    cmd.CommandType = CommandType.StoredProcedure;

                    //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                    //cmd.CommandType = CommandType.Text;
                    //con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var sl = new ViewsCount();

                            //  ss.Id = Convert.ToInt32(rdr["ID"]);
                            sl.Count = Convert.ToInt32(rdr["likes"]);



                            scllist.Add(sl);

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, sl);

                        }

                    }
                    else
                    {
                        var m = new Message();
                        m.message = "No Data Found";
                        message.Add(m);
                        ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);


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

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete_Short(int MovieId)
        {
            HttpResponseMessage result = null;

            List<ShortVideoModel> svlist = new List<ShortVideoModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteShorts", con);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)
                {

                    var sd = new ShortVideoModel();
                    var m = new Message();

                    m.message = "deleted succesfully";
                    message.Add(m);
                    svlist.Remove(sd);

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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage ShortscommentLikeunlike([FromBody] ShortCommentslikes svc)
        {
            List<ShortCommentslikes> svcllist = new List<ShortCommentslikes>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPCommentLikeUnlike", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ShortVideoId", svc.ShortVideoId);
                    cmd.Parameters.AddWithValue("UserId", svc.UserId);
                    cmd.Parameters.AddWithValue("ShortcommentLike", svc.ShortcommentLike);
                    cmd.Parameters.AddWithValue("ShortCommentUnlike", svc.ShortcommentUnlike);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k > 0)
                    {
                        var m = new Message();
                        m.message = "added succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
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
        public HttpResponseMessage GetShortsCommentsLikesUnlikes(int id)
        {
            HttpResponseMessage result = null;
            List<ShortsCommentslikesUnlikes> svclulist = new List<ShortsCommentslikesUnlikes>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetCommentLikeUnlike", con);
                cmd.Parameters.AddWithValue("@ShortVideoId", id);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {

                        var ss = new ShortsCommentslikesUnlikes();

                        ss.ShortscommentLike = Convert.ToInt32(rdr["ShortCommentLike"]);

                        ss.ShortscommentUnlike = Convert.ToInt32(rdr["ShortCommentUnlike"]);


                        svclulist.Add(ss);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, svclulist);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }

        }


        [System.Web.Http.HttpPost]
        public HttpResponseMessage Short_Like([FromBody] ShortLikeModel slf)
        {
            List<ShortLikeModel> sllist = new List<ShortLikeModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd1 = new SqlCommand("select Likes from tblShortLikes where UserId=@UserId and ShortVideoId=@ShortVideoId ", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@UserId", slf.UserId);
                    cmd1.Parameters.AddWithValue("@ShortVideoId", slf.ShortId);

                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand("SPShortUnLike", con);
                        cmd.Parameters.AddWithValue("@UserId", slf.UserId);
                        cmd.Parameters.AddWithValue("@ShortVideoId", slf.ShortId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                        if (i == 1)
                        {
                            var getfavorite = new favModel();
                            var m = new Message();
                            //m.message = "unliked video Sucessfully";
                            message.Add(m);

                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 0);

                        }
                    }
                    else
                    {
                        SqlCommand cmd2 = new SqlCommand("SPShortLike", con);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("ShortVideoId", slf.ShortId);
                        cmd2.Parameters.AddWithValue("UserId", slf.UserId);
                        cmd2.Parameters.AddWithValue("Likes", slf.Likes);
                        con.Open();
                        int k = cmd2.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                          //  m.message = "uploaded succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 1);
                        }
                        else
                        {

                        }
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
        public HttpResponseMessage Get_ShortVideoByid(int id)
        {
            List<ShortVideoModel> scplist = new List<ShortVideoModel>();

            HttpResponseMessage result = null;

            using (SqlConnection con = new SqlConnection(CS))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetShortVideoById", con);
                cmd.Parameters.AddWithValue("@VideoId", id);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    List<ShortVideoModel> scvlist = new List<ShortVideoModel>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var sf = new ShortVideoModel();


                        sf.ShortId = Convert.ToInt32(dt.Rows[i]["Id"]);
                        sf.MovieId = Convert.ToInt32(dt.Rows[i]["MovieId"]);
                        string url = HttpContext.Current.Request.Url.Authority;

                        string shortmovieurl = dt.Rows[i]["ShortVideo"].ToString();

                        sf.ShortVideo = "https://productionbucket.blob.core.windows.net/shortvideos" + shortmovieurl;
                        sf.Comments = Convert.ToInt32(dt.Rows[i]["Comment"]);
                        sf.Likes = Convert.ToInt32(dt.Rows[i]["likes"]);


                        scplist.Add(sf);
                    }
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, scplist);

                }




                else
                {
                    var m = new Message();
                    m.message = "No Data Found";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();
            }
                return result;
                
            }
        }
    } 
