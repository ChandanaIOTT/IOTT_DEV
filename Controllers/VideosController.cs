using IOTT_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using IOTT_API.Helper;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Drawing.Imaging;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using I_OTT_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace IOTT_API.Controllers
{
  //  [Authorize]
    public class VideosController : ApiController
    {
        // GET: Videos/PlayVideos
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllVideos()
        
        {
            HttpResponseMessage result = null;


            string icon = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllVideos", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                if (dt.Rows.Count > 0)
                {
                    List<VideoModel> vlist = new List<VideoModel>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        var video = new VideoModel();

                        video.Id = Convert.ToInt32(dt.Rows[i]["ID"]);
                        video.MovieName = dt.Rows[i]["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = dt.Rows[i]["MoviePoster"].ToString();
                        string webpster = dt.Rows[i]["WebPoster"].ToString();
                        //video.MoviePoster = "http://" + url + "/Images/" + movieurl;
                        //video.WebPoster = "http://" + url + "/Images/" + movieurl; 

                        //video.MoviePoster = "https://developmentbucketioot.s3.ap-south-1.amazonaws.com/Images/," + movieurl;
                        //video.WebPoster = "https://developmentbucketioot.s3.ap-south-1.amazonaws.com/" + webpster;
                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        video.WebPoster = "https://productionbucket.blob.core.windows.net/images/" + webpster;
                        //video.Quality360 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q360"].ToString();
                        //video.Quality480 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q480"].ToString();
                        //video.Quality720 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q720"].ToString();
                        //video.Quality1080 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q1080"].ToString();

                        video.Quality360 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q360"].ToString();
                        video.Quality480 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q480"].ToString();
                        video.Quality720 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q720"].ToString();
                        video.Quality1080 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q1080"].ToString();
                        //video.Quality360 = "https://d1cm35407ati9u.cloudfront.net/movies/" + dt.Rows[i]["Q360"].ToString();
                        //video.Quality480 = "https://d1cm35407ati9u.cloudfront.net/movies/" + dt.Rows[i]["Q480"].ToString();
                        //video.Quality720 = "https://d1cm35407ati9u.cloudfront.net/movies/" + dt.Rows[i]["Q720"].ToString();
                        //video.Quality1080 = "https://d1cm35407ati9u.cloudfront.net/movies/" + dt.Rows[i]["Q1080"].ToString();

                        video.Description = dt.Rows[i]["Description"].ToString();
                        video.Duration = dt.Rows[i]["Duration"].ToString();
                        video.Genre = dt.Rows[i]["Genre"].ToString();
                        video.IMDbRating = dt.Rows[i]["IMDbRating"].ToString();
                        video.Language = dt.Rows[i]["Language"].ToString();
                        video.PriceOfTheMovie = dt.Rows[i]["PriceOfTheMovie"].ToString();
                        video.Certificate = dt.Rows[i]["Certificate"].ToString();
                        video.Category = dt.Rows[i]["Category"].ToString();
                        video.Subtitles = dt.Rows[i]["Subtitles"].ToString();
                        video.ReleasedYear = dt.Rows[i]["ReleasedYear"].ToString();
                        video.MovieTrailer = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["MovieTrailer"].ToString();
                        video.MovieQuality = dt.Rows[i]["MovieQuality"].ToString();
                        video.IsPremium = Convert.ToBoolean(dt.Rows[i]["IsPremium"]);
                        video.IsDashBoardBanner = Convert.ToInt32(dt.Rows[i]["isbanner"]);


                        List<Cast> clist = new List<Cast>();

                        SqlCommand cmd1 = new SqlCommand("select * from MovieCast where MovieId='" + video.Id + "'", con);
                        cmd1.CommandType = CommandType.Text;

                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {


                            var cast1 = new Cast();
                            cast1.Charactor = dt1.Rows[j]["Charactor"].ToString();
                            cast1.Role = dt1.Rows[j]["CharctorRole"].ToString();
                            cast1.ImageURL = "https://productionbucket.blob.core.windows.net/images/" + dt1.Rows[j]["ImgURL"].ToString();


                            clist.Add(cast1);
                            video.cast = clist.ToList();

                        }


                        vlist.Add(video);



                    }

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                }
                else
                {
                    var m = new Message();
                    m.message = "No data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();

            }
            return result;

        }
        [Authorize]

        public HttpResponseMessage GetPurchasedMovies(int UserId)
        {
            HttpResponseMessage result = null;


            string icon = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetPurchasedMovies", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();
                List<VideoModel> vlist = new List<VideoModel>();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string date1 = dt.Rows[i]["DateTime"].ToString();
                        DateTime dt2 = Convert.ToDateTime(date1);
                        
                        DateTime myDate1 = dt2;
                        DateTime myDate2 = DateTime.Now;

                        TimeSpan myDateResult;

                        myDateResult = myDate2 - myDate1;
                        string days = myDateResult.Days.ToString();
                        if (Convert.ToInt32(days)<=3) { 
                       
                       
                        var video = new VideoModel();
                        video.Id = Convert.ToInt32(dt.Rows[i]["ID"]);
                        video.MovieName = dt.Rows[i]["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = dt.Rows[i]["MoviePoster"].ToString();
                        string webpster = dt.Rows[i]["WebPoster"].ToString();
                        //video.MoviePoster = "http://" + url + "/Images/" + movieurl;
                        //video.WebPoster = "http://" + url + "/Images/" + movieurl; 

                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        video.WebPoster = "https://productionbucket.blob.core.windows.net/images/" + webpster;
                        //video.Quality360 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q360"].ToString();
                        //video.Quality480 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q480"].ToString();
                        //video.Quality720 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q720"].ToString();
                        //video.Quality1080 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q1080"].ToString();

                        video.Quality360 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q360"].ToString();
                        video.Quality480 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q480"].ToString();
                        video.Quality720 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q720"].ToString();
                        video.Quality1080 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q1080"].ToString();

                        video.Description = dt.Rows[i]["Description"].ToString();
                        video.Duration = dt.Rows[i]["Duration"].ToString();
                        video.Genre = dt.Rows[i]["Genre"].ToString();
                        video.IMDbRating = dt.Rows[i]["IMDbRating"].ToString();
                        video.Language = dt.Rows[i]["Language"].ToString();
                        video.PriceOfTheMovie = dt.Rows[i]["PriceOfTheMovie"].ToString();
                        video.Certificate = dt.Rows[i]["Certificate"].ToString();
                        video.Category = dt.Rows[i]["Category"].ToString();
                        video.Subtitles = dt.Rows[i]["Subtitles"].ToString();
                        video.ReleasedYear = dt.Rows[i]["ReleasedYear"].ToString();
                        video.MovieTrailer = dt.Rows[i]["MovieTrailer"].ToString();
                        video.MovieQuality = dt.Rows[i]["MovieQuality"].ToString();
                        video.IsPremium = Convert.ToBoolean(dt.Rows[i]["IsPremium"]);
                        video.IsDashBoardBanner = Convert.ToInt32(dt.Rows[i]["isbanner"]);


                        List<Cast> clist = new List<Cast>();

                        SqlCommand cmd1 = new SqlCommand("select * from MovieCast where MovieId='" + video.Id + "'", con);
                        cmd1.CommandType = CommandType.Text;

                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {


                            var cast1 = new Cast();
                            cast1.Charactor = dt1.Rows[j]["Charactor"].ToString();
                            cast1.Role = dt1.Rows[j]["CharctorRole"].ToString();
                            cast1.ImageURL = "https://productionbucket.blob.core.windows.net/images/" + dt1.Rows[j]["ImgURL"].ToString();


                            clist.Add(cast1);
                            video.cast = clist.ToList();

                        }
                        

                        vlist.Add(video);

                        }
                        else
                        {

                        }
                    }

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                }
                else
                {
                    
                    var m = new Message();
                    m.message = "";
                    message.Add(m);
                    result=ControllerContext.Request.CreateResponse(HttpStatusCode.OK,vlist);

                }
                con.Close();

            }
            return result;

        }
       // [Authorize]
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteMovie(int id)
        {
            HttpResponseMessage result = null;

            List<VideoModel> llist = new List<VideoModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteMovie", con);
                cmd.Parameters.AddWithValue("@MovieId", id);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();
                if (k >= 0)
                {
                    var movie = new VideoModel();
                    var m = new Message();

                    m.message = "Movie deleted succesfully";
                    message.Add(m);
                    llist.Remove(movie);

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


        [Authorize]

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetVideoByid(int id)
        {
            HttpResponseMessage result = null;

            using (SqlConnection con = new SqlConnection(CS))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetAllVideosById", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    List<VideoModel> vlist = new List<VideoModel>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {




                        var video = new VideoModel();

                        video.Id = Convert.ToInt32(dt.Rows[i]["ID"]);
                        video.MovieName = dt.Rows[i]["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = dt.Rows[i]["MoviePoster"].ToString();
                        string webposter = dt.Rows[i]["WebPoster"].ToString();
                        // video.MoviePoster = "http://" + url + "/Images/" + movieurl;
                        //  video.WebPoster = "http://" + url + "/Images/" + movieurl;
                        //video.Quality360 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q360"].ToString();
                        //video.Quality480 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q480"].ToString();
                        //video.Quality720 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q720"].ToString();
                        //video.Quality1080 = "http://" + url + "/MoviesData/" + dt.Rows[i]["Q1080"].ToString();

                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        video.WebPoster = "https://productionbucket.blob.core.windows.net/images/" + webposter;

                        video.Quality360 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q360"].ToString();
                        video.Quality480 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q480"].ToString();
                        video.Quality720 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q720"].ToString();
                        video.Quality1080 = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["Q1080"].ToString();
                        video.Description = dt.Rows[i]["Description"].ToString();
                        video.Duration = dt.Rows[i]["Duration"].ToString();
                        video.Genre = dt.Rows[i]["Genre"].ToString();
                        video.IMDbRating = dt.Rows[i]["IMDbRating"].ToString();
                        video.Language = dt.Rows[i]["Language"].ToString();
                        video.PriceOfTheMovie = dt.Rows[i]["PriceOfTheMovie"].ToString();
                        video.Certificate = dt.Rows[i]["Certificate"].ToString();
                        video.Category = dt.Rows[i]["Category"].ToString();
                        video.Subtitles = dt.Rows[i]["Subtitles"].ToString();
                        video.ReleasedYear = dt.Rows[i]["ReleasedYear"].ToString();
                        video.MovieTrailer = "https://productionbucket.blob.core.windows.net/movies/" + dt.Rows[i]["MovieTrailer"].ToString();
                        video.MovieQuality = dt.Rows[i]["MovieQuality"].ToString();
                        video.IsPremium = Convert.ToBoolean(dt.Rows[i]["IsPremium"]);
                        video.IsDashBoardBanner = Convert.ToInt32(dt.Rows[i]["isbanner"]);

                        List<Cast> clist = new List<Cast>();

                        SqlCommand cmd1 = new SqlCommand("select * from MovieCast where MovieId='" + video.Id + "'", con);
                        cmd1.CommandType = CommandType.Text;

                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {


                            var cast1 = new Cast();
                            cast1.Charactor = dt1.Rows[j]["Charactor"].ToString();
                            cast1.Role = dt1.Rows[j]["CharctorRole"].ToString();
                            cast1.ImageURL = "https://productionbucket.blob.core.windows.net/images/" + dt1.Rows[j]["ImgURL"].ToString();


                            clist.Add(cast1);
                            video.cast = clist.ToList();

                        }


                        vlist.Add(video);



                    }

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                }
                else
                {
                    var m = new Message();
                    m.message = "No data Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();

            }
            return result;

        }

        [Authorize]

        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetByCategory(string category)
        {
            HttpResponseMessage result = null;
            List<ShortModel> vlist = new List<ShortModel>();


            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetByCategory", con);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.CommandType = CommandType.StoredProcedure;



                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var video = new ShortModel();

                        video.Id = Convert.ToInt32(rdr["ID"]);
                        video.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;

                        vlist.Add(video);
                        var newList1 = vlist.OrderByDescending(x => x.Id).ToList();


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, newList1);


                    }


                }
                else
                {
                    var m = new Message();
                    m.message = "no data in this category";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;

        }
        public int MaxId()
        {
            int ID = 0;
            SqlConnection con = new SqlConnection(CS);
            con.Open();
            SqlCommand cmd = new SqlCommand("Select MAX(Id) as ID from tbl_MovieData", con);

            cmd.CommandType = CommandType.Text;


            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    ID = Convert.ToInt32(rdr["ID"].ToString());

                }
            }
            return ID;

        }
        [Authorize]

        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetByLanguage(string language)
        {
            HttpResponseMessage result = null;
            List<ShortModel> vlist = new List<ShortModel>();

            string icon = "";
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetByLanguage", con);
                cmd.Parameters.AddWithValue("@Language", language);
                cmd.CommandType = CommandType.StoredProcedure;



                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var video = new ShortModel();


                        video.Id = Convert.ToInt32(rdr["ID"]);
                        video.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        vlist.Add(video);

                    }

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                }
                else
                {
                    var m = new Message();
                    m.message = "no data in this language";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        [Authorize]

        public HttpResponseMessage GetByGenre(string Name)
        {
            HttpResponseMessage result = null;
            List<ShortModel> vlist = new List<ShortModel>();


            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetByGenre", con);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.CommandType = CommandType.StoredProcedure;



                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var video = new ShortModel();

                        video.Id = Convert.ToInt32(rdr["ID"]);
                        video.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;

                        vlist.Add(video);
                        var newList = vlist.OrderByDescending(x => x.Id).ToList();

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, newList);

                    }

                }
                else
                {
                    var m = new Message();
                    m.message = "no data in this category";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Icon(string iconName)
        {
            List<VideoModel> vlist = new List<VideoModel>();
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var path = "~/MoviesData/" + "Screenshot (63).png";
            path = System.Web.Hosting.HostingEnvironment.MapPath(path);
            var ext = System.IO.Path.GetExtension(path);
            var contents = System.IO.File.ReadAllBytes(path);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(contents);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + ext);
            return response;

            // return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, new { response,vlist });
        }
        //public HttpResponseMessage playvideo([System.Web.Http.FromBody] string filename)
        //{
        //    string ext = "MP4";
        //    using (SqlConnection con = new SqlConnection(CS))
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_User", con);
        //        cmd.CommandType = CommandType.Text;
        //        con.Open();
        //    }
        //        var video = new VideoStream(filename);

        //    var response = Request.CreateResponse();
        //    response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/" + ext));


        //    return response;
        //}
        public class VideoStream
        {
            private readonly string _filename;

            public VideoStream(string filename)
            {
                _filename = "D:\\I OTT\\IOTT API\\MoviesData\\lake_evening_dusk_boat_pier_calm_585" + "." + "MP4";
            }

            public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
            {
                try
                {
                    var buffer = new byte[65536];

                    using (var video = System.IO.File.Open(_filename, FileMode.Open, FileAccess.Read))
                    {
                        var length = (int)video.Length;
                        var bytesRead = 1;

                        while (length > 0 && bytesRead > 0)
                        {
                            bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                            length -= bytesRead;
                        }
                    }
                }
                catch (HttpException ex)
                {
                    return;
                }
                finally
                {
                    outputStream.Close();
                }
            }
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage UploadMovie([FromBody] VideoModel video)
        {
            List<VideoModel> vlist = new List<VideoModel>();
            HttpResponseMessage result = null;
            Cast cast = new Cast();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SpUploadMovie", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MovieName", video.MovieName);
                    cmd.Parameters.AddWithValue("MoviePoster", video.MoviePoster);
                    cmd.Parameters.AddWithValue("WebPoster", video.WebPoster);
                    cmd.Parameters.AddWithValue("Genre", video.Genre);
                    cmd.Parameters.AddWithValue("Description", video.Description);
                    cmd.Parameters.AddWithValue("Duration", video.Duration);
                    cmd.Parameters.AddWithValue("IMDbRating", video.IMDbRating);
                    cmd.Parameters.AddWithValue("Language", video.Language);
                    cmd.Parameters.AddWithValue("PriceOfTheMovie", video.PriceOfTheMovie);
                    cmd.Parameters.AddWithValue("Certificate", video.Certificate);
                    cmd.Parameters.AddWithValue("Category", video.Category);
                    cmd.Parameters.AddWithValue("Subtitles", video.Subtitles);
                    cmd.Parameters.AddWithValue("ReleasedYear", video.ReleasedYear);
                    cmd.Parameters.AddWithValue("IsPremium", video.IsPremium);
                    cmd.Parameters.AddWithValue("isbanner", video.IsDashBoardBanner);

                    cmd.Parameters.AddWithValue("MovieTrailer", video.MovieTrailer);
                    cmd.Parameters.AddWithValue("MovieQuality", video.MovieQuality);
                    cmd.Parameters.AddWithValue("Quality360", video.Quality360);
                    cmd.Parameters.AddWithValue("Quality480", video.Quality480);
                    cmd.Parameters.AddWithValue("Quality720", video.Quality720);
                    cmd.Parameters.AddWithValue("Quality1080", video.Quality1080);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    int Id = MaxId();
                    foreach (Cast item in video.cast)
                    {
                        SqlCommand cmd1 = new SqlCommand("SPMovieCast", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Charactor", item.Charactor);
                        cmd1.Parameters.AddWithValue("@CharctorRole", item.Role);
                        cmd1.Parameters.AddWithValue("@ImgURL", item.ImageURL);
                        cmd1.Parameters.AddWithValue("@Id", Id);
                        con.Open();
                        int i = cmd1.ExecuteNonQuery();
                        con.Close();
                    }

                    var m = new Message();
                    m.message = "updated succefully";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
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
        [System.Web.Http.HttpPut]
        public HttpResponseMessage EditMovie([FromBody] VideoModel video)
        {
            HttpResponseMessage result = null;
            List<VideoModel> vlist = new List<VideoModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UspEdit_Movie", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Id", video.Id);
                cmd.Parameters.AddWithValue("MovieName", video.MovieName);
                cmd.Parameters.AddWithValue("MoviePoster", video.MoviePoster);
                cmd.Parameters.AddWithValue("WebPoster", video.WebPoster);
                cmd.Parameters.AddWithValue("Genre", video.Genre);
                cmd.Parameters.AddWithValue("Description", video.Description);
                cmd.Parameters.AddWithValue("Duration", video.Duration);
                cmd.Parameters.AddWithValue("IMDbRating", video.IMDbRating);
                cmd.Parameters.AddWithValue("Language", video.Language);
                cmd.Parameters.AddWithValue("PriceOfTheMovie", video.PriceOfTheMovie);
                cmd.Parameters.AddWithValue("Certificate", video.Certificate);
                cmd.Parameters.AddWithValue("Category", video.Category);
                cmd.Parameters.AddWithValue("Subtitles", video.Subtitles);
                cmd.Parameters.AddWithValue("ReleasedYear", video.ReleasedYear);
                cmd.Parameters.AddWithValue("IsPremium", video.IsPremium);
                cmd.Parameters.AddWithValue("isbanner", video.IsDashBoardBanner);

                cmd.Parameters.AddWithValue("MovieTrailer", video.MovieTrailer);
                cmd.Parameters.AddWithValue("MovieQuality", video.MovieQuality);
                cmd.Parameters.AddWithValue("Quality360", video.Quality360);
                cmd.Parameters.AddWithValue("Quality480", video.Quality480);
                cmd.Parameters.AddWithValue("Quality720", video.Quality720);
                cmd.Parameters.AddWithValue("Quality1080", video.Quality1080);
                int k = cmd.ExecuteNonQuery();
                if (k >= 0)
                {

                    var m = new Message();
                    m.message = "Movie updated succesfully";
                    message.Add(m);
                    vlist.Add(video);

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

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_MovieAnalytics([System.Web.Http.FromBody] MovieAnalysis views)
        {
            HttpResponseMessage result = null;

            List<MovieAnalysis> malist = new List<MovieAnalysis>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd1 = new SqlCommand("select * from tbl_movieAnalytics where UserId=@UserId and MovieName=@MovieName ", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@MovieName", views.MovieName);
                    cmd1.Parameters.AddWithValue("@UserId", views.UserId);

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
                        SqlCommand cmd = new SqlCommand("SPAddMovieAnalytics", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MovieName", views.MovieName);
                        cmd.Parameters.AddWithValue("@UserId", views.UserId);
                        DateTime now = DateTime.Now;
                        cmd.Parameters.AddWithValue("@Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

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
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_MovieAnalytics(string MovieName)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<ViewsCount> vlist = new List<ViewsCount>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetMovieAnalytics", con);
                // cmd.Parameters.AddWithValue("@MovieName", MovieName);
                cmd.Parameters.AddWithValue("@MovieName", MovieName);
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
                    m.message = "Something wrong ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_TrailerViews([System.Web.Http.FromBody] TrailerModel count)
        {
            HttpResponseMessage result = null;

            List<TrailerModel> vtlist = new List<TrailerModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd1 = new SqlCommand("select * from tbl_TrailerCount where UserId=@UserId and TrailerName=@TrailerName  ", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@UserId", count.UserId);
                    cmd1.Parameters.AddWithValue("@TrailerName", count.TrailerName);

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
                        SqlCommand cmd = new SqlCommand("SPTrailerCount", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", count.UserId);
                        cmd.Parameters.AddWithValue("@TrailerName", count.TrailerName);

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
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_TrailerViews(String Name)
        {

            HttpResponseMessage result = null;
            //string icon = "";

            List<ViewsCount> vlist = new List<ViewsCount>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetTrailerCount", con);
                // cmd.Parameters.AddWithValue("@MovieName", MovieName);
                cmd.Parameters.AddWithValue("@TrailerName", Name);
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



        [Authorize]

        [System.Web.Http.HttpPost]
        public HttpResponseMessage UploadTrailer([FromBody] MovieTrailer trailer)
        {
            List<MovieTrailer> vlist = new List<MovieTrailer>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPUploadTrailer", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("MovieName", trailer.MovieName);
                    cmd.Parameters.AddWithValue("TrailerVideo", trailer.TrailerVideo);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Trailer uploaded succesfully";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
            }
            return result;

        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_MoviesCount()
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<MovieCountModel> mclist = new List<MovieCountModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPCount", con);
                // cmd.Parameters.AddWithValue("@MovieName", MovieName);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var m = new MovieCountModel();
                        m.TotalMoviesCount = Convert.ToInt32(rdr["MoviesCount"]);
                        m.FreeMoviesCount=Convert.ToInt32(rdr["freeCount"]);
                        //v.MovieName =rdr["MovieName"].ToString();


                        mclist.Add(m);


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Data";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }
            return result;
        }


        [Authorize]

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetTrailerByid(string name )
        {
            HttpResponseMessage result = null;
            List<MovieTrailer> vlist = new List<MovieTrailer>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetTrailerById", con);
                cmd.Parameters.AddWithValue("@MovieName", name);
                cmd.CommandType = CommandType.StoredProcedure;


                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieTrailer", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var trailer = new MovieTrailer();

                        trailer.Id = Convert.ToInt32(rdr["ID"]);
                        trailer.MovieName = rdr["MovieName"].ToString();

                        string url = HttpContext.Current.Request.Url.Authority;

                        string trailervideo = rdr["TrailerVideo"].ToString();
                        trailer.TrailerVideo = "https://productionbucket.blob.core.windows.net/movies/" + trailervideo;


                        vlist.Add(trailer);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);
                    }


                }
                else
                {
                    var m = new Message();
                    m.message = "No Trailer Found ";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }
        [Authorize]

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllTrailer()
        {
            HttpResponseMessage result = null;
            List<MovieTrailer> vlist = new List<MovieTrailer>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetalltrailers", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieTrailer", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var trailer = new MovieTrailer();

                        trailer.Id = Convert.ToInt32(rdr["ID"]);
                        trailer.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        string trailervideo = rdr["TrailerVideo"].ToString();
                      //  trailer.TrailerVideo = "http://" + url + "/MoviesData/" + rdr["TrailerVideo"].ToString(); ;
                        trailer.TrailerVideo = "https://productionbucket.blob.core.windows.net/movies/" + rdr["TrailerVideo"].ToString(); ;



                        vlist.Add(trailer);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Trailer Found ";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage EditTrailer([FromBody] MovieTrailer trailer)
        {
            HttpResponseMessage result = null;
            List<MovieTrailer> vlist = new List<MovieTrailer>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPEditTrailer", con);
                cmd.Parameters.AddWithValue("@Id", trailer.Id);
                cmd.Parameters.AddWithValue("@MovieName", trailer.MovieName);
                cmd.Parameters.AddWithValue("@TrailerVideo", trailer.TrailerVideo);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    var m = new Message();
                    m.message = "Trailer updated succesfully";
                    message.Add(m);
                    vlist.Add(trailer);

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
        public HttpResponseMessage DeleteTrailer(int id)
        {
            HttpResponseMessage result = null;

            List<MovieTrailer> vlist = new List<MovieTrailer>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteTrailer", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    var trailer = new MovieTrailer();
                    var m = new Message();


                    m.message = "Trailer deleted succesfully";
                    message.Add(m);
                    vlist.Remove(trailer);

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
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post_UpcomingBanners([FromBody] UpcomingBanners banners)
        {
            List<UpcomingBanners> ub = new List<UpcomingBanners>();
            HttpResponseMessage result = null;

            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPUpComingBanners", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Poster", banners.Poster);
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

            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUpComingBanners()
        {
            HttpResponseMessage result = null;
            List<UpcomingBanners> ublist = new List<UpcomingBanners>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SpGetUpcommingBanners", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var ubs = new UpcomingBanners();
                        ubs.Id = Convert.ToInt32(rdr["ID"]);
                        string bannerurl = rdr["Poster"].ToString();
                        ubs.Poster = "https://productionbucket.blob.core.windows.net/images/" + bannerurl;
                        ublist.Add(ubs);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, ublist);

                    }
                }
                else
                {
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                con.Close();
            }
            return result;
        }
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete_UpcomingBanners(int Id)
        {
            HttpResponseMessage result = null;

            List<UpcomingBanners> ublist = new List<UpcomingBanners>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPDeleteUpcomingBanners", con);
                cmd.Parameters.AddWithValue("@Id ", Id);
                cmd.CommandType = CommandType.StoredProcedure;
                int k = cmd.ExecuteNonQuery();

                if (k == 1)
                {

                    var sds = new UpcomingBanners();
                    var m = new Message();

                    m.message = "deleted succesfully";
                    message.Add(m);
                    ublist.Remove(sds);

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


        [Authorize]


        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetDashBoardBanners()
        {
            HttpResponseMessage result = null;
            List<ImageModel> vlist = new List<ImageModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SpGetDashboardBanners", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var img = new ImageModel();


                        img.Id = Convert.ToInt32(rdr["ID"]);
                        img.Name = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        string movieurl = rdr["MoviePoster"].ToString();
                        string weburl = rdr["WebPoster"].ToString();

                        img.URL = "https://productionbucket.blob.core.windows.net/images/" + movieurl;
                        img.WebURL = "https://productionbucket.blob.core.windows.net/images/" + weburl;

                        img.IsPremium = Convert.ToBoolean(rdr["IsPremium"]);


                         vlist.Add(img);

                        //var newList = vlist.OrderByDescending(x => x.Id).ToList();

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                    }

                }
                else
                {
                    var m = new Message();
                    m.message = "no data in this category";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        // admin api 
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetFreeMovies()
        {
            HttpResponseMessage result = null;
            List<FreeModel> flist = new List<FreeModel>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SpGetFreeMovies", con);

                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_MovieData ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var fre = new FreeModel();


                        fre.Id = Convert.ToInt32(rdr["ID"]);
                        fre.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;

                        fre.Genre= rdr["Genre"].ToString();


                        flist.Add(fre);

                        //var newList = vlist.OrderByDescending(x => x.Id).ToList();

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, flist);

                    }

                }
                else
                {
                    var m = new Message();
                    m.message = "no Data";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> uploadImg(ImageType imageDetails)
        {
            HttpResponseMessage result = null;

            byte[] bytes = Convert.FromBase64String(imageDetails.Image);

            Image image;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);

            }
            // image.Save("~/ Images / " + imageDetails.Type, ImageFormat.Jpeg);
            var img = HttpContext.Current.Server.MapPath("~/Images/");


            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, img);

            return result;
        }


        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> UploadProfile()
        {
            string Message = "";

            HttpResponseMessage result = null;
            string url = HttpContext.Current.Request.Url.Authority;
            List<FileDetails> flist = new List<FileDetails>();
            try
            {

                var profileuploadpath = HttpContext.Current.Server.MapPath("~/Images/");
                var provider =
                new MultipartFormDataStreamProvider(profileuploadpath);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                }
                await Request.Content.ReadAsMultipartAsync(provider);
                string uploadingFileName = provider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
                string originalFileName = String.Concat(profileuploadpath, "\\" + (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                string filename = (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' });

                if (File.Exists(originalFileName))
                {
                    File.Delete(originalFileName);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                File.Move(uploadingFileName, originalFileName);
                Message = "";
                var f = new FileDetails();
                f.File = filename;
                f.URL = "http://" + url + "/Images/" + filename;
                flist.Add(f);
                result = Request.CreateResponse(HttpStatusCode.Created, filename);
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        [System.Web.Http.HttpPost]
        public HttpResponseMessage UploadImage()
        {
            HttpResponseMessage result = null;
            string url = HttpContext.Current.Request.Url.Authority;
            List<FileDetails> flist = new List<FileDetails>();
            try
            {

                var httpRequest = HttpContext.Current.Request;
                string filename = "";

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/Images/" + postedFile.FileName);
                        filename = postedFile.FileName;
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                        var f = new FileDetails();
                        f.File = filename;
                        f.URL = "http://" + url + "/Images/" + filename;
                        flist.Add(f);
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, flist);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<string> UploadVideoFile()
        {
            string Message = "";
            HttpResponseMessage result = null;
            string url = HttpContext.Current.Request.Url.Authority;
            List<FileDetails> flist = new List<FileDetails>();
            try
            {



                var fileuploadPath = HttpContext.Current.Server.MapPath("~/MoviesData/");
                var provider = new MultipartFormDataStreamProvider(fileuploadPath);
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                await content.ReadAsMultipartAsync(provider);
                string uploadingFileName = provider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
                string originalFileName = String.Concat(fileuploadPath, "\\" + (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                string filename = (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' });

                if (File.Exists(originalFileName))
                {
                    File.Delete(originalFileName);
                }

                File.Move(uploadingFileName, originalFileName);
                Message = "";
                var f = new FileDetails();
                f.File = filename;
                f.URL = "http://" + url + "/MoviesData/" + filename;
                flist.Add(f);
                result = Request.CreateResponse(HttpStatusCode.Created, flist);
                string FileDe = "{ FileName:" + f.File + ", URL:" + f.URL + " }";
                return FileDe;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [Authorize]

        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetMovieByName(string Name)
        {
            HttpResponseMessage result = null;
            IList<ShortModel> vlist = new List<ShortModel>();


            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetByName", con);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.CommandType = CommandType.StoredProcedure;



                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var video = new ShortModel();


                        video.Id = Convert.ToInt32(rdr["ID"]);
                        video.MovieName = rdr["MovieName"].ToString();
                        string url = HttpContext.Current.Request.Url.Authority;
                        string movieurl = rdr["MoviePoster"].ToString();
                        video.MoviePoster = "https://productionbucket.blob.core.windows.net/images/" + movieurl;



                        vlist.Add(video);



                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, vlist);

                }
                else
                {
                    var m = new Message();
                    m.message = "no data found ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                con.Close();
            }
            return result;

        }

    }
}