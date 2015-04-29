using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class ExpenseIndexViewModel
    {
        [Display(Name="SO Number")]
        public int SONumber { get; set; }
        public DateTime DateProcessed { get; set; }
        public Status Stats { get; set; }
        public int TotalExpense { get; set; }

    }
}