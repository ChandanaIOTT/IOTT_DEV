using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace I_OTT_API.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public byte IsSubscribed { get; set; }
        public string SubsType { get; set; }
        public string DeviceName { get; set; }
        public string ActiveDeviceQuantity { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy HH:mm:ss}")]
        public DateTime SubDateTime { get; set; }
        public string ProfileImg { get; set; }
        public string OTP { get; set; }
        public byte[] PhotoData { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }


    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
    public class Profile
    {
        public int UserId { get; set; }
    }
    public class UserDevice
    {
        public string DeviceName { get; set; }

        public int UserId { get; set; }
    }
    public class FavoriteModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public string MovieName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dddd, dd-MM-yyyy HH:mm:ss}")]
        public DateTime Date { get; set; }

    }
    public class MovieAnalysis
    {
        public int UserId { get; set; }
        public string MovieName { get; set; }

    }
    public class ProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string ProfileImg { get; set; }
        public byte[] ProfilePic { get; set; }

       public string DeviceName { get; set; }

      //  public int UserId { get; set; }



    }
    public class FCMModel
    {
        public int Id { get; set; }

        public string Fcm_Token { get; set; }
    }
    public class FCM
    {
        public int Id { get; set; }
        public string Fcm_Token { get; set; }

        public int UserId { get; set; }
    }
    public class WalletModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public int Wallet_BalAmount { get; set; }

    }
    public class Phno
    {
        public string PhoneNo { get; set; }
    }
    public class Wallet
    {
        public int Amount { get; set; }
    }
    public class BonusAmnt
    {
        public int BonusAmount { get; set; }
    }
    public class ViewsCount
    {
        public int Count { get; set; }

        //public String MovieName { get; set; }
    }
    public class FeedBackModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FeedBack { get; set; }
        public int Rating { get; set; }
    }
    public class Qrmodel
    {
        public int UserId { get; set; }
        public BitmapByteQRCode QRCode { get; set; }
        public string Time { get; set; }
    }
    public class ReferralModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Referralcode { get; set; }

        public string ReferredBy { get; set; }

        public int BonusAmount { get; set; }


    }
    public class Referralscratch
    {
        public int UserId { get; set; }

        public string Referralcode { get; set; }
        public int Id  { get; set; }

        public int BonusAmount { get; set; }
    }
    public class ReferralHistory
    {
        public int UserId { get; set; }

        public string Referralcode { get; set; }

         public int BonusAmount { get; set; }
    }
    public class Referral
    {
        public string MobileNo { get; set; }
        public string ReferralCode { get; set; }
        public string RefferedBy { get; set; }

    }
    public class ReferralBonus
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefferelCode { get; set; }
        public string Amonut { get; set; }
    }
    
    public class Ref
    {
        public string ReferralCode { get; set; }
    }
    public class PurchaseMovieCount
    {
       public String MovieName { get; set; }

       public int MovieCount { get; set; }
       
      
    }
    public class Referred
    {
        public int BonusAmount { get; set; }

    }
}
    