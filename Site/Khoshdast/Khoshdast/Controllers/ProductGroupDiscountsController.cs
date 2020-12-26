using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Khoshdast.Controllers
{
    public class ProductGroupDiscountsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index(Guid id)
        {
            var productGroupDiscounts = db.ProductGroupDiscounts.Include(p => p.ProductGroup)
                .Where(p => p.ProductGroupId == id && p.IsDeleted == false).OrderByDescending(p => p.CreationDate);

            return View(productGroupDiscounts.ToList());
        }

        public ActionResult Create(Guid id)
        {
            ViewBag.ProductGroupId = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductGroupDiscount productGroupDiscount, Guid id)
        {
            if (ModelState.IsValid)
            {
                productGroupDiscount.IsDeleted = false;
                productGroupDiscount.CreationDate = DateTime.Now;
                productGroupDiscount.ProductGroupId = id;
                productGroupDiscount.Id = Guid.NewGuid();
                db.ProductGroupDiscounts.Add(productGroupDiscount);

                if (productGroupDiscount.IsActive)
                    EnableDiscountForProducts(id, productGroupDiscount.Amount, productGroupDiscount.IsPercentage,
                        productGroupDiscount.Id);

                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            ViewBag.ProductGroupId = id;
            return View(productGroupDiscount);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroupDiscount productGroupDiscount = db.ProductGroupDiscounts.Find(id);
            if (productGroupDiscount == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductGroupId = productGroupDiscount.ProductGroupId;
            return View(productGroupDiscount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductGroupDiscount productGroupDiscount)
        {
            if (ModelState.IsValid)
            {
                productGroupDiscount.IsDeleted = false;
                productGroupDiscount.LastModifiedDate = DateTime.Now;

                if (productGroupDiscount.IsActive)
                    EnableDiscountForProducts(productGroupDiscount.ProductGroupId, productGroupDiscount.Amount, productGroupDiscount.IsPercentage,
                        productGroupDiscount.Id);
                else
                    DisableDiscountForProducts(productGroupDiscount.ProductGroupId);

                db.Entry(productGroupDiscount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = productGroupDiscount.ProductGroupId });
            }
            ViewBag.ProductGroupId = productGroupDiscount.ProductGroupId;
            return View(productGroupDiscount);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroupDiscount productGroupDiscount = db.ProductGroupDiscounts.Find(id);
            if (productGroupDiscount == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductGroupId = productGroupDiscount.ProductGroupId;

            return View(productGroupDiscount);
        }

        // POST: ProductGroupDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ProductGroupDiscount productGroupDiscount = db.ProductGroupDiscounts.Find(id);
            productGroupDiscount.IsDeleted = true;
            productGroupDiscount.DeletionDate = DateTime.Now;

            DisableDiscountForProducts(productGroupDiscount.ProductGroupId);

            db.SaveChanges();
            return RedirectToAction("Index", new { id = productGroupDiscount.ProductGroupId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void EnableDiscountForProducts(Guid id, decimal amount, bool isPercent, Guid productGroupDiscountId)
        {
            ProductGroup oProductGroup = db.ProductGroups.Find(id);

            if (oProductGroup != null)
            {
                List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                    .Where(c => c.ProductGroupId == oProductGroup.Id && c.IsDeleted == false).ToList();

                List<Product> products = new List<Product>();

                foreach (ProductGroupRelProduct productGroupRelProduct in productGroupRelProducts)
                {
                    if (!products.Contains(productGroupRelProduct.Product))
                        products.Add(productGroupRelProduct.Product);
                }

                foreach (Product product in products)
                {
                    decimal discountAmount = product.Amount - amount;

                    if (isPercent)
                    {
                        discountAmount = product.Amount - (product.Amount * amount / 100);
                    }

                    product.DiscountAmount = discountAmount;
                    product.IsInPromotion = true;
                    product.LastModifiedDate = DateTime.Now;
                    product.ProductGroupDiscountId = productGroupDiscountId;
                }
            }
        }
        public void DisableDiscountForProducts(Guid id)
        {
            ProductGroup oProductGroup = db.ProductGroups.Find(id);

            if (oProductGroup != null)
            {
                List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                    .Where(c => c.ProductGroupId == oProductGroup.Id && c.IsDeleted == false).ToList();

                List<Product> products = new List<Product>();

                foreach (ProductGroupRelProduct productGroupRelProduct in productGroupRelProducts)
                {
                    if (!products.Contains(productGroupRelProduct.Product))
                        products.Add(productGroupRelProduct.Product);
                }

                foreach (Product product in products)
                {
                    product.DiscountAmount = 0;
                    product.IsInPromotion = false;
                    product.LastModifiedDate = DateTime.Now;
                    product.ProductGroupDiscountId = null;
                }
            }
        }
    }
}
