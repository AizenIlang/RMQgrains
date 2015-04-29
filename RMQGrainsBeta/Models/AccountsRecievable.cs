using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class AccountsRecievable
    {
        public DateTime DateProcessed { get; set; }
        public string CustomerName { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public int ChequeNumber { get; set; }
        public DateTime DateOfCheque { get; set; }
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public int Amount { get; set; }

    }
}