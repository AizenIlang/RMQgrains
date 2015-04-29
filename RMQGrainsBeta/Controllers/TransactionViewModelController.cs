using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMQGrainsBeta.Models;

namespace RMQGrainsBeta.Controllers
{
    public class TransactionViewModelController : Controller
    {
        // GET: TransactionViewModel
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            TransactionViewModel TransVM = new TransactionViewModel();
            TransactionViewModelData TransData = new TransactionViewModelData();
            List<TransactionViewModelData> TransList = new List<TransactionViewModelData>();
            foreach(Transaction trn in db.Transactions)
            {
                var SOnum = db.Cements.Find(trn.CementID);
                var Del = db.Deliveries.Find(trn.DeliveryID);
                var Pay = db.Payments.Find(trn.PaymentID);
                List<Expense> explist = new List<Expense>();
                if (Pay != null)
                {
                    explist = db.Expenses.Where(t => t.PaymentID == Pay.PaymentID).ToList();
                    
                }
                else
                {
                    explist.Add(new Expense() { Amount = 0, Description = "Nothing", ExpenseID = 0, Expenses = EXP.Hauling, PaymentID = 0, Total = 0 });
                    Pay = new Payment { BankName = "", Branch = "", DateofCheque = DateTime.Now, DeliveryID = 0, DRNumber = "", Name = "", PaymentID = 0, TermsOfPayment = 0 };
                }
               
                TransList.Add(new TransactionViewModelData { DeliveryTo = Del.DeliveryTo,BankName=Pay.BankName,SONumber=SOnum.SOnumber,ExpenseList = explist });
                

                

                
            }

            TransVM.TransactionList = TransList;

            return View(TransVM);
        }
    }
}