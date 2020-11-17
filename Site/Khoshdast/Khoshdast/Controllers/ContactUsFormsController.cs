using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Khoshdast.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ContactUsFormsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.ContactUsForms.Where(a => a.IsDeleted == false).OrderByDescending(a => a.CreationDate).ToList());
        }

        // GET: ContactUsForms/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // GET: ContactUsForms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactUsForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Message,Ip,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ContactUsForm contactUsForm)
        {
            if (ModelState.IsValid)
            {
                contactUsForm.IsDeleted = false;
                contactUsForm.CreationDate = DateTime.Now;

                contactUsForm.Id = Guid.NewGuid();
                db.ContactUsForms.Add(contactUsForm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactUsForm);
        }

        // GET: ContactUsForms/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // POST: ContactUsForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Message,Ip,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ContactUsForm contactUsForm)
        {
            if (ModelState.IsValid)
            {
                contactUsForm.IsDeleted = false;
                contactUsForm.LastModifiedDate = DateTime.Now;
                db.Entry(contactUsForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUsForm);
        }

        // GET: ContactUsForms/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // POST: ContactUsForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            contactUsForm.IsDeleted = true;
            contactUsForm.DeletionDate = DateTime.Now;

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



        [AllowAnonymous]
        [HttpPost]
        public ActionResult SubmitComment(string name, string email, string body)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
                return Json("InvalidEmail", JsonRequestBehavior.AllowGet);

            ContactUsForm comment = new ContactUsForm();

            comment.Name = name;
            comment.Email = email;
            comment.Message = body;
            comment.CreationDate = DateTime.Now;
            comment.IsDeleted = false;
            comment.Id = Guid.NewGuid();
            comment.IsActive = false;
            comment.Ip = Request.UserHostAddress;

            db.ContactUsForms.Add(comment);
            db.SaveChanges();
            return Json("true", JsonRequestBehavior.AllowGet);

        }
    }
}
