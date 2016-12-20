using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdventureWorks.Models;

namespace AdventureWorks.Controllers
{
    public class PurchaseOrderDetailsController : Controller
    {
        private AdventureWorksEntities db = new AdventureWorksEntities();

        // GET: PurchaseOrderDetails
        public ActionResult Index()
        {
            var purchaseOrderDetails = db.PurchaseOrderDetails.Include(p => p.Product).Include(p => p.PurchaseOrderHeader);
            return View(purchaseOrderDetails.ToList());
        }

        // GET: PurchaseOrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderDetail purchaseOrderDetail = db.PurchaseOrderDetails.Find(id);
            if (purchaseOrderDetail == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Create
        public ActionResult Create()
        {
            var purchaseOrderDetails = db.PurchaseOrderDetails.Include(p => p.Product)
                .Include(p => p.PurchaseOrderHeader)
                .Include(p => p.Product.ProductReviews);

            var reviews = db.ProductReviews.GroupBy(pr => pr.ProductID, pr => pr.Rating)
                                       .Select(g => new { ProductID = g.Key, Average = g.Average()  });
            var products = db.Products.Where(p => p.ProductSubcategoryID <4 );

            foreach (var review in reviews)
            {
                var product = products.FirstOrDefault(p => p.ProductID == review.ProductID);
                if(product != null)
                {
                    product.Name += ":" + product.ListPrice.ToString("C") + " (" + review.Average.ToString("F2") + ")";
                }
            }


            ViewBag.ProductID = new SelectList(products, "ProductID", "Name");
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrderHeaders, "PurchaseOrderID", "PurchaseOrderID");
            return View();
        }

        // POST: PurchaseOrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseOrderID,PurchaseOrderDetailID,DueDate,OrderQty,ProductID,UnitPrice,LineTotal,ReceivedQty,RejectedQty,StockedQty,ModifiedDate")] PurchaseOrderDetail purchaseOrderDetail)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseOrderDetails.Add(purchaseOrderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", purchaseOrderDetail.ProductID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrderHeaders, "PurchaseOrderID", "PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderDetail purchaseOrderDetail = db.PurchaseOrderDetails.Find(id);
            if (purchaseOrderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", purchaseOrderDetail.ProductID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrderHeaders, "PurchaseOrderID", "PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
            return View(purchaseOrderDetail);
        }

        // POST: PurchaseOrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseOrderID,PurchaseOrderDetailID,DueDate,OrderQty,ProductID,UnitPrice,LineTotal,ReceivedQty,RejectedQty,StockedQty,ModifiedDate")] PurchaseOrderDetail purchaseOrderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", purchaseOrderDetail.ProductID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrderHeaders, "PurchaseOrderID", "PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderDetail purchaseOrderDetail = db.PurchaseOrderDetails.Find(id);
            if (purchaseOrderDetail == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderDetail);
        }

        // POST: PurchaseOrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PurchaseOrderDetail purchaseOrderDetail = db.PurchaseOrderDetails.Find(id);
            db.PurchaseOrderDetails.Remove(purchaseOrderDetail);
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
