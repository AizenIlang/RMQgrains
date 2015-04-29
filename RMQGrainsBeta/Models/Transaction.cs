using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
            
        public int CementID { get; set; }
        public int DeliveryID { get; set; }
        public int? PaymentID { get; set; }

        

        
        

    }
}