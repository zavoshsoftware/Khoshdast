using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Gender).Where(u=>u.IsDeleted==false).OrderByDescending(u=>u.CreationDate).Include(u => u.Role).Where(u=>u.IsDeleted==false).OrderByDescending(u=>u.CreationDate);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Title");
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Title");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Password,CellNum,FullName,Code,Email,GenderId,RoleId,RemainCredit,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] User user)
        {
            if (ModelState.IsValid)
            {
				user.IsDeleted=false;
				user.CreationDate= DateTime.Now; 
					
                user.Id = Guid.NewGuid();
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Title", user.GenderId);
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Title", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Title", user.GenderId);
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Title", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Password,CellNum,FullName,Code,Email,GenderId,RoleId,RemainCredit,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] User user)
        {
            if (ModelState.IsValid)
            {
				user.IsDeleted=false;
					user.LastModifiedDate=DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Title", user.GenderId);
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Title", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            User user = db.Users.Find(id);
			user.IsDeleted=true;
			user.DeletionDate=DateTime.Now;
 
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

        //[Authorize]
        //[Route("profile")]
        //public ActionResult UserProfile()
        //{
        //    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

        //    string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

        //    Guid userId = new Guid(name);

        //    User user = db.Users.Find(userId);

        //    if (user != null)
        //    {
        //        ProfileViewModel profile = new ProfileViewModel()
        //        {
        //            Id = user.Id,
        //            CellNumber = user.CellNum,
        //            Email = user.Email,
        //            FullName = user.Email,
        //        };

        //        ViewBag.GenderId = new SelectList(db.Genders, "Id", "Title",user.GenderId);

        //        ViewBag.BirthDay = new SelectList(db.Genders, "Id", "Title", user.GenderId);
                

        //    }

        //    return View(profile);
        //}

    }
}
