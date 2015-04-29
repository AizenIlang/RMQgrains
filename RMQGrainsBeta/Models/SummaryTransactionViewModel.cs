using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class SummaryTransactionViewModel
    {
        public int SONumber { get; set; }
        public int QuantityToDeliver { get; set; }
        public string CustomerName { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public int ChequeNumber { get; set; }
        public DateTime DateOfCheque { get; set; }
        public Status Stats { get; set; }
        public string Remarks { get; set; }
        public List<Expense> Expenses { get; set; }
        public List<Expense> ExpensesAsWhole { get; set; }
        public int Income { get; set; }
        public int TotalIncome { get; set; }
        public int CurrentStock { get; set; }
        public DateTime DateProcessed { get; set; }
        public List<Cement> RelatedCementList { get; set; }

    }

    public enum Status { Pending,Cleared,Closed};
}