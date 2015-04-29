using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class Cement
    {
        public int CementID { get; set; }
        public DateTime DateCreation { get; set; }
        public int RenewalPayment { get; set; }
        public int? TransactionID { get; set; }
        public bool Renewed { get; set; }
        public int SOnumber { get; set; }
        public Status Stats{ get; set; }
        public string CompanyName { get; set; }
        public string BuyingTo { get; set; }
        public DateTime CurrentDate { get; set; }
        public string CementType { get; set; }
        public int TotalQuantity { get; set; }

        public PayMethod PaymentMethod { get; set; }
        public int ChequeNumber { get; set; }

        public string Remarks { get; set; }
        
        public string ImageLocation { get; set; }

        
        public virtual Transaction Transactions { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }

    }

    public enum PayMethod { Cash,Cheque};
    
}