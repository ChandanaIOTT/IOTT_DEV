
using I_OTT_API.Models;
using IOTT_API.Models;
using System.Collections.Generic;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Collections.Specialized;
using CCA.Util;
using System.IO;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using TokenBasedAuthentication.Models;

namespace IOTT_API.Controllers
{
    [Authorize]

    public class PaymentController : ApiController
    {
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        // GET: Payement
        [Authorize]

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Wallet([System.Web.Http.FromBody] WalletModel w)
        {
            HttpResponseMessage result = null;

            List<WalletModel> wlist = new List<WalletModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select Wallet_BalAmount from dbo.tbl_Wallet Where UserId=@UserId", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", w.UserId);

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        var wallet = new Wallet();

                        SqlCommand cmd2 = new SqlCommand("SPUpdateWallet", con);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        wallet.Amount = Convert.ToInt32(dt.Rows[0]["Wallet_BalAmount"]);
                        cmd2.Parameters.AddWithValue("@UserId", w.UserId);
                        cmd2.Parameters.AddWithValue("@Wallet_BalAmount", w.Wallet_BalAmount + wallet.Amount);

                        con.Open();
                        int k = cmd2.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Succsfully added payement";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                        }
                    }
                    else
                    {
                        SqlCommand cmd1 = new SqlCommand("SPWallet", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@UserId", w.UserId);
                        cmd1.Parameters.AddWithValue("@Wallet_BalAmount", w.Wallet_BalAmount);
                        con.Open();
                        int k = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (k == 1)
                        {
                            var m = new Message();
                            m.message = "Succsfully added";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }
            return result;
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_Wallet(int UserId)
        {
            HttpResponseMessage result = null;
            string icon = "";
            List<Wallet> wlist = new List<Wallet>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetWallet", con);
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
                        var w = new Wallet();
                        w.Amount = Convert.ToInt32(rdr["Wallet_BalAmount"]);
                        wlist.Add(w);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, w);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "no data found";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                return result;

            }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_PurchaseMoviecount(int MovieId)
        {
            HttpResponseMessage result = null;
            List<PurchaseMovieCount> pmclist = new List<PurchaseMovieCount>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPPurchaseMovieCount", con);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var v = new PurchaseMovieCount();
                        //v.MovieName = rdr["MovieName"].ToString();

                        v.MovieCount = Convert.ToInt32(rdr["Count"]);


                        pmclist.Add(v);


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, v);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "No Count  Found for this MovieId";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                }
                con.Close();
            }

            return result;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_BonusAmnt(int UserId)
        {
            HttpResponseMessage result = null;
            string icon = "";
            List<Wallet> wlist = new List<Wallet>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select BonusAmount from tbl_Referralcode where UserId=@UserId", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var w = new BonusAmnt();
                        if (rdr["BonusAmount"].ToString() != "")
                        {
                            w.BonusAmount = Convert.ToInt32(rdr["BonusAmount"]);
                        }
                        else
                        {
                            w.BonusAmount = 0;
                        }

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, w);
                    }
                }
                else
                {
                    var w = new BonusAmnt();
                    w.BonusAmount = 0;
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, w);
                }
                return result;

            }
        }
        public int GetwalletAmount(int UserId)
        {

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SPGetWallet", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                int Amount = 0;
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Wallet_BalAmount"].ToString() != "" && dt.Rows[0]["Wallet_BalAmount"].ToString() != null)
                    {
                        Amount = Convert.ToInt32(dt.Rows[0]["Wallet_BalAmount"]);
                    }

                }

                con.Close();
                if (Amount > 0)
                {
                    return Amount;
                }
                else
                {
                    return 0;
                }
            }

        }

        public int GetBonusAmount(int UserId)
        {

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SPGetBonuusAmount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                int BonusAmount = 0;
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["BonusAmount"].ToString() != "" && dt.Rows[0]["BonusAmount"].ToString() != null)
                    {
                        BonusAmount = Convert.ToInt32(dt.Rows[0]["BonusAmount"]);
                    }
                    con.Close();

                }
                if (BonusAmount > 0)
                {
                    return BonusAmount;
                }
                else
                {
                    return 0;
                }
            }

        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Purchase_Movie([System.Web.Http.FromBody] PurchaseModel p)
        {
            int WalletAmount = 0;
            int BonusAmount = 0;
            HttpResponseMessage result = null;
            List<PurchaseModel> wplist = new List<PurchaseModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    int bonusamnt = 0;
                    if (p.UseBonus == "YES")
                    {
                        bonusamnt = GetBonusAmount(p.UserId);
                    }
                    else
                    {
                        bonusamnt = 0;
                    }

                    int walletamnt = GetwalletAmount(p.UserId);
                    int actualPurchaseAmnt = 0;
                    if (walletamnt + bonusamnt >= p.PurchaseAmount) {
                        if (bonusamnt <= 5)
                        {
                            BonusAmount = 0;
                            actualPurchaseAmnt = p.PurchaseAmount - bonusamnt;
                            WalletAmount = walletamnt - actualPurchaseAmnt;
                        }
                        else
                        {
                            if (p.UseBonus == "YES")
                            {
                                BonusAmount = bonusamnt - 5;
                                actualPurchaseAmnt = p.PurchaseAmount - 5;
                                WalletAmount = walletamnt - actualPurchaseAmnt;
                            }
                            else
                            {

                                WalletAmount = walletamnt - p.PurchaseAmount;
                            }

                        }


                        var purchase = new PurchaseModel();

                        SqlCommand cmd2 = new SqlCommand("SPPurchaseMovie", con);
                        cmd2.CommandType = CommandType.StoredProcedure;

                        cmd2.Parameters.AddWithValue("@UserId", p.UserId);
                        cmd2.Parameters.AddWithValue("@PurchaseAmount", p.PurchaseAmount);

                        cmd2.Parameters.AddWithValue("@Wallet_BalAmount", WalletAmount);
                        cmd2.Parameters.AddWithValue("@BonusAmount", BonusAmount);

                        cmd2.Parameters.AddWithValue("@MovieId", p.MovieId);

                        DateTime now1 = DateTime.Now;
                       // cmd2.Parameters.AddWithValue("@DateTime", now1.ToShortDateString());
                        cmd2.Parameters.AddWithValue("@DateTime", now1.ToString());


                        con.Open();
                        int k = cmd2.ExecuteNonQuery();
                        con.Close();
                        if (k > 0)
                        {
                            var m = new Message();
                            m.message = "Movie Purchased succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, m);
                        }

                    }
                    else
                    {
                        var m1 = new Message();
                        m1.message = "Insufficiant balance";
                        message.Add(m1);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
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
        public HttpResponseMessage Get_OrderId()
        {
            List<OrderGenerate> olist = new List<OrderGenerate>();
            HttpResponseMessage result = null;
            string User = "IOTT_";
            string orderId = null;
            List<OrderIdModel> om = new List<OrderIdModel>();
            var om1 = new OrderIdModel();
            // var dateAndTime = DateTime.Now;
            // var Date = dateAndTime.ToString("dd-MM-yyyy").Split();
            try
            {

                using (SqlConnection con = new SqlConnection(CS))
                {

                    DateTime DateTime1 = DateTime.Now.Date;

                    var year = DateTime1.Year;
                    var month = DateTime1.Month;
                    var day = DateTime1.Day;
                    string oredrid = "" + year + month + day;

                    con.Open();
                    string empty = string.Empty;
                    SqlCommand cmd = new SqlCommand("SP_GetOrderId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        int Id = ((int)dt.Rows[0]["Id"]);
                        om1.OrderId = oredrid + Id;
                    }
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, om1);
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
        public HttpResponseMessage Get_Purchase_history(int UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<PurchaseHistory> wlist = new List<PurchaseHistory>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetPurchaseHistory", con);
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
                        var p = new PurchaseHistory();

                        p.MovieId = Convert.ToInt32(rdr["MovieId"]);
                        p.UserId = Convert.ToInt32(rdr["UserId"]);
                        p.PurchaseAmount = Convert.ToInt32(rdr["PurchaseAmount"]);
                        p.MovieName = rdr["MovieName"].ToString();
                        p.DateTime = rdr["DateTime"].ToString();

                        wlist.Add(p);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, wlist);

                    }
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
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Is_purchase(int UserId, int MovieId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<IsPurchase> flist = new List<IsPurchase>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPIs_Purchase", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@MovieId", MovieId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);


                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
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
                            if (Convert.ToInt32(days) <= 3)
                            {
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK,1);
                            }
                            else
                            {
                                var m = new Message();
                                m.message = "";
                                message.Add(m);
                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, 0);
                            }
                        }

                    }
                    else
                    {
                   
                    }
                    con.Close();
                }

                else
                {
                    var m = new Message { };
                    m.message = "Not Purchased";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest,m );
                    { }

                }
                return result;
            }
        }
            
        public HttpResponseMessage GetPaymentStatus(string OrderId,int UserId)
        {
            HttpResponseMessage result = null;
            string icon = "";
            SqlConnection con = new SqlConnection(CS);

            try
            {

                //          workingKey = '6E824886AB948C420B7D6E0E2B9333F6',
                //accessCode = 'AVVJ56IK18AD79JVDA',
                //test   string accessCode = "AVAL57IK77BH75LAHB";//from avenues
                string accessCode = "AVFS95JG21AO55SFOA";//from avenues production
                                                         // string workingKey = "86567DA6FF41E6BEC421DEE713376AF5";// from avenues test
                string workingKey = "F02D57D470DB95DFA712A398DD1B36D4";// from avenues production
                //string s=""+ReffNo+"|"+OrderId+"|";
                //string orderStatusQuery = "104003051888|86218925|"; // Ex.= CCAvenue Reference No.|Order No.|
                string orderStatusQuery =  "|" + OrderId + "|"; // Ex.= CCAvenue Reference No.|Order No.|
                string encQuery = "";

               // string queryUrl = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans"; //test
               // string queryUrl = "https://secure.ccavenue.com/apis/servlet/DoWebTrans";//production
                string queryUrl = "https://login.ccavenue.com/apis/servlet/DoWebTrans";

                CCACrypto ccaCrypto = new CCACrypto();
                encQuery = ccaCrypto.Encrypt(orderStatusQuery, workingKey);

                // make query for the status of the order to ccAvenues change the command param as per your need
                string authQueryUrlParam = "enc_request=" + encQuery + "&access_code=" + accessCode + "&command=orderStatusTracker&request_type=STRING&response_type=STRING";

                // Url Connection
                String message = postPaymentRequestToGateway(queryUrl, authQueryUrlParam);
                //Response.Write(message);
                NameValueCollection param = getResponseMap(message);
                String status = "";
                String encRes = "";
                if (param != null && param.Count == 2)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if ("status".Equals(param.Keys[i]))
                        {
                            status = param[i];
                        }
                        if ("enc_response".Equals(param.Keys[i]))
                        {
                            encRes = param[i];
                            //Response.Write(encResXML);
                        }
                    }


                    if (!"".Equals(status) && status.Equals("0"))
                    {
                        String ResString = ccaCrypto.Decrypt(encRes, workingKey);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, ResString);
                        //Response.Write(ResString);
                        string[] words = ResString.Split('|');

                        foreach (var word in words)
                        {
                            string status1= words[1].ToString();
                            string TrancId= words[2].ToString();
                            string PaymentReff= words[3].ToString();
                            string Amount= words[37].ToString();
                            if (status1 == "Shipped")
                            {
                                SqlCommand cmd = new SqlCommand("[SPAddTransactionDataAndroid]", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserId", UserId);
                                cmd.Parameters.AddWithValue("@OrderId", OrderId);
                                cmd.Parameters.AddWithValue("@TransactionId", TrancId);
                                cmd.Parameters.AddWithValue("@paymentRef", PaymentReff);
                                cmd.Parameters.AddWithValue("@Amount", Amount);
                                cmd.Parameters.AddWithValue("@Status","Success");
                                // cmd.Parameters.AddWithValue("@DateOftrans",Params[40]);
                                cmd.Parameters.AddWithValue("@DateOftrans", DateTime.Now.ToString());
                                con.Open();
                                int k = cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                SqlCommand cmd = new SqlCommand("[SPAddTransactionData]", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserId", UserId);
                                cmd.Parameters.AddWithValue("@OrderId", OrderId);
                                cmd.Parameters.AddWithValue("@TransactionId", TrancId);
                                cmd.Parameters.AddWithValue("@paymentRef", PaymentReff);
                                cmd.Parameters.AddWithValue("@Amount", Amount);
                                cmd.Parameters.AddWithValue("@Status", status1);
                                cmd.Parameters.AddWithValue("@DateOftrans", DateTime.Now.ToString());
                                //cmd.Parameters.AddWithValue("@MovieId", MovieId);
                                con.Open();
                                int k = cmd.ExecuteNonQuery();
                            }
                        }


                    }
                    else if (!"".Equals(status) && status.Equals("1"))
                    {
                        Console.WriteLine("failure response from ccAvenues: " + encRes);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, encRes);

                      

                    }

                }


            }
            catch (Exception exp)
            {
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, exp);

            }

            return result; 
        }


        public HttpResponseMessage GetWebPaymentStatus(string TransactionId, string OrderId)
        {
            HttpResponseMessage result = null;
            string icon = "";


            try
            {

      //          workingKey = '6E824886AB948C420B7D6E0E2B9333F6',
      //accessCode = 'AVVJ56IK18AD79JVDA',
                //test   string accessCode = "AVAL57IK77BH75LAHB";//from avenues
                string accessCode = "AVVJ56IK18AD79JVDA";//from avenues production
                                                         // string workingKey = "86567DA6FF41E6BEC421DEE713376AF5";// from avenues test
                string workingKey = "6E824886AB948C420B7D6E0E2B9333F6";// from avenues production
                //string s=""+ReffNo+"|"+OrderId+"|";
                //string orderStatusQuery = "104003051888|86218925|"; // Ex.= CCAvenue Reference No.|Order No.|
                string orderStatusQuery = "" + TransactionId + "|" + OrderId + "|"; // Ex.= CCAvenue Reference No.|Order No.|
                string encQuery = "";

                  string queryUrl = "https://apitest.ccavenue.com/apis/servlet/DoWebTrans"; //test
                //string queryUrl = "https://secure.ccavenue.com/apis/servlet/DoWebTrans";//production
                //string queryUrl = "https://login.ccavenue.com/apis/servlet/DoWebTrans";

                CCACrypto ccaCrypto = new CCACrypto();
                encQuery = ccaCrypto.Encrypt(orderStatusQuery, workingKey);

                // make query for the status of the order to ccAvenues change the command param as per your need
                string authQueryUrlParam = "enc_request=" + encQuery + "&access_code=" + accessCode + "&command=orderStatusTracker&request_type=STRING&response_type=STRING";

                // Url Connection
                String message = postPaymentRequestToGateway(queryUrl, authQueryUrlParam);
                //Response.Write(message);
                NameValueCollection param = getResponseMap(message);
                String status = "";
                String encRes = "";
                if (param != null && param.Count == 2)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if ("status".Equals(param.Keys[i]))
                        {
                            status = param[i];
                        }
                        if ("enc_response".Equals(param.Keys[i]))
                        {
                            encRes = param[i];
                            //Response.Write(encResXML);
                        }
                    }
                    if (!"".Equals(status) && status.Equals("0"))
                    {
                        String ResString = ccaCrypto.Decrypt(encRes, workingKey);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, ResString);
                        //Response.Write(ResString);
                    }
                    else if (!"".Equals(status) && status.Equals("1"))
                    {
                        Console.WriteLine("failure response from ccAvenues: " + encRes);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, encRes);
                    }

                }

            }
            catch (Exception exp)
            {
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, exp);

            }

            return result;
        }
   
        [System.Web.Http.HttpGet]
        public HttpResponseMessage SattleMent(int UserId)
        {
            SqlConnection con = new SqlConnection(CS);
            HttpResponseMessage result = null;
     
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetLasttenUsers", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                   
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string orderid = dt.Rows[i]["Order_Id"].ToString();
                       

                        GetPaymentStatus(orderid, UserId);
                        var m1 = new Message();
                        m1.message = "Success";
                        message.Add(m1);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                }

                var m2 = new Message();
                m2.message = "Not Any Remaining transaction to be sattled";
                message.Add(m2);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
            }
            catch (Exception ex)
            {
                var m1 = new Message();
                m1.message = "Something Went Wrong";
                message.Add(m1);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
            }
            return result;
        
        }
      [System.Web.Http.HttpGet]
        public HttpResponseMessage GetReciept(string OrderId)
        {
            HttpResponseMessage result = null;

            List<TransactionModel> wlist = new List<TransactionModel>();
            var  t =new TransactionModel();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPGetReciept", con);
                    cmd.Parameters.AddWithValue("@OrderId", OrderId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            t.Id = Convert.ToInt32(rdr["Id"]);
                            t.UserId = Convert.ToInt32(rdr["UserId"]);
                            //t.MovieId = Convert.ToInt32(rdr["MovieId"]);

                            t.Order_Id = rdr["Order_Id"].ToString();
                            t.TrancId = rdr["TrancId"].ToString();
                            t.PayementRef = rdr["PaymentRef"].ToString();
                            t.Amount = rdr["Amount"].ToString();
                            t.Status = rdr["Status"].ToString();
                            t.DateTime = rdr["DateTime"].ToString();
                        }
                    }
                    con.Close();
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, t);



                    // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message, decryptedParameters);
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
        public HttpResponseMessage TransactionDetails([System.Web.Http.FromBody] TransactionModel t)
        {
            HttpResponseMessage result = null;

            List<TransactionModel> wlist = new List<TransactionModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                  
                    cmd.Parameters.AddWithValue("@UserId", t.UserId);
                    cmd.Parameters.AddWithValue("@TrancId", t.TrancId);
                    cmd.Parameters.AddWithValue("@PaymentRef", t.PayementRef);
                    cmd.Parameters.AddWithValue("@Amount", t.Amount);
                    cmd.Parameters.AddWithValue("@Status", t.Status);
                    DateTime now = DateTime.Now;
                    cmd.Parameters.AddWithValue("@DateTime", now.ToShortDateString());
                    cmd.Parameters.AddWithValue("@Order_Id", t.Order_Id);  
                    
                  

                    con.Open();
                    int k = cmd.ExecuteNonQuery();

                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Transaction Succsfully added";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message, decryptedParameters);
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
        public HttpResponseMessage GetAllPayementHistory()
        {
            List<TransactionModel> wlist = new List<TransactionModel>();
            HttpResponseMessage result = null;

            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_TransactionDetails", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var t = new TransactionModel();


                    t.Id = Convert.ToInt32(rdr["Id"]);
                    t.UserId = Convert.ToInt32(rdr["UserId"]);
                    //t.MovieId = Convert.ToInt32(rdr["MovieId"]);

                    t.Order_Id = rdr["Order_Id"].ToString();
                    t.TrancId = rdr["TrancId"].ToString();
                    t.PayementRef = rdr["PaymentRef"].ToString();
                    t.Amount = rdr["Amount"].ToString();
                    t.Status = rdr["Status"].ToString();
                    t.DateTime = rdr["DateTime"].ToString();
                    wlist.Add(t);

                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, wlist);

                }
                con.Close();
            }
            return result;
        }

        public HttpResponseMessage InitiateTransaction([System.Web.Http.FromBody] OrderGenerate t)
        {
            HttpResponseMessage result = null;

           
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPInitiateTransaction", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                  
                    cmd.Parameters.AddWithValue("@UserId", t.UserId);
                    cmd.Parameters.AddWithValue("@OrderId", t.OrderId); 
                   
                    cmd.Parameters.AddWithValue("@Amount", t.Amount);
                  
  
                    con.Open();
                    int k = cmd.ExecuteNonQuery();

                    con.Close();
                    if (k == 1)
                    {
                        var m = new Message();
                        m.message = "Transaction Initiated";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    // result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message, decryptedParameters);
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
        public HttpResponseMessage Get_TransactionDetails(int UserId)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<TransactionModel> wlist = new List<TransactionModel>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPGetTransaction_Details", con);
                 cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM transaction_details ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var t = new TransactionModel();


                         t.Id = Convert.ToInt32(rdr["Id"]);
                         t.UserId = Convert.ToInt32(rdr["UserId"]);
                         //t.MovieId = Convert.ToInt32(rdr["MovieId"]);

                         t.Order_Id = rdr["Order_Id"].ToString();
                         t.TrancId = rdr["TrancId"].ToString();
                         t.PayementRef = rdr["PaymentRef"].ToString();
                         t.Amount = rdr["Amount"].ToString();
                         t.Status = rdr["Status"].ToString();
                         t.DateTime = rdr["DateTime"].ToString();


                        wlist.Add(t);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, wlist);

                    }
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


        private string postPaymentRequestToGateway(String queryUrl, String urlParam)
        {

            String message = "";
            try
            {
                StreamWriter myWriter = null;// it will open a http connection with provided url
                WebRequest objRequest = WebRequest.Create(queryUrl);//send data using objxmlhttp object
                objRequest.Method = "POST";
                //objRequest.ContentLength = TranRequest.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";//to set content type
                myWriter = new System.IO.StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(urlParam);//send data
                myWriter.Close();//closed the myWriter object

                // Getting Response
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();//receive the responce from objxmlhttp object 
                using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
                {
                    message = sr.ReadToEnd();
                    //Response.Write(message);
                }
            }
            catch (Exception exception)
            {
                Console.Write("Exception occured while connection." + exception);
            }
            return message;

        }

        private NameValueCollection getResponseMap(String message)
        {
            NameValueCollection Params = new NameValueCollection();
            if (message != null || !"".Equals(message))
            {
                string[] segments = message.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
            }
            return Params;
        }

    }
}