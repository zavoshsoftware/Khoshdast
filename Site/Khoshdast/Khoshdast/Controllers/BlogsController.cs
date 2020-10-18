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
using ViewModels;

namespace Khoshdast.Controllers
{
    public class BlogsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var blogs = db.Blogs.Include(b => b.BlogGroup).Where(b => b.IsDeleted == false).OrderByDescending(b => b.CreationDate);
            return View(blogs.ToList());
        }


        public ActionResult Create()
        {
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }


                #endregion

                blog.Visit = 0;
                blog.IsDeleted = false;
                blog.CreationDate = DateTime.Now;

                blog.Id = Guid.NewGuid();
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }


                #endregion
                blog.IsDeleted = false;
                blog.LastModifiedDate = DateTime.Now;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Blog blog = db.Blogs.Find(id);
            blog.IsDeleted = true;
            blog.DeletionDate = DateTime.Now;

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

        [Route("blog")]
        public ActionResult List()
        {
            List<Blog> blogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            BlogListViewModel blogList=new BlogListViewModel()
            {
                Blogs = blogs
            };
            return View(blogList);
        }


        [Route("blog/{urlParam}")]
        public ActionResult ListByGroup(string urlParam)
        {
            BlogGroup blogGroup = db.BlogGroups.FirstOrDefault(c => c.UrlParam == urlParam && c.IsActive);

            if (blogGroup == null)
                return Redirect("/blog");

            List<Blog> blogs = db.Blogs.Where(c => c.BlogGroupId == blogGroup.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            BlogListViewModel blogList=new BlogListViewModel()
            {
                Blogs = blogs,
                BlogGroup = blogGroup
            };
            return View(blogList);
        }


        [Route("blog/post/{urlParam}")]
        public ActionResult Details(string urlParam)
        {
         
            Blog blog = db.Blogs.FirstOrDefault(c=>c.UrlParam==urlParam);
            if (blog == null)
            {
                return Redirect("/blog");
            }

            blog.Visit++;
            db.SaveChanges();

            BlogDetailViewModel detail = new BlogDetailViewModel()
            {
                Blog = blog,
                BlogComments = db.BlogComments.Where(c => c.BlogId == blog.Id && c.IsActive && c.IsDeleted == false).ToList(),
                SidebarBlogGroups = db.BlogGroups.Where(c =>  c.IsActive && c.IsDeleted == false).ToList(),
                SidebarRecentBlogs = db.Blogs.Where(c => c.IsActive && c.IsDeleted == false).OrderByDescending(c=>c.CreationDate).Take(4).ToList(),
                NextBlog = GetNeighbourBlogs("next",blog),
               PrevBlog =  GetNeighbourBlogs("prev",blog),

            };

            return View(detail);
        }

        public Blog GetNeighbourBlogs(string type, Blog blog)
        {
            if (type == "next")
            {
                Blog nextBlog = db.Blogs.FirstOrDefault(c =>
                    c.CreationDate > blog.CreationDate && c.IsDeleted == false && c.IsActive);

                if (nextBlog != null)
                    return nextBlog;
                else
                    return db.Blogs.FirstOrDefault(c => c.IsActive && c.IsDeleted == false);
            }
            else
            {
                Blog prevBlog = db.Blogs.FirstOrDefault(c =>
                    c.CreationDate < blog.CreationDate && c.IsDeleted == false && c.IsActive);

                if (prevBlog != null)
                    return prevBlog;
                else
                    return db.Blogs.FirstOrDefault(c => c.IsActive && c.IsDeleted == false);
            }
        }
    }
}
