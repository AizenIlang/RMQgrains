using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RMQGrainsBeta.Models
{
    public class CementIndexViewModels
    {
        public int SONumber { get; set; }
        public bool Renewed { get; set; }
        public int TransactionID { get; set; }
        public int CurrentStock { get; set; }
        public int OriginalStock { get; set; }
        public int QuantitySold { get; set; }
        public DateTime DateProcessed { get; set; }
        public Status Stats { get; set; }
        public DateTime DateClosed { get; set; }
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public int TotalIncome { get; set; }
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public int GrossIncome { get; set; }
        public string Remarks { get; set; }
    }
}
