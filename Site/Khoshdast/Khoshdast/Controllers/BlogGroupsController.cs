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
    [Authorize(Roles = "Administrator,Content")]
    public class BlogGroupsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.BlogGroups.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }
         
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlogGroup blogGroup)
        {
            if (ModelState.IsValid)
            {
				blogGroup.IsDeleted=false;
				blogGroup.CreationDate= DateTime.Now; 
					
                blogGroup.Id = Guid.NewGuid();
                db.BlogGroups.Add(blogGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogGroup);
        }

        // GET: BlogGroups/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogGroup blogGroup = db.BlogGroups.Find(id);
            if (blogGroup == null)
            {
                return HttpNotFound();
            }
            return View(blogGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlogGroup blogGroup)
        {
            if (ModelState.IsValid)
            {
				blogGroup.IsDeleted=false;
					blogGroup.LastModifiedDate=DateTime.Now;
                db.Entry(blogGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogGroup);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogGroup blogGroup = db.BlogGroups.Find(id);
            if (blogGroup == null)
            {
                return HttpNotFound();
            }
            return View(blogGroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            BlogGroup blogGroup = db.BlogGroups.Find(id);
			blogGroup.IsDeleted=true;
			blogGroup.DeletionDate=DateTime.Now;
 
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
