using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQGrainsBeta.Models
{
    public class Delivery
    {
        public int DeliveryID { get; set; }
        public DateTime DateofCreation { get; set; }
        public HaulType Hauling { get; set; }
        public string HaulerCompany { get; set; }
        public string PlateNumber { get; set; }
        public string DriverName { get; set; }
        public int QuantityToDeliver { get; set; }
        public Dest Destination { get; set; }
        public string DestinationTo { get; set; }

        public DateTime DateOfArrival { get; set; }
        public string DeliveryTo { get; set; }
        public string DRNumber { get; set; }
        public int CementID { get; set; }
        public int? TransactionID { get; set; }

        public virtual Transaction Transactions { get; set; }
        public virtual Cement Cement { get; set; }
    }
    public enum HaulType { Own,Rental};
    public enum Dest { Delivery,Storage};
}