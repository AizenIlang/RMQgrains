using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public string Description { get; set; }
        public EXP Expenses { get; set; }
        public int Amount { get; set; }
        public int PaymentID { get; set; }
        
        public int Total { get; set; }
        
        public virtual Payment Payment { get; set; }

      
    }

    public enum EXP { Hauling,Labor};

    
}