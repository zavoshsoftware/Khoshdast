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
    [Authorize(Roles = "Administrator")]
    public class TextItemTypesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: TextItemTypes
        public ActionResult Index()
        {
            return View(db.TextItemTypes.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: TextItemTypes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItemType textItemType = db.TextItemTypes.Find(id);
            if (textItemType == null)
            {
                return HttpNotFound();
            }
            return View(textItemType);
        }

        // GET: TextItemTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TextItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Name,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] TextItemType textItemType)
        {
            if (ModelState.IsValid)
            {
				textItemType.IsDeleted=false;
				textItemType.CreationDate= DateTime.Now; 
					
                textItemType.Id = Guid.NewGuid();
                db.TextItemTypes.Add(textItemType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(textItemType);
        }

        // GET: TextItemTypes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItemType textItemType = db.TextItemTypes.Find(id);
            if (textItemType == null)
            {
                return HttpNotFound();
            }
            return View(textItemType);
        }

        // POST: TextItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Name,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] TextItemType textItemType)
        {
            if (ModelState.IsValid)
            {
				textItemType.IsDeleted=false;
					textItemType.LastModifiedDate=DateTime.Now;
                db.Entry(textItemType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(textItemType);
        }

        // GET: TextItemTypes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextItemType textItemType = db.TextItemTypes.Find(id);
            if (textItemType == null)
            {
                return HttpNotFound();
            }
            return View(textItemType);
        }

        // POST: TextItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TextItemType textItemType = db.TextItemTypes.Find(id);
			textItemType.IsDeleted=true;
			textItemType.DeletionDate=DateTime.Now;
 
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
