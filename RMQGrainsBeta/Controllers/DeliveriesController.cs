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
    public class DeliveriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Deliveries
        public ActionResult Index()
        {
            var deliveries = db.Deliveries.Include(d => d.Cement);
            return View(deliveries.ToList());
        }

        // GET: Deliveries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        // GET: Deliveries/Create
        public ActionResult Create()
        {
            ViewBag.CementID = new SelectList(db.Cements, "CementID", "SOnumber");
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Deliveries.Add(delivery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CementID = new SelectList(db.Cements, "CementID", "CompanyName", delivery.CementID);
            return View(delivery);
        }

        public ActionResult AddDelivery(int CementID)
        {
            DeliveryPaymentViewModel dpVM = new DeliveryPaymentViewModel();
            
            ViewBag.CementID = CementID;
            return View(dpVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddDelivery(Delivery delivery,int CementID)
        //{
        //    Transaction transaction = new Transaction();
        //    db.Deliveries.Add(delivery);
        //    db.SaveChanges();
        //    transaction.DeliveryID = delivery.DeliveryID;
            
        //    if (!db.Transactions.Any(t => t.CementID == CementID))
        //    {
        //        transaction.CementID = CementID;
        //        transaction.DeliveryID = delivery.DeliveryID;
                
        //        db.Transactions.Add(transaction);
        //        db.SaveChanges();

        //        delivery.TransactionID = transaction.TransactionID;
        //        db.Deliveries.Attach(delivery);
        //        var entry = db.Entry(delivery);
        //        entry.Property(e => e.TransactionID).IsModified = true;
        //        // Transaction Creation + Delivery ID added completed

        //    }
        //    else
        //    {
        //        transaction = db.Transactions.Where(t => t.CementID == CementID).First();
        //        delivery.TransactionID = transaction.TransactionID;
        //        db.Deliveries.Attach(delivery);
        //        var entry = db.Entry(delivery);
        //        entry.Property(e => e.TransactionID).IsModified = true;
        //        // Updated Transaction added delivery entity with The transaction ID

                
        //        // other changed properties
                
        //    }
            
        //    db.SaveChanges();

        //    return RedirectToAction("Index", "Cements", null);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDelivery(DeliveryPaymentViewModel delivery, int CementID, int TransactionID)
        {
            
            // Create Individiual Instance.
            Transaction transaction = new Transaction();
            Delivery Deliveries = new Delivery();
            Payment Payments = new Payment();
            Expense Expenses = new Expense();
            Cement Cements = new Cement();
            List<Expense> expenseList = new List<Expense>();

            // Delivery Instantiate
            Deliveries.CementID = CementID;
            Deliveries.DateOfArrival = delivery.DeliveryModel.DateOfArrival;
            Deliveries.DateofCreation = DateTime.Now;
            Deliveries.DeliveryTo = delivery.DeliveryModel.DeliveryTo;
            Deliveries.Destination = delivery.DeliveryModel.Destination;
            Deliveries.DriverName = delivery.DeliveryModel.DriverName;
            Deliveries.DRNumber = delivery.DeliveryModel.DRNumber;
            Deliveries.HaulerCompany = delivery.DeliveryModel.HaulerCompany;
            Deliveries.DestinationTo = delivery.DeliveryModel.DestinationTo;
            Deliveries.Hauling = delivery.DeliveryModel.Hauling;
            Deliveries.PlateNumber = delivery.DeliveryModel.PlateNumber;
            Deliveries.QuantityToDeliver = delivery.DeliveryModel.QuantityToDeliver;
            Deliveries.TransactionID = TransactionID;
            

            db.Deliveries.Add(Deliveries);
            db.SaveChanges();
            transaction.DeliveryID = delivery.DeliveryModel.DeliveryID;



            //if (!db.Transactions.Any(t => t.CementID == CementID))
            //{
            //    transaction.CementID = CementID;
            //    transaction.DeliveryID = delivery.DeliveryModel.DeliveryID;

            //    db.Transactions.Add(transaction);
            //    db.SaveChanges();

            //    Deliveries.TransactionID = transaction.TransactionID;
            //    db.Deliveries.Attach(Deliveries);
            //    var entry = db.Entry(Deliveries);
            //    entry.Property(e => e.TransactionID).IsModified = true;
            //    // Transaction Creation + Delivery ID added completed

            //    Cements = db.Cements.Find(Deliveries.CementID);
            //    Cements.TransactionID = transaction.TransactionID;
            //    db.Cements.Attach(Cements);
            //    var entry2 = db.Entry(Cements);
            //    entry2.Property(e => e.TransactionID).IsModified = true;

            //}
            //else
            //{
            //    transaction = db.Transactions.Where(t => t.CementID == CementID).First();
            //    Deliveries.TransactionID = transaction.TransactionID;
            //    db.Deliveries.Attach(Deliveries);
            //    var entry = db.Entry(Deliveries);
            //    entry.Property(e => e.TransactionID).IsModified = true;
            //    // Updated Transaction added delivery entity with The transaction ID


            //    // other changed properties

            //}

            
            //Checking if Payment DR Number is not null Save Payment
            if (!String.IsNullOrEmpty(delivery.PaymentModel.Name) || !String.IsNullOrEmpty(delivery.PaymentModel.BankName))
            {
                Payments.BankName = delivery.PaymentModel.BankName;
                Payments.Name = delivery.PaymentModel.Name;
                Payments.Branch = delivery.PaymentModel.Branch;
                Payments.DateofCheque = delivery.PaymentModel.DateofCheque;
                Payments.DRNumber = delivery.DeliveryModel.DRNumber;
                Payments.PricePHP = delivery.PaymentModel.PricePHP;
                Payments.ChequeNumber = delivery.PaymentModel.ChequeNumber;
                Payments.TermsOfPayment = delivery.PaymentModel.TermsOfPayment;
                Payments.TransactionID = TransactionID;
                Payments.DeliveryID = Deliveries.DeliveryID;
                db.Payments.Add(Payments);
                db.SaveChanges();
                //Save Transactions PaymentID;

                //transaction.PaymentID = Payments.PaymentID;
                //db.Transactions.Attach(transaction);
                //var entry = db.Entry(transaction);
                //entry.Property(e => e.PaymentID).IsModified = true;

                // Expese Checking.
                if (delivery.PaymentModel.Expenses != null)
                {
                    foreach (Expense exp in delivery.PaymentModel.Expenses)
                    {
                        exp.PaymentID = Payments.PaymentID;

                        db.Expenses.Add(exp);
                    }
                }
                

                

            }

            db.SaveChanges();
            Cements = db.Cements.Find(CementID);
            return RedirectToAction("Index", "SummaryTransaction", new { SOnumberPass= Cements.SOnumber });
        }
        // GET: Deliveries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            ViewBag.CementID = new SelectList(db.Cements, "CementID", "CompanyName", delivery.CementID);
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryID,Hauling,HaulerCompany,PlateNumber,DriverName,QuantityToDeliver,Destination,DateOfArrival,DeliveryTo,DRNumber,CementID")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CementID = new SelectList(db.Cements, "CementID", "CompanyName", delivery.CementID);
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delivery delivery = db.Deliveries.Find(id);
            db.Deliveries.Remove(delivery);
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
