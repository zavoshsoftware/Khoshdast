using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Khoshdast.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class ProductGroupsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ProductGroups
        public ActionResult Index(Guid? id)
        {
            List<ProductGroup> productGroups = new List<ProductGroup>();
            if (id == null)
            {
                productGroups = db.ProductGroups.Where(a => a.IsDeleted == false && a.ParentId == null).OrderBy(a => a.Order)
                    .ToList();
                ViewBag.Title = "مدیریت گروه محصولات";
                ViewBag.hidden = "html-hidden";
            }
            else
            {
                productGroups = db.ProductGroups.Where(a => a.IsDeleted == false && a.ParentId == id)
                    .OrderBy(a => a.Order)
                    .ToList();
                ViewBag.Title = $"مدیریت زیر گروه محصولات {db.ProductGroups.Find(id)?.Title}";
                ViewBag.parentId = id;
                ViewBag.classItem = "html-hidden";
            }
            return View(productGroups);
        }
         
        public ActionResult Create(Guid? id)
        {
            ViewBag.Title = id == null ? "افزودن گروه محصول" : $"افزودن زیر گروه به گروه {db.ProductGroups.Find(id)?.Title}";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductGroup productGroup, Guid? id, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/ProductGroup/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    productGroup.ImageUrl = newFilenameUrl;
                }


                #endregion
                productGroup.ParentId = id;
                productGroup.IsDeleted = false;
                productGroup.CreationDate = DateTime.Now;
                productGroup.Id = Guid.NewGuid();
                db.ProductGroups.Add(productGroup);
                db.SaveChanges();

                if (id == null)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index", new { id = id });

            }

            return View(productGroup);
        }

        public ActionResult Edit(Guid? id, Guid? parentId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductGroup productGroup = db.ProductGroups.Find(id);
            if (productGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentId = parentId;
            return View(productGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductGroup productGroup, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/ProductGroup/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    productGroup.ImageUrl = newFilenameUrl;
                }


                #endregion
                productGroup.IsDeleted = false;
                productGroup.LastModifiedDate = DateTime.Now;
                db.Entry(productGroup).State = EntityState.Modified;
                db.SaveChanges();
                if (productGroup.ParentId == null)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index", new { id = productGroup.ParentId });

            }
            ViewBag.parentId = productGroup.ParentId;
            return View(productGroup);
        }

        // GET: ProductGroups/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroup productGroup = db.ProductGroups.Find(id);
            if (productGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentId = productGroup.ParentId;
            return View(productGroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ProductGroup productGroup = db.ProductGroups.Find(id);
            productGroup.IsDeleted = true;
            productGroup.DeletionDate = DateTime.Now;

            if (db.ProductGroups.Any(current => current.ParentId == id && current.IsDeleted == false))
            {
                List<ProductGroup> oProductGroups = db.ProductGroups
                    .Where(current => current.ParentId == id && current.IsDeleted == false).ToList();
                foreach (ProductGroup item in oProductGroups)
                {
                    item.IsDeleted = true;
                    item.DeletionDate = DateTime.Now;
                }
            }
            db.SaveChanges();
            if (productGroup.ParentId == null)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Index", new { id = productGroup.ParentId });
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
