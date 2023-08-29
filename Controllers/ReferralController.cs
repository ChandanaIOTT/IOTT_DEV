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
using System.Web.Http;

namespace IOTT_API.Controllers
{

    public class ReferralController : ApiController
    {
        string CS = ConfigurationManager.ConnectionStrings["IOTT_DEVELOPMENT"].ConnectionString;
        List<Message> message = new List<Message>();
        Random random = new Random();
        [HttpPost]
        public HttpResponseMessage Get_referralcode([FromBody] ReferralModel rc)
        {
            List<ReferralModel> referralModelList = new List<ReferralModel>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(CS))
                {

                    connection.Open();
                    SqlCommand cmd = new SqlCommand("select *from tbl_Referralcode Where UserId=@UserId", connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", rc.UserId);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    connection.Close();
                    if (dt.Rows.Count > 0)
                    {
                        connection.Open();
                        SqlCommand cmd1 = new SqlCommand("SPGetReferralcode", connection);
                        cmd1.Parameters.AddWithValue("@UserId", rc.UserId);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var referral = new Ref();

                                referral.ReferralCode = rdr["Referralcode"].ToString();

                                result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referral);

                            }

                        }
                    }
                    else
                    {
                        string empty = string.Empty;
                        string randomAlphaNumeric = this.GetRandomAlphaNumeric();
                        SqlCommand sqlCommand = new SqlCommand("SPReferralCode", connection);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@UserId", rc.UserId);
                        sqlCommand.Parameters.AddWithValue("@ReferralCode", randomAlphaNumeric);
                        connection.Open();
                        int num = sqlCommand.ExecuteNonQuery();
                        connection.Close();
                        if (num == 1)
                        {
                            //var m = new Message();

                            //m.message = randomAlphaNumeric;
                            // message.Add(m);
                            var referral = new Ref();

                            referral.ReferralCode = randomAlphaNumeric;
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referral);
                        }
                        else
                        {
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);
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

        public string GetRandomAlphaNumeric()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray());

        }
        [HttpGet]
        public HttpResponseMessage Is_Valid(string ReferralCode)
        {

            HttpResponseMessage result = null;
            string icon = "";

            List<Ref> referralList = new List<Ref>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPIs_Valid", con);
                cmd.Parameters.AddWithValue("@Referralcode", ReferralCode);
                cmd.CommandType = CommandType.StoredProcedure;

                //SqlCommand cmd = new SqlCommand("SELECT * FROM  ", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {


                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, true);
                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "Something went wrong ";
                    message.Add(m);
                    result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, false);
                }
                con.Close();
            }
            return result;
        }


        [HttpPost]
        public HttpResponseMessage Validate_Referral([FromBody] Referral r)
        {
            HttpResponseMessage result = null;
            List<Referral> referralList = new List<Referral>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {

                    SqlCommand cmd = new SqlCommand("[SpReferredBy]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReferralCode", r.ReferralCode);
                    cmd.Parameters.AddWithValue("@PhoneNo", r.MobileNo);
                    cmd.Parameters.AddWithValue("@RefferedBy", r.RefferedBy);

                    con.Open();
                    
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    if (k >0)
                    {
                        Message m = new Message();
                        m.message = "uploaded succesfully";
                        message.Add(m);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    else
                    {

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
        public HttpResponseMessage Get_referredreward(int UserId)
        {
            List<ReferralBonus> referralBonusList = new List<ReferralBonus>();
            HttpResponseMessage result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("SPGetRefferedReward", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@UserId", UserId);
                    SqlDataReader rdr = cmd1.ExecuteReader();
                    var rb = new ReferralBonus();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                           
                            rb.Id = Convert.ToInt32(rdr["Id"]);
                            rb.UserId = Convert.ToInt32(rdr["UserId"]);

                            rb.Amonut = rdr["BonusAmount"].ToString();
                            referralBonusList.Add(rb);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referralBonusList);

                        }
                        con.Close();
                    }
                    else
                    {
                        rb.Id = 0;
                        rb.UserId = 0;

                        rb.Amonut = null;
                        referralBonusList.Add(rb);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referralBonusList);

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
        public HttpResponseMessage Get_ReferredByreward(int UserId)
        {
            HttpResponseMessage result = null;
            List<ReferralBonus> referralBonusList = new List<ReferralBonus>();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[SPGetRefferedByReward]", con);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    var v = new ReferralBonus();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                           
                            v.Id = Convert.ToInt32(rdr["Id"]);
                            v.UserId = Convert.ToInt32(rdr["UserId"]);
                            v.RefferelCode = rdr["ReffrelByCode"].ToString();
                            
                            v.Amonut = rdr["Amount"].ToString();
                            referralBonusList.Add(v);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referralBonusList);
                        }
                    }
                    else
                    {

                        v.Id = 0;
                        v.UserId = 0;

                        v.Amonut = null;
                        referralBonusList.Add(v);

                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, referralBonusList);
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
        [HttpPost]
        public HttpResponseMessage Scratch_Referred([FromBody] Referralscratch rs)
        {
            HttpResponseMessage result = null;
            List<Referralscratch> RefferelDetails = new List<Referralscratch>();
            try
            {
                SqlConnection connection = new SqlConnection(CS);
                connection.Open();
               SqlCommand cmd1 = new SqlCommand("Select * from tbl_Referralcode where UserId=@UserId", connection);
                cmd1.Parameters.AddWithValue("@UserId", rs.UserId);
                cmd1.CommandType = CommandType.Text;
                SqlDataAdapter rdr =new  SqlDataAdapter(cmd1);
                DataTable dt = new DataTable();
                rdr.Fill(dt);

                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("SPScratched", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", rs.UserId);
                    cmd.Parameters.AddWithValue("@ReferralCode", rs.Referralcode);
                    int Bonus = 0;
                    if (dt.Rows[0]["BonusAmount"].ToString() != "")
                    {
                         Bonus = Convert.ToInt32(dt.Rows[0]["BonusAmount"]) + 3;
                    }
                    else
                    {
                         Bonus =  3;
                    }
                    
                    cmd.Parameters.AddWithValue("@TotalAmount", Bonus);
                    con.Open();
                    int k = cmd.ExecuteNonQuery();
                    rs.BonusAmount = 3;
                    RefferelDetails.Add(rs);
                    con.Close();
                    if (k >0)
                    {
                        
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, RefferelDetails);
                        
                    }
                    else
                    {
                       
                        Message m = new Message();
                        m.message = "No Data Found";
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
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { message });
            }
            return result;

        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Scratch_Referredby([FromBody] Referralscratch rbs)
        {
            HttpResponseMessage result = null;

            List<Referralscratch> RefferelDetails = new List<Referralscratch>();
            try
            {

             

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    SqlCommand cmd1 = new SqlCommand("select BonusAmount from tbl_Referralcode where UserId=@UserId", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@UserId", rbs.UserId);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd1);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        var Referred = new Referred();
                        SqlCommand cmd2 = new SqlCommand("[SPRefferedByScratched]", con);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@UserId", rbs.UserId);
                        cmd2.Parameters.AddWithValue("@ReferralCode", rbs.Referralcode);
                        cmd2.Parameters.AddWithValue("@Id", rbs.Id);
                        
                        int Bonus = 0;
                        if (dt.Rows[0]["BonusAmount"].ToString() != "")
                        {
                            Bonus = Convert.ToInt32(dt.Rows[0]["BonusAmount"]) + 3;
                        }
                        else
                        {
                            Bonus = 3;
                        }
                        cmd2.Parameters.AddWithValue("@TotalAmount", Bonus);
                        rbs.BonusAmount = 3;
                       
                        con.Open();
                        int k = cmd2.ExecuteNonQuery();
                        con.Close();
                        if (k > 0)
                        {
                            RefferelDetails.Add(rbs);
                            Message m = new Message();
                            m.message = " scratched succesfully";
                            message.Add(m);
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, RefferelDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var m1 = new Message();
                m1.message = ex.Message;
                message.Add(m1);
                result = ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest,  message );
            }
            return result;

        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get_Rewardhistory(int UserId)
        {
            HttpResponseMessage result = null;
            List<ReferralHistory> rhlist = new List<ReferralHistory>();


            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SPRewardHistory", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);


                cmd.CommandType = CommandType.StoredProcedure;
                //SqlCommand cmd = new SqlCommand("SELECT * FROM LanguageTable", con);
                //cmd.CommandType = CommandType.Text;
                //con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        var rh = new ReferralHistory();
                        rh.UserId = Convert.ToInt32(rdr["UserId"]);
                        rh.Referralcode = rdr["ReffrelCode"].ToString();
                        if (rdr["Amount"] == DBNull.Value)
                        {
                            result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, rhlist);
                        }
                        else
                        {
                            rh.BonusAmount = Convert.ToInt32(rdr["Amount"]);

                        }
                        rhlist.Add(rh);
                        result = ControllerContext.Request.CreateResponse(HttpStatusCode.OK, rhlist);

                    }
                }
                else
                {
                    var m = new Message();
                    m.message = "Nothing Found";
                    message.Add(m);
                    ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, message);

                }
                return result;
            }


        }
    }
}


