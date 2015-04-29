using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class PaymentViewModel
    {
        public int PaymentID { get; set; }
        public int PricePHP { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public Terms TermsOfPayment { get; set; }
        public DateTime DateofCheque { get; set; }
        public int DeliveryID { get; set; }
        public int TransactionID { get; set; }
        
        public List<Expense> Expenses { get; set; }
    }
}