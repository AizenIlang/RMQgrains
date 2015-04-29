using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public int ChequeNumber { get; set; }
        public Terms TermsOfPayment { get; set; }
        public DateTime DateofCheque { get; set; }

        public int PricePHP { get; set; }
        public int DeliveryID { get; set; }
       
        public string DRNumber { get; set; }
        public int? TransactionID { get; set; }
        
        public virtual Transaction Transactions { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
    }

    public enum Terms { Cash,Cheque};
}