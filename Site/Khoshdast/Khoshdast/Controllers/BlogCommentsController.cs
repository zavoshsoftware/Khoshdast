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
    public class BlogCommentsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var blogComments = db.BlogComments.Include(b => b.Blog).Where(b=>b.IsDeleted==false).OrderByDescending(b=>b.CreationDate);
            return View(blogComments.ToList());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.BlogComments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        public ActionResult Create()
        {
            ViewBag.BlogId = new SelectList(db.Blogs, "Id", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlogComment blogComment)
        {
            if (ModelState.IsValid)
            {
				blogComment.IsDeleted=false;
				blogComment.CreationDate= DateTime.Now; 
					
                blogComment.Id = Guid.NewGuid();
                db.BlogComments.Add(blogComment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogId = new SelectList(db.Blogs, "Id", "Title", blogComment.BlogId);
            return View(blogComment);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.BlogComments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogId = new SelectList(db.Blogs, "Id", "Title", blogComment.BlogId);
            return View(blogComment);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlogComment blogComment)
        {
            if (ModelState.IsValid)
            {
				blogComment.IsDeleted=false;
					blogComment.LastModifiedDate=DateTime.Now;
                db.Entry(blogComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogId = new SelectList(db.Blogs, "Id", "Title", blogComment.BlogId);
            return View(blogComment);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.BlogComments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            BlogComment blogComment = db.BlogComments.Find(id);
			blogComment.IsDeleted=true;
			blogComment.DeletionDate=DateTime.Now;
 
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
        public ActionResult SubmitComment(string name, string email, string body, string code,string site)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
                return Json("InvalidEmail", JsonRequestBehavior.AllowGet);
            else
            {

                Blog blog =
                    db.Blogs.FirstOrDefault(c => c.UrlParam == code);

                if (blog != null)
                {
                    BlogComment comment = new BlogComment();

                    comment.Name = name;
                    comment.Email = email;
                    comment.Message = body;
                    comment.CreationDate = DateTime.Now;
                    comment.IsDeleted = false;
                    comment.Id = Guid.NewGuid();
                    comment.BlogId = blog.Id;
                    comment.IsActive = false;
                    comment.Website = site;

                    db.BlogComments.Add(comment);
                    db.SaveChanges();
                    return Json("true", JsonRequestBehavior.AllowGet);
                }

                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }
    }
}
