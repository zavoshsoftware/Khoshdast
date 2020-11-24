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
    public class SidebarBannersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: SidebarBanners
        public ActionResult Index()
        {
            return View(db.SidebarBanners.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: SidebarBanners/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SidebarBanner sidebarBanner = db.SidebarBanners.Find(id);
            if (sidebarBanner == null)
            {
                return HttpNotFound();
            }
            return View(sidebarBanner);
        }

        // GET: SidebarBanners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SidebarBanners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SidebarBanner sidebarBanner, HttpPostedFileBase fileupload)
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
                    sidebarBanner.ImageUrl = newFilenameUrl;
                }
                #endregion

                sidebarBanner.IsDeleted=false;
				sidebarBanner.CreationDate= DateTime.Now; 
					
                sidebarBanner.Id = Guid.NewGuid();
                db.SidebarBanners.Add(sidebarBanner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sidebarBanner);
        }

        // GET: SidebarBanners/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SidebarBanner sidebarBanner = db.SidebarBanners.Find(id);
            if (sidebarBanner == null)
            {
                return HttpNotFound();
            }
            return View(sidebarBanner);
        }

        // POST: SidebarBanners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SidebarBanner sidebarBanner, HttpPostedFileBase fileupload)
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
                    sidebarBanner.ImageUrl = newFilenameUrl;
                }
                #endregion

                sidebarBanner.IsDeleted=false;
					sidebarBanner.LastModifiedDate=DateTime.Now;
                db.Entry(sidebarBanner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sidebarBanner);
        }

        // GET: SidebarBanners/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SidebarBanner sidebarBanner = db.SidebarBanners.Find(id);
            if (sidebarBanner == null)
            {
                return HttpNotFound();
            }
            return View(sidebarBanner);
        }

        // POST: SidebarBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            SidebarBanner sidebarBanner = db.SidebarBanners.Find(id);
			sidebarBanner.IsDeleted=true;
			sidebarBanner.DeletionDate=DateTime.Now;
 
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
