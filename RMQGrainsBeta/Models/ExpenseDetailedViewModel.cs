using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class ExpenseDetailedViewModel
    {
        public string Description { get; set; }
        public int Amount { get; set; }
        public int QuantityTobeDelivered { get; set; }
        public string Destination { get; set; }
        public DateTime DateOfExpenseCreated { get; set; }
        public int Total { get; set; }

    }
}