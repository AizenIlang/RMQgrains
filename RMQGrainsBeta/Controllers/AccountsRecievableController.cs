using RMQGrainsBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMQGrainsBeta.Controllers
{
    public class AccountsRecievableController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountsRecievable
        public ActionResult Index(DateTime? DateFrom, DateTime? DateTo)
        {
            List<AccountsRecievable> ARList = new List<AccountsRecievable>();
            AccountsRecievable AR = new AccountsRecievable();
            Cement cement = new Cement();
            Payment payment = new Payment();
            Delivery delivery = new Delivery();
            int TotalAmount = 0;

            if (DateFrom != null && DateTo != null)
            {
                foreach (var pay in db.Payments.Where(t => t.DateofCheque > DateFrom && t.DateofCheque < DateTo))
                {
                    AR = new AccountsRecievable();
                    AR.DateProcessed = DateTime.Now;
                    AR.CustomerName = pay.Name;
                    AR.Amount = pay.PricePHP; // Not sure About this
                    AR.BankName = pay.BankName;
                    AR.Branch = pay.Branch;
                    AR.ChequeNumber = pay.ChequeNumber;
                    AR.DateOfCheque = pay.DateofCheque;

                    ARList.Add(AR);
                    TotalAmount += AR.Amount;

                }
            }
            else
            {
                foreach (var pay in db.Payments.Include("Delivery"))
                {
                    AR = new AccountsRecievable();
                    AR.DateProcessed = pay.Delivery.DateOfArrival;
                    AR.CustomerName = pay.Name;
                    AR.Amount = pay.PricePHP * pay.Delivery.QuantityToDeliver;
                    AR.BankName = pay.BankName;
                    AR.Branch = pay.Branch;
                    AR.ChequeNumber = pay.ChequeNumber;
                    AR.DateOfCheque = pay.DateofCheque;

                    ARList.Add(AR);
                    TotalAmount += AR.Amount;

                }
            }


            ViewBag.TotalAmount = String.Format("{0:0,0}", TotalAmount); 
            return View(ARList.ToList());
        }
    }
}