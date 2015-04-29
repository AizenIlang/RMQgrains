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
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Delivery);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create(int? DeliveryID, int? TransactionID)
        {
            if (DeliveryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Delivery deliver = db.Deliveries.Find(DeliveryID);
            Transaction transaction = db.Transactions.Find(TransactionID);
            if (deliver == null && transaction == null)
            {
                return HttpNotFound();
            }
            

            
            PaymentViewModel PaymentView = new PaymentViewModel();
            //var insertMe = db.Expenses.ToList();
            //PaymentView.Expenses = insertMe;

            ViewBag.DeliveryID = deliver.DeliveryID;
            ViewBag.TransactionID = transaction.TransactionID;
            ViewBag.QuantityToDeliver = deliver.QuantityToDeliver;
            return View(PaymentView);

        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentViewModel payment)
        {

            Transaction trans = new Transaction();
            if (ModelState.IsValid)
            {
                //var DRNumberFind = db.Deliveries.Find(payment.DeliveryID).DRNumber;
                //payment.DRNumber = DRNumberFind;
                //db.Payments.Add(payment);
                Payment pay = new Payment {
                    BankName = payment.BankName, 
                    DateofCheque = payment.DateofCheque,
                    Branch = payment.Branch,
                    DeliveryID = payment.DeliveryID,
                    Name = payment.Name,
                    
                    TermsOfPayment = payment.TermsOfPayment,
                    PricePHP = payment.PricePHP
                };


                db.Payments.Add(pay);
               
                db.SaveChanges();

                // Update Transaction
                
                    trans = db.Transactions.Find(payment.TransactionID);
                    trans.PaymentID = pay.PaymentID;
                    db.Transactions.Attach(trans);
                    
                    var entry = db.Entry(trans);
                    entry.Property(e => e.PaymentID).IsModified = true;

                    pay.TransactionID = trans.TransactionID;
                    db.Payments.Attach(pay);

                    var entry2 = db.Entry(pay);
                    entry2.Property(e => e.TransactionID).IsModified = true;
                    db.SaveChanges();

                    if (payment.Expenses != null)
                    {
                        foreach (Expense exp in payment.Expenses)
                        {
                            exp.PaymentID = pay.PaymentID;
                            db.Expenses.Add(exp);
                        }
                    }
                

                db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "HaulerCompany", payment.DeliveryID);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "HaulerCompany", payment.DeliveryID);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentID,Name,BankName,Branch,TermsOfPayment,DateofCheque,DeliveryID,DRNumber")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeliveryID = new SelectList(db.Deliveries, "DeliveryID", "HaulerCompany", payment.DeliveryID);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
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
