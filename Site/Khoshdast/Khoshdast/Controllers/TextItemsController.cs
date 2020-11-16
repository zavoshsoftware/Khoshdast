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
    public class TextItemsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: TextItems
        public ActionResult Index(Guid id)
        {
            var textItems = db.TextItems.Include(t => t.TextItemType).Where(t=>t.TextItemTypeId==id&& t.IsDeleted==false).OrderByDescending(t=>t.CreationDate);
            return View(textItems.ToList());
        }

        // GET: TextItems/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItem textItem = db.TextItems.Find(id);
            if (textItem == null)
            {
                return HttpNotFound();
            }
            return View(textItem);
        }

        // GET: TextItems/Create
        public ActionResult Create()
        {
            ViewBag.TextItemTypeId = new SelectList(db.TextItemTypes, "Id", "Title");
            return View();
        }

        // POST: TextItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TextItem textItem, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/text/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    textItem.ImageUrl = newFilenameUrl;
                }


                #endregion
                textItem.IsDeleted=false;
				textItem.CreationDate= DateTime.Now; 
					
                textItem.Id = Guid.NewGuid();
                db.TextItems.Add(textItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TextItemTypeId = new SelectList(db.TextItemTypes, "Id", "Title", textItem.TextItemTypeId);
            return View(textItem);
        }

        // GET: TextItems/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItem textItem = db.TextItems.Find(id);
            if (textItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.TextItemTypeId = textItem.TextItemTypeId;
            return View(textItem);
        }

        // POST: TextItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TextItem textItem, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/text/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    textItem.ImageUrl = newFilenameUrl;
                }


                #endregion
                textItem.IsDeleted=false;
					textItem.LastModifiedDate=DateTime.Now;
                db.Entry(textItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new{id=textItem.TextItemTypeId});
            }
            ViewBag.TextItemTypeId = textItem.TextItemTypeId;
            return View(textItem);
        }

        // GET: TextItems/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItem textItem = db.TextItems.Find(id);
            if (textItem == null)
            {
                return HttpNotFound();
            }
            return View(textItem);
        }

        // POST: TextItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TextItem textItem = db.TextItems.Find(id);
			textItem.IsDeleted=true;
			textItem.DeletionDate=DateTime.Now;
 
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
