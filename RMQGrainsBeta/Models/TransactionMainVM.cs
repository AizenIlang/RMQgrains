using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class TransactionMainVM
    {
        public int SONumber { get; set; }
        public int QuantityDelivered { get; set; }
        public string DeliveryTo { get; set; }
        public int PricePerSack { get; set; }
        public string DRNumber { get; set; }
        public int TotalExpenses { get; set; }
        public int Income { get; set; }
        public string BankName { get; set; }
        public int ChequeNumber { get; set; }


    }
}