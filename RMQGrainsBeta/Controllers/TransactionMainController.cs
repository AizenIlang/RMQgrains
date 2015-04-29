using RMQGrainsBeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMQGrainsBeta.Controllers
{
    public class TransactionMainController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: TransactionMain
        public ActionResult Index(int SOnumberPass)
        {
            TransactionMainVM transactionVM = new TransactionMainVM();

            List<TransactionMainVM> transactionVMList = new List<TransactionMainVM>();
            Cement cement = new Cement();
            Delivery delivery = new Delivery();
            Payment payment = new Payment();
            int TotalExpense = 0;
            int TotalIncome = 0;
            cement = db.Cements.Where(t => t.SOnumber == SOnumberPass).FirstOrDefault();
            List<Cement> listCement = new List<Cement>();
            foreach (Cement asdf in db.Cements.Where(t => t.TransactionID == cement.TransactionID))
            {
                listCement.Add(asdf);
            }


            foreach (Cement fullSO in listCement)
            {
                TotalExpense = 0;
                TotalIncome = 0;
                foreach (Delivery del in db.Deliveries.Include("Cement").Where(t => t.Cement.CementID == fullSO.CementID))
                {
                    transactionVM.SONumber = del.Cement.SOnumber;
                    transactionVM.QuantityDelivered = del.QuantityToDeliver;
                    transactionVM.DeliveryTo = del.DeliveryTo;
                    payment = new Payment();
                    payment = db.Payments.Include("Expenses").Where(t => t.DeliveryID == del.DeliveryID).FirstOrDefault();
                    transactionVM.PricePerSack = payment.PricePHP;
                    transactionVM.DRNumber = delivery.DRNumber;
                    foreach (Expense exp in payment.Expenses)
                    {
                        TotalExpense += exp.Total;

                    }

                    TotalIncome += payment.PricePHP * del.QuantityToDeliver;
                    transactionVM.BankName = payment.BankName;
                    transactionVM.TotalExpenses = TotalExpense;
                    transactionVM.Income = TotalIncome;
                    transactionVM.ChequeNumber = payment.ChequeNumber;
                    transactionVMList.Add(transactionVM);

                }
            }
          

            

            return View(transactionVMList.ToList());
        }
    }
}