using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class CementViewModels
    {

        public virtual Cement Cements { get; set; }
        public virtual Delivery Deliveries { get; set; }
        public virtual Transaction Transactions { get; set; }
    }


    
    
    
 
}