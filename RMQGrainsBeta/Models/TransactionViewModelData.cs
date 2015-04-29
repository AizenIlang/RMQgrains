using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class TransactionViewModelData
    {
        public int SONumber { get; set; }
        public string DeliveryTo { get; set; }
        public string BankName { get; set; }
        public List<Expense> ExpenseList { get; set; }

    }
}