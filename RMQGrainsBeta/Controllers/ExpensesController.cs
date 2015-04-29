using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RMQGrainsBeta.Models;

namespace RMQGrainsBeta.Controllers
{
    public class ExpensesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Expenses
        public ActionResult Index()
        {
            var expenses = db.Expenses.Include(e => e.Payment);
            return View(expenses.ToList());
        }

        public ActionResult DetailedExpenses(int SOnumberPass)
        {
            int TotalExpenses = 0;
            ExpenseDetailedViewModel ExpenseVM = new ExpenseDetailedViewModel();
            List<ExpenseDetailedViewModel> ExpenseVMList = new List<ExpenseDetailedViewModel>();

            Cement cement = db.Cements.Where(c => c.SOnumber == SOnumberPass).FirstOrDefault();
            Delivery delivery = db.Deliveries.Where(t=>t.TransactionID == cement.TransactionID).FirstOrDefault();
            Payment payment = db.Payments.Include(t=>t.Expenses).Where(c=>c.TransactionID == cement.TransactionID).FirstOrDefault();

            if (cement.RenewalPayment != 0)
            {
                ExpenseVM = new ExpenseDetailedViewModel();
                ExpenseVM.Description = "Renewal Expense";
                ExpenseVM.Amount = 1;
                ExpenseVM.QuantityTobeDelivered = 0;
                ExpenseVM.Destination = "";
                ExpenseVM.DateOfExpenseCreated = cement.CurrentDate;
                ExpenseVM.Total = cement.RenewalPayment;
                ExpenseVMList.Add(ExpenseVM);
                TotalExpenses += cement.RenewalPayment;
            }
            foreach (Payment pays in db.Payments.Where(t => t.TransactionID == cement.TransactionID))
            {
                foreach (Expense exp in db.Expenses.Where(t => t.PaymentID == pays.PaymentID))
                {
                    ExpenseVM = new ExpenseDetailedViewModel();
                    ExpenseVM.Description = exp.Description;
                    ExpenseVM.Amount = exp.Amount;
                    ExpenseVM.QuantityTobeDelivered = delivery.QuantityToDeliver;
                    ExpenseVM.Destination = payment.Name;
                    ExpenseVM.DateOfExpenseCreated = cement.CurrentDate;
                    ExpenseVM.Total = exp.Total;
                    ExpenseVMList.Add(ExpenseVM);
                    TotalExpenses += exp.Total;
                }
            }

            //foreach (Expense exp in db.Expenses.Where(t => t.PaymentID == payment.PaymentID))
            //{
            //    ExpenseVM.Description = exp.Description;
            //    ExpenseVM.Amount = exp.Amount;
            //    ExpenseVM.QuantityTobeDelivered = delivery.QuantityToDeliver;
            //    ExpenseVM.Destination = payment.Name;
            //    ExpenseVM.DateOfExpenseCreated = cement.CurrentDate;
            //    ExpenseVM.Total = exp.Total;
            //    ExpenseVMList.Add(ExpenseVM);
            //    TotalExpenses += exp.Total;
            //}

            ViewBag.TotalExpenses = TotalExpenses;

            return View(ExpenseVMList);
        }

        public ActionResult ExpensesIndex(DateTime? DateFrom, DateTime? DateTo)
        {
            int TotalExpense = 0;
            int AllExpense = 0;
            ExpenseIndexViewModel expIndex = new ExpenseIndexViewModel();
            List<ExpenseIndexViewModel> expList = new List<ExpenseIndexViewModel>();
            Payment payment = new Payment();
            Cement cement = new Cement();

            if (DateFrom != null && DateTo != null)
            {
                foreach (Expense exp in db.Expenses.Include(t => t.Payment).Where(t => t.Payment.DateofCheque > DateFrom && t.Payment.DateofCheque < DateTo))
                {
                    expIndex = new ExpenseIndexViewModel();
                    cement = db.Cements.Where(t => t.TransactionID == exp.Payment.TransactionID).FirstOrDefault();
                    expIndex.SONumber = cement.SOnumber;
                    expIndex.DateProcessed = cement.CurrentDate;
                    expIndex.Stats = cement.Stats;
                    if (cement.RenewalPayment != 0)
                    {
                        expIndex.TotalExpense += cement.RenewalPayment;
                    }
                    foreach (Expense addExpense in db.Expenses.Where(t => t.PaymentID == exp.PaymentID))
                    {
                        TotalExpense += addExpense.Total;
                        expIndex.SONumber = db.Cements.Where(t => t.TransactionID == addExpense.Payment.TransactionID).LastOrDefault().SOnumber;
                        
                    }
                    expIndex.TotalExpense = TotalExpense;
                    
                    TotalExpense = 0;
                    expList.Add(expIndex);
                    AllExpense += exp.Total;
                }

             
               
            }
            else
            {
                foreach (Expense exp in db.Expenses.Include(t => t.Payment))
                {
                    expIndex = new ExpenseIndexViewModel();
                    cement = db.Cements.Where(t => t.TransactionID == exp.Payment.TransactionID).OrderByDescending(t => t.DateCreation).First();
                    expIndex.SONumber = cement.SOnumber;
                    expIndex.DateProcessed = cement.CurrentDate;
                    expIndex.Stats = cement.Stats;
                    if (cement.RenewalPayment != 0)
                    {
                        expIndex.TotalExpense += cement.RenewalPayment;
                    }
                    foreach (Expense addExpense in db.Expenses.Where(t => t.PaymentID == exp.PaymentID))
                    {
                        TotalExpense += addExpense.Total;
                        
                        
                    }
                    expIndex.TotalExpense = TotalExpense;
                    
                    TotalExpense = 0;
                    
                    expList.Add(expIndex);
                    AllExpense += exp.Total;
                }
                
                
            }

            ViewBag.AllExpense = AllExpense;
            return View(expList.ToList().GroupBy(l => l.SONumber).Select(group => group.Last()));

        }

        // GET: Expenses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // GET: Expenses/Create
        public ActionResult Create()
        {

            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "DRNumber");
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpenseID,Expenses,Amount,PaymentID")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Expenses.Add(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "Name", expense.PaymentID);
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "Name", expense.PaymentID);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpenseID,Expenses,Amount,PaymentID")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PaymentID = new SelectList(db.Payments, "PaymentID", "Name", expense.PaymentID);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Expense expense = db.Expenses.Find(id);
            db.Expenses.Remove(expense);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
