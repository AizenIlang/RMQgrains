using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class DeliveryPaymentViewModel
    {
        public Delivery DeliveryModel  { get; set; }
        public Payment PaymentModel { get; set; }

    }
}