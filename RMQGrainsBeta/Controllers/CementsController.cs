using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RMQGrainsBeta.Models;
using System.IO;


namespace RMQGrainsBeta.Controllers
{
    public class CementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cements
        public ActionResult Index(DateTime? DateFrom, DateTime? DateTo)
        {
            
            //var all = db.Cements.Include(t => t.Deliveries);
            List<CementIndexViewModels> cmVM = new List<CementIndexViewModels>();
            Payment payList = new Payment();
            int TotalDelivery = 0;
            int TotalIncome = 0;
            int TotalExpenses = 0;
            int TotalGross = 0;

            foreach (Cement cement in db.Cements)
            {
                List<Delivery> delList = new List<Delivery>();
                
                foreach (Delivery delTemp in db.Deliveries.Where(t => t.TransactionID == cement.TransactionID))
                {
                    delList.Add(delTemp);
                }
                foreach (int qtyToDeliver in delList.Select(t => t.QuantityToDeliver))
                {
                    TotalDelivery += qtyToDeliver;
                }


                //Checking Payments if Any
                foreach (Delivery payDel in delList)
                {
                    if (db.Payments.Any(t => t.DeliveryID == payDel.DeliveryID))
                    {
                        payList = db.Payments.Where(t => t.DeliveryID == payDel.DeliveryID).FirstOrDefault();
                        if (payList != null)
                        {
                            foreach (Expense expDel in db.Expenses.Where(t => t.PaymentID == payList.PaymentID))
                            {
                                TotalExpenses += expDel.Total;

                            }
                        }
                        //Calculate Total Expense.
                       
                        //Calculate Total Income.
                        foreach (int TotsPay in db.Payments.Where(t => t.DeliveryID == payDel.DeliveryID).Select(c => c.PricePHP))
                        {
                            TotalIncome += (TotalDelivery * TotsPay)-TotalExpenses;
                            TotalGross += TotalDelivery * TotsPay;
                        }

                       
                    }
                }

                // Adding Renewal to Expense if Any
                TotalIncome -= cement.RenewalPayment;
                

                CementIndexViewModels tempData = new CementIndexViewModels();
                tempData.SONumber = cement.SOnumber;
                tempData.Renewed = cement.Renewed;
                tempData.OriginalStock = cement.TotalQuantity;
                tempData.QuantitySold = TotalDelivery;
                tempData.CurrentStock = cement.TotalQuantity - TotalDelivery;
                tempData.DateProcessed = cement.CurrentDate;
                tempData.Stats = cement.Stats;
                tempData.TotalIncome = TotalIncome;
                tempData.TransactionID = (int)cement.TransactionID;
                tempData.GrossIncome = TotalGross;
                tempData.Remarks = cement.Remarks;

                cmVM.Add(tempData);
                TotalDelivery = 0;
                TotalExpenses = 0;
                TotalIncome = 0;
                TotalGross = 0;
            }
            if (DateFrom != null && DateTo != null)
            {
                cmVM = cmVM.Where(t => t.DateProcessed >= DateFrom && t.DateProcessed <= DateTo).ToList();
            }
            return View(cmVM.ToList().GroupBy(l => l.TransactionID).Select(group=>group.Last()));
        }

        public ActionResult RenewSO(int CementID,int CurrentStock)
        {
            Cement newCement = new Cement();

            newCement = db.Cements.Find(CementID);
            newCement.TotalQuantity = CurrentStock;
            Transaction transaction = db.Transactions.Find(newCement.TransactionID);

            if (transaction == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            


            if (newCement == null)
            {
                return HttpNotFound();
            }



            ViewBag.TransactionID = newCement.TransactionID;

            return View(newCement);
        }
        [HttpPost]
        public ActionResult RenewSO(Cement cement, HttpPostedFileBase Upload)
        {
            cement.DateCreation = DateTime.Now;

            if (Upload.ContentLength > 0)
            {
                var FileName = Upload.FileName;
                var path = Path.Combine(Server.MapPath("~/Content/Images"), FileName);
                Upload.SaveAs(path);
                cement.ImageLocation = ("/Content/Images/" + FileName);
            }

            if (ModelState.IsValid)
            {

                db.Cements.Add(cement);
                db.SaveChanges();
            }

            
            return RedirectToAction("Index", "SummaryTransaction", new { SOnumberPass = cement.SOnumber });
        }
        // GET: Cements/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cement cement = db.Cements.Find(id);


            if (cement == null)
            {
                return HttpNotFound();
            }
            return View(cement);
        }

        // GET: Cements/Create
        public ActionResult Create()
        {
            CementViewModels cement = new CementViewModels();


            return View(cement);
        }

        // POST: Cements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CementViewModels cement, HttpPostedFileBase Upload)
        {
            Transaction Trans = new Transaction();
            cement.Cements.DateCreation = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (Upload.ContentLength > 0)
                {
                    var FileName = Upload.FileName;
                    var PathLocation = Path.Combine(Server.MapPath("~/Content/Images/"), FileName);
                    Upload.SaveAs(PathLocation);
                    var Loc = "/Content/Images/" + FileName;
                    cement.Cements.ImageLocation = Loc;

                }
                
                
                db.Cements.Add(cement.Cements);


                db.SaveChanges();
                // Update Cement.transaction ID
                Trans.CementID = cement.Cements.CementID;
                db.Transactions.Add(Trans);

                cement.Cements.TransactionID = Trans.TransactionID;
                db.Cements.Attach(cement.Cements);
                var entry4 = db.Entry(cement.Cements);
                entry4.Property(e => e.TransactionID).IsModified = true;
                db.SaveChanges();

                if (cement.Deliveries.QuantityToDeliver != 0)
                {
                    cement.Deliveries.CementID = cement.Cements.CementID;
                    cement.Deliveries.DateofCreation = DateTime.Now;
                    db.Deliveries.Add(cement.Deliveries);
                    db.SaveChanges();



                    
                    Trans.DeliveryID = cement.Deliveries.DeliveryID;
                    
                    db.Transactions.Attach(Trans);
                    var entry3 = db.Entry(Trans);
                    entry3.Property(e => e.DeliveryID).IsModified = true;
                    
                    

                    cement.Cements.TransactionID = Trans.TransactionID;
                    db.Cements.Attach(cement.Cements);
                    var entry = db.Entry(cement.Cements);
                    entry.Property(e => e.TransactionID).IsModified = true;


                    cement.Deliveries.TransactionID = Trans.TransactionID;
                    db.Deliveries.Attach(cement.Deliveries);
                    var entry2 = db.Entry(cement.Deliveries);
                    entry2.Property(e => e.TransactionID).IsModified = true;
                    // other changed properties
                    db.SaveChanges();
                }
                db.SaveChanges();

                if (cement.Deliveries.DeliveryID != 0)
                {
                    return RedirectToAction("Create", "Payments", new { DeliveryID = cement.Deliveries.DeliveryID, TransactionID = Trans.TransactionID });
                }

            }

            return Redirect("Index");
        }

        // GET: Cements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cement cement = db.Cements.Find(id);
            if (cement == null)
            {
                return HttpNotFound();
            }
            return View(cement);
        }

        // POST: Cements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CementID,SOnumber,CompanyName,BuyingTo,CurrentDate,CementType,TotalQuantity,PaymentMethod,ChequeNumber,Remarks,ImageLocation")] Cement cement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cement);
        }

        // GET: Cements/Delete/5
        public ActionResult Delete(int? SOnumber)
        {
            if (SOnumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cement cement = db.Cements.Where(t => t.SOnumber == SOnumber).OrderByDescending(c => c.DateCreation).FirstOrDefault();
            if (cement == null)
            {
                return HttpNotFound();
            }
            return View(cement);
        }

        // POST: Cements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? SOnumber)
        {
            Cement cement = db.Cements.Where(t => t.SOnumber == SOnumber).OrderByDescending(c => c.DateCreation).FirstOrDefault();
            db.Cements.Remove(cement);
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
