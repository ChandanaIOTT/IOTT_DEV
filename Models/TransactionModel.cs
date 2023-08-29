using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOTT_API.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public string TrancId { get; set; }
        public string Order_Id { get; set; }
        public string PayementRef { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
        public string DateTime { get; set; }
       // public string encResp { get; set; }

    }

    public class PurchaseModel
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int PurchaseAmount { get; set; }
        public string UseBonus { get; set; }
       
        

    }

    public class OrderIdModel
    { 
   public string OrderId { get; set; }
    }
        public class PurchaseHistory
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int PurchaseAmount { get; set; }
        public string MovieName { get; set; }
        public string DateTime { get; set; }

    }
    public class OrderGenerate
    {
        public int UserId { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
    }
    public class IsPurchase
    {
        public int UserId { get; set; }

        public int MovieId { get; set; }

    }
}