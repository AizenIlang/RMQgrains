using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMQGrainsBeta.Models;
namespace RMQGrainsBeta.Controllers
{
    public class SummaryTransactionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: SummaryTransaction
        public ActionResult Index(int? SOnumberPass)
        {
            SummaryTransactionViewModel summaryTransactionVM = new SummaryTransactionViewModel();

            Cement cement = new Cement();
            Delivery deliver = new Delivery();
            Payment payment = new Payment();
            int TotalExpense = 0;
            int TotalAsWhole = 0;
            int IncomeAsWhole = 0;
            List<Expense> expenseList = new List<Expense>();
            List<Expense> expenseASWhole = new List<Expense>();
            List<Payment> paymentList = new List<Payment>();

            cement = db.Cements.Where(t => t.SOnumber == SOnumberPass).FirstOrDefault();
            if (db.Deliveries.Any(t => t.TransactionID == cement.TransactionID))
            {
                deliver = db.Deliveries.Where(t => t.TransactionID == cement.TransactionID).OrderByDescending(t => t.DateofCreation).FirstOrDefault();
            }
            
            
            if (deliver != null)
            {
                payment = db.Payments.Where(t => t.DeliveryID == deliver.DeliveryID).OrderByDescending(t => t.Delivery.DateofCreation).FirstOrDefault();
            }
            else
            {
                deliver = new Delivery();
                payment = new Payment();
            }
           

            foreach (Expense exp in db.Expenses)
            {
                if (payment != null)
                {
                    if (exp.PaymentID == payment.PaymentID)
                    {
                        expenseList.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalExpense += exp.Total;
                    }
                    //TOTAL Expense as a Whole
                    if (exp.Payment.TransactionID == cement.TransactionID)
                    {
                        expenseASWhole.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalAsWhole += exp.Total;
                    }


                }
                
                
               
            }

            //Checking if SO have Renewal Payment.
            if (cement.RenewalPayment != 0)
            {
                expenseList.Add(new Expense { PaymentID = 0, Amount = 1, Description = "Renewal SO", ExpenseID = 0, Total = cement.RenewalPayment });
                TotalExpense += cement.RenewalPayment;
            }

            



            //if (expenseList.Count <= 0)
            //{
            //    expenseList.Add(new Expense { PaymentID = 0, Amount = 0, Description = "", ExpenseID = 0, Total = 0 });
            //}

            if (payment == null)
            {
                //payment = new Payment {BankName="",Branch="",DateofCheque=DateTime.Now,DeliveryID=0,DRNumber="",Name="",PaymentID=0,TermsOfPayment=Terms.Cash };
                payment = new Payment();

            }
            else
            {
                summaryTransactionVM.DateOfCheque = payment.DateofCheque;
                summaryTransactionVM.DateProcessed = payment.Delivery.DateofCreation;
            }


            //Creation of Total as whole income
            foreach (Payment payWhole in db.Payments.Include("Delivery").Where(t => t.TransactionID == cement.TransactionID))
            {
                paymentList.Add(payWhole);
                IncomeAsWhole += (payWhole.PricePHP * payWhole.Delivery.QuantityToDeliver); 
            }




            //Creation of Summary
            summaryTransactionVM.BankName = payment.BankName;
            summaryTransactionVM.Branch = payment.Branch;
            summaryTransactionVM.ChequeNumber = cement.ChequeNumber;
            summaryTransactionVM.CurrentStock = cement.TotalQuantity - deliver.QuantityToDeliver;
            summaryTransactionVM.CustomerName = payment.Name;
            
            summaryTransactionVM.Income = payment.PricePHP * deliver.QuantityToDeliver;
            summaryTransactionVM.TotalIncome = (payment.PricePHP * deliver.QuantityToDeliver) - TotalExpense;
            summaryTransactionVM.QuantityToDeliver = deliver.QuantityToDeliver;
            summaryTransactionVM.Remarks = cement.Remarks;
            summaryTransactionVM.SONumber = cement.SOnumber;
            summaryTransactionVM.Stats = cement.Stats; // for Now everything is Pending;
            summaryTransactionVM.Expenses = expenseList;
            summaryTransactionVM.ExpensesAsWhole = expenseASWhole;
            

            //Checking How Many SO relations

            int relations = 0;
            List<Cement> countCement = new List<Cement>();
            countCement = db.Cements.Where(t => t.TransactionID == cement.TransactionID).ToList();
            relations = countCement.Count;

            summaryTransactionVM.RelatedCementList = countCement;

            ViewBag.relations = relations;
            ViewBag.CementID = cement.CementID;
            ViewBag.TotalExpense = TotalExpense;
            ViewBag.TotalAsWhole = TotalAsWhole;
            ViewBag.IncomeAsWhole = IncomeAsWhole;
            ViewBag.TransactionID = cement.TransactionID;
            return View(summaryTransactionVM);
        }

        [HttpPost]
        public ActionResult SummaryUpdate(Status Stats, string Remarks,int SONumber)
        {
            Cement cement = db.Cements.Where(t => t.SOnumber == SONumber).FirstOrDefault();



            switch (Stats)
            {
                case Status.Pending: cement.Stats = Status.Pending;
                    break;
                case Status.Cleared: cement.Stats = Status.Cleared;
                    break;
                case Status.Closed: cement.Stats = Status.Closed;
                    break;

            }






            db.Cements.Attach(cement);
            cement.Stats = Stats;
            cement.Remarks = Remarks;
            var entry = db.Entry(cement);
            entry.Property(e => e.Remarks).IsModified = true;
            entry.Property(e => e.Stats).IsModified = true;


            
            db.SaveChanges();

            return RedirectToAction("Index", "SummaryTransaction", new { @SOnumberPass = SONumber });
               
        }

        public ActionResult ForStorage(int SOnumberPass, int Operation)
        {

            SummaryTransactionViewModel summaryTransactionVM = new SummaryTransactionViewModel();
            if (Operation == 0)
            {
                ViewBag.Operation = "For Storage";
            }
            else
            {
                ViewBag.Operation = "For Completion";
            }

            Cement cement = new Cement();
            Delivery deliver = new Delivery();
            Payment payment = new Payment();
            int TotalExpense = 0;
            int TotalAsWhole = 0;
            int IncomeAsWhole = 0;
            List<Expense> expenseList = new List<Expense>();
            List<Expense> expenseASWhole = new List<Expense>();
            List<Payment> paymentList = new List<Payment>();

            cement = db.Cements.Where(t => t.SOnumber == SOnumberPass).FirstOrDefault();
            if (db.Deliveries.Any(t => t.TransactionID == cement.TransactionID))
            {
                deliver = db.Deliveries.Where(t => t.TransactionID == cement.TransactionID).OrderByDescending(t => t.DateofCreation).FirstOrDefault();
            }


            if (deliver != null)
            {
                payment = db.Payments.Where(t => t.DeliveryID == deliver.DeliveryID).OrderByDescending(t => t.Delivery.DateofCreation).FirstOrDefault();
            }
            else
            {
                deliver = new Delivery();
                payment = new Payment();
            }


            foreach (Expense exp in db.Expenses)
            {
                if (payment != null)
                {
                    if (exp.PaymentID == payment.PaymentID)
                    {
                        expenseList.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalExpense += exp.Total;
                    }
                    //TOTAL Expense as a Whole
                    if (exp.Payment.TransactionID == cement.TransactionID)
                    {
                        expenseASWhole.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalAsWhole += exp.Total;
                    }


                }



            }

            //Checking if SO have Renewal Payment.
            if (cement.RenewalPayment != 0)
            {
                expenseList.Add(new Expense { PaymentID = 0, Amount = 1, Description = "Renewal SO", ExpenseID = 0, Total = cement.RenewalPayment });
                TotalExpense += cement.RenewalPayment;
            }





            //if (expenseList.Count <= 0)
            //{
            //    expenseList.Add(new Expense { PaymentID = 0, Amount = 0, Description = "", ExpenseID = 0, Total = 0 });
            //}

            if (payment == null)
            {
                //payment = new Payment {BankName="",Branch="",DateofCheque=DateTime.Now,DeliveryID=0,DRNumber="",Name="",PaymentID=0,TermsOfPayment=Terms.Cash };
                payment = new Payment();

            }
            else
            {
                summaryTransactionVM.DateOfCheque = payment.DateofCheque;
                summaryTransactionVM.DateProcessed = payment.Delivery.DateofCreation;
            }


            //Creation of Total as whole income
            foreach (Payment payWhole in db.Payments.Include("Delivery").Where(t => t.TransactionID == cement.TransactionID))
            {
                paymentList.Add(payWhole);
                IncomeAsWhole += (payWhole.PricePHP * payWhole.Delivery.QuantityToDeliver);
            }




            //Creation of Summary
            summaryTransactionVM.BankName = payment.BankName;
            summaryTransactionVM.Branch = payment.Branch;
            summaryTransactionVM.ChequeNumber = cement.ChequeNumber;
            summaryTransactionVM.CurrentStock = cement.TotalQuantity - deliver.QuantityToDeliver;
            summaryTransactionVM.CustomerName = payment.Name;

            summaryTransactionVM.Income = payment.PricePHP * deliver.QuantityToDeliver;
            summaryTransactionVM.TotalIncome = (payment.PricePHP * deliver.QuantityToDeliver) - TotalExpense;
            summaryTransactionVM.QuantityToDeliver = deliver.QuantityToDeliver;
            summaryTransactionVM.Remarks = cement.Remarks;
            summaryTransactionVM.SONumber = cement.SOnumber;
            summaryTransactionVM.Stats = cement.Stats; // for Now everything is Pending;
            summaryTransactionVM.Expenses = expenseList;
            summaryTransactionVM.ExpensesAsWhole = expenseASWhole;


            //Checking How Many SO relations

            int relations = 0;
            List<Cement> countCement = new List<Cement>();
            countCement = db.Cements.Where(t => t.TransactionID == cement.TransactionID).ToList();
            relations = countCement.Count;

            summaryTransactionVM.RelatedCementList = countCement;

            ViewBag.relations = relations;
            ViewBag.CementID = cement.CementID;
            ViewBag.TotalExpense = TotalExpense;
            ViewBag.TotalAsWhole = TotalAsWhole;
            ViewBag.IncomeAsWhole = IncomeAsWhole;
            ViewBag.TransactionID = cement.TransactionID;
            return View(summaryTransactionVM);

            
        }

        [HttpPost]
        public ActionResult ForStorage(string Remarks, int SOnumber)
        {
            Cement cement = new Cement();

            cement = db.Cements.Where(t => t.SOnumber == SOnumber).FirstOrDefault();

            cement.Remarks = Remarks;
            cement.Stats = Status.Closed;
            db.Cements.Attach(cement);
            var entry = db.Entry(cement);
            entry.Property(e => e.Remarks).IsModified = true;

            var entry2 = db.Entry(cement);
            entry2.Property(e => e.Stats).IsModified = true;
            // other changed properties
            db.SaveChanges();







            SummaryTransactionViewModel summaryTransactionVM = new SummaryTransactionViewModel();

            cement = new Cement();
            Delivery deliver = new Delivery();
            Payment payment = new Payment();
            int TotalExpense = 0;
            int TotalAsWhole = 0;
            int IncomeAsWhole = 0;
            List<Expense> expenseList = new List<Expense>();
            List<Expense> expenseASWhole = new List<Expense>();
            List<Payment> paymentList = new List<Payment>();

            cement = db.Cements.Where(t => t.SOnumber == SOnumber).FirstOrDefault();
            if (db.Deliveries.Any(t => t.TransactionID == cement.TransactionID))
            {
                deliver = db.Deliveries.Where(t => t.TransactionID == cement.TransactionID).OrderByDescending(t => t.DateofCreation).FirstOrDefault();
            }


            if (deliver != null)
            {
                payment = db.Payments.Where(t => t.DeliveryID == deliver.DeliveryID).OrderByDescending(t => t.Delivery.DateofCreation).FirstOrDefault();
            }
            else
            {
                deliver = new Delivery();
                payment = new Payment();
            }


            foreach (Expense exp in db.Expenses)
            {
                if (payment != null)
                {
                    if (exp.PaymentID == payment.PaymentID)
                    {
                        expenseList.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalExpense += exp.Total;
                    }
                    //TOTAL Expense as a Whole
                    if (exp.Payment.TransactionID == cement.TransactionID)
                    {
                        expenseASWhole.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalAsWhole += exp.Total;
                    }


                }



            }

            //Checking if SO have Renewal Payment.
            if (cement.RenewalPayment != 0)
            {
                expenseList.Add(new Expense { PaymentID = 0, Amount = 1, Description = "Renewal SO", ExpenseID = 0, Total = cement.RenewalPayment });
                TotalExpense += cement.RenewalPayment;
            }





            //if (expenseList.Count <= 0)
            //{
            //    expenseList.Add(new Expense { PaymentID = 0, Amount = 0, Description = "", ExpenseID = 0, Total = 0 });
            //}

            if (payment == null)
            {
                //payment = new Payment {BankName="",Branch="",DateofCheque=DateTime.Now,DeliveryID=0,DRNumber="",Name="",PaymentID=0,TermsOfPayment=Terms.Cash };
                payment = new Payment();

            }
            else
            {
                summaryTransactionVM.DateOfCheque = payment.DateofCheque;
                summaryTransactionVM.DateProcessed = payment.Delivery.DateofCreation;
            }


            //Creation of Total as whole income
            foreach (Payment payWhole in db.Payments.Include("Delivery").Where(t => t.TransactionID == cement.TransactionID))
            {
                paymentList.Add(payWhole);
                IncomeAsWhole += (payWhole.PricePHP * payWhole.Delivery.QuantityToDeliver);
            }




            //Creation of Summary
            summaryTransactionVM.BankName = payment.BankName;
            summaryTransactionVM.Branch = payment.Branch;
            summaryTransactionVM.ChequeNumber = cement.ChequeNumber;
            summaryTransactionVM.CurrentStock = cement.TotalQuantity - deliver.QuantityToDeliver;
            summaryTransactionVM.CustomerName = payment.Name;

            summaryTransactionVM.Income = payment.PricePHP * deliver.QuantityToDeliver;
            summaryTransactionVM.TotalIncome = (payment.PricePHP * deliver.QuantityToDeliver) - TotalExpense;
            summaryTransactionVM.QuantityToDeliver = deliver.QuantityToDeliver;
            summaryTransactionVM.Remarks = cement.Remarks;
            summaryTransactionVM.SONumber = cement.SOnumber;
            summaryTransactionVM.Stats = cement.Stats; // for Now everything is Pending;
            summaryTransactionVM.Expenses = expenseList;
            summaryTransactionVM.ExpensesAsWhole = expenseASWhole;


            //Checking How Many SO relations

            int relations = 0;
            List<Cement> countCement = new List<Cement>();
            countCement = db.Cements.Where(t => t.TransactionID == cement.TransactionID).ToList();
            relations = countCement.Count;

            summaryTransactionVM.RelatedCementList = countCement;

            ViewBag.relations = relations;
            ViewBag.CementID = cement.CementID;
            ViewBag.TotalExpense = TotalExpense;
            ViewBag.TotalAsWhole = TotalAsWhole;
            ViewBag.IncomeAsWhole = IncomeAsWhole;
            ViewBag.TransactionID = cement.TransactionID;










            return View("Index",summaryTransactionVM);
        }

        public ActionResult PreviousSummary(int? SOnumberPass)
        {
            SummaryTransactionViewModel summaryTransactionVM = new SummaryTransactionViewModel();

            Cement cement = new Cement();
            Delivery deliver = new Delivery();
            Payment payment = new Payment();
            int TotalExpense = 0;
            int TotalAsWhole = 0;
            int IncomeAsWhole = 0;

            List<Expense> expenseList = new List<Expense>();
            List<Expense> expenseASWhole = new List<Expense>();
            List<Payment> paymentList = new List<Payment>();

            cement = db.Cements.Where(t => t.SOnumber == SOnumberPass).FirstOrDefault();
            if (db.Deliveries.Any(t => t.Cement.SOnumber == cement.SOnumber))
            {
                deliver = db.Deliveries.Where(t => t.Cement.SOnumber == cement.SOnumber).OrderByDescending(t => t.DateofCreation).FirstOrDefault();
            }


            if (deliver != null)
            {
                payment = db.Payments.Where(t => t.DeliveryID == deliver.DeliveryID).OrderByDescending(t => t.Delivery.DateofCreation).FirstOrDefault();
            }
            else
            {
                deliver = new Delivery();
                payment = new Payment();
            }


            foreach (Expense exp in db.Expenses)
            {
                if (payment != null)
                {
                    if (exp.PaymentID == payment.PaymentID)
                    {
                        expenseList.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalExpense += exp.Total;
                    }
                    //TOTAL Expense as a Whole
                    if (exp.Payment.TransactionID == cement.TransactionID)
                    {
                        expenseASWhole.Add(new Expense { PaymentID = exp.PaymentID, Amount = exp.Amount, Description = exp.Description, ExpenseID = exp.ExpenseID, Total = exp.Total });
                        TotalAsWhole += exp.Total;
                    }


                }



            }


            //Checking if SO have Renewal Payment.
            if (cement.RenewalPayment != 0)
            {
                expenseList.Add(new Expense { PaymentID = 0, Amount = 1, Description = "Renewal SO", ExpenseID = 0, Total = cement.RenewalPayment });
                TotalExpense += cement.RenewalPayment;
            }








            //if (expenseList.Count <= 0)
            //{
            //    expenseList.Add(new Expense { PaymentID = 0, Amount = 0, Description = "", ExpenseID = 0, Total = 0 });
            //}

            if (payment == null)
            {
                //payment = new Payment {BankName="",Branch="",DateofCheque=DateTime.Now,DeliveryID=0,DRNumber="",Name="",PaymentID=0,TermsOfPayment=Terms.Cash };
                payment = new Payment();

            }
            else
            {
                summaryTransactionVM.DateOfCheque = payment.DateofCheque;
                summaryTransactionVM.DateProcessed = payment.Delivery.DateofCreation;
            }


            //Creation of Total as whole income
            foreach (Payment payWhole in db.Payments.Include("Delivery").Where(t => t.TransactionID == cement.TransactionID))
            {
                paymentList.Add(payWhole);
                IncomeAsWhole += (payWhole.PricePHP * payWhole.Delivery.QuantityToDeliver);
            }

            //Creation of Summary
            summaryTransactionVM.BankName = payment.BankName;
            summaryTransactionVM.Branch = payment.Branch;
            summaryTransactionVM.ChequeNumber = cement.ChequeNumber;
            summaryTransactionVM.CurrentStock = cement.TotalQuantity - deliver.QuantityToDeliver;
            summaryTransactionVM.CustomerName = payment.Name;

            summaryTransactionVM.Income = payment.PricePHP * deliver.QuantityToDeliver;
            summaryTransactionVM.TotalIncome = (payment.PricePHP * deliver.QuantityToDeliver) - TotalExpense;
            summaryTransactionVM.QuantityToDeliver = deliver.QuantityToDeliver;
            summaryTransactionVM.Remarks = cement.Remarks;
            summaryTransactionVM.SONumber = cement.SOnumber;
            summaryTransactionVM.Stats = cement.Stats; // for Now everything is Pending;
            summaryTransactionVM.Expenses = expenseList;
            summaryTransactionVM.ExpensesAsWhole = expenseASWhole;


            //Checking How Many SO relations

            int relations = 0;
            List<Cement> countCement = new List<Cement>();
            countCement = db.Cements.Where(t => t.TransactionID == cement.TransactionID).ToList();
            relations = countCement.Count;

            summaryTransactionVM.RelatedCementList = countCement;

            ViewBag.relations = relations;
            ViewBag.CementID = cement.CementID;
            ViewBag.TotalExpense = TotalExpense;
            ViewBag.TotalAsWhole = TotalAsWhole;
            ViewBag.IncomeAsWhole = IncomeAsWhole;
            ViewBag.TransactionID = cement.TransactionID;
            return View("Index",summaryTransactionVM);
        }


    }


    
}