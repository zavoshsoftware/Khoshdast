using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class ProductGroupsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();


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
                CodeGenerator codeGenerator = new CodeGenerator();

                productGroup.UrlParam = GetUrlParam(productGroup.Title);
                productGroup.Code = codeGenerator.ReturnProductGroupCode();
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

        public string GetUrlParam(string title)
        {
            title = ReplaceCharachter(title, '@');
            title = ReplaceCharachter(title, '#');
            title = ReplaceCharachter(title, '$');
            title = ReplaceCharachter(title, '&');
            title = ReplaceCharachter(title, '^');
            title = ReplaceCharachter(title, '/');
            title = ReplaceCharachter(title, ']');
            title = ReplaceCharachter(title, '[');
            title = ReplaceCharachter(title, '%');
            title = ReplaceCharachter(title, '?');
            title = ReplaceCharachter(title, '؟');
            title = ReplaceCharachter(title, '!');

            return title.Replace(' ', '-');
        }

        public string ReplaceCharachter(string title, char charachter)
        {
            if (title.Contains(charachter))
                return title.Replace(charachter, '-');
            return title;
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

        [AllowAnonymous]
        [Route("category")]
        public ActionResult List()
        {

            ProductGroupListViewModel productGroupList = new ProductGroupListViewModel()
            {
                ProductGroups = db.ProductGroups.Where(c => c.ParentId == null && c.IsDeleted == false && c.IsActive).ToList(),
            };

            return View(productGroupList);
        }

        public string GetProductGroupsCodes()
        {
            List<ProductGroup> productGroups = db.ProductGroups.Where(c => c.IsDeleted == false).ToList();

            int i = 1;
            foreach (ProductGroup productGroup in productGroups)
            {
                productGroup.Code = i;
                i++;
            }
            db.SaveChanges();
            return String.Empty;
        }
        public string GetProductGroupsUrlParam()
        {
            List<ProductGroup> productGroups = db.ProductGroups.Where(c => c.IsDeleted == false).ToList();

            int i = 1;
            foreach (ProductGroup productGroup in productGroups)
            {
                productGroup.UrlParam = GetUrlParam(productGroup.Title);
            }
            db.SaveChanges();
            return String.Empty;
        }

        public string InsertGroups()
        {
            Guid parent1 = Guid.NewGuid();
            Guid parent2 = Guid.NewGuid();
            Guid parent3 = Guid.NewGuid();

            InsertProGr(parent1, "ابزار نقاشی", 1, null);

            Guid parent11 = Guid.NewGuid();
            InsertProGr(parent11, "غلطک طرح‌دار", 1, parent1);

            InsertProGr(Guid.NewGuid(), "رول یدک طرح‌دار GR", 1, parent11);
            InsertProGr(Guid.NewGuid(), "رول یدک طرح‌دار آبی Z", 1, parent11);
            InsertProGr(Guid.NewGuid(), "غلطک اسفنجی 9 اینچ", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک اسفنجی 4 اینچ", 1, parent11);
            InsertProGr(Guid.NewGuid(), "غلطک تکسچر ٩اینچ", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک تکسچر 7 اینچ", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک تکسچر 7 اینچ b", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک تکسچر 4 اینچ", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک طرح‌دار چرمی", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک طرح دار پارچه‌اى TM", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  غلطک طرح دار IN - F", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  سایر غلطک‌ها", 1, parent11);
            InsertProGr(Guid.NewGuid(), "  دسته غلطک", 1, parent11);

            Guid parent12 = Guid.NewGuid();
            InsertProGr(parent12, "غلطک ساده", 2, parent1);


            InsertProGr(Guid.NewGuid(), " غلطک 25 سانتی", 1, parent12);
            InsertProGr(Guid.NewGuid(), " غلطک 20 سانتی", 1, parent12);
            InsertProGr(Guid.NewGuid(), " غلطک 15 سانتی", 1, parent12);
            InsertProGr(Guid.NewGuid(), " غلطک 10 سانتی", 1, parent12);
            InsertProGr(Guid.NewGuid(), " غلطک 5 سانتی", 1, parent12);


            Guid parent13 = Guid.NewGuid();
            InsertProGr(parent13, "شابلون", 3, parent1);



            InsertProGr(Guid.NewGuid(), "شابلون 40x12 سانتی ", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون 27x20 سانتی", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون 120x60 سانتی", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون 60x40 سانتی", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون طرح سنگ SF6", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون پوست حیوانات", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون ANA", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون ANB", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون ANC", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون AND", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون ANF", 1, parent13);
            InsertProGr(Guid.NewGuid(), "شابلون حروف و اعداد ", 1, parent13);



            Guid parent14 = Guid.NewGuid();
            InsertProGr(parent14, "مهر و استامپ", 3, parent1);



            InsertProGr(Guid.NewGuid(), "مهر کوچک", 1, parent14);
            InsertProGr(Guid.NewGuid(), "مهر بزرگ", 1, parent14);
            InsertProGr(Guid.NewGuid(), "استامپ", 1, parent14);





            Guid parent15 = Guid.NewGuid();
            InsertProGr(parent15, "ابزار پتینه", 3, parent1);


            InsertProGr(Guid.NewGuid(), " ابزار طرح چوب", 1, parent15);
            InsertProGr(Guid.NewGuid(), "ابزار طرح‌دار", 1, parent15);
            InsertProGr(Guid.NewGuid(), " میکسر", 1, parent15);
            InsertProGr(Guid.NewGuid(), "سنباده گیر", 1, parent15);
            InsertProGr(Guid.NewGuid(), "ابزار اپوکسى", 1, parent15);
            InsertProGr(Guid.NewGuid(), "قلم پتینه", 1, parent15);

            Guid parent16 = Guid.NewGuid();
            InsertProGr(parent16, "ابزار پتینه", 3, parent1);


            InsertProGr(Guid.NewGuid(), "قلم هنری", 1, parent16);
            InsertProGr(Guid.NewGuid(), "قلم ساختمانی", 1, parent16);

            InsertProGr(Guid.NewGuid(), "سنباده و پوستاب", 3, parent1);
            InsertProGr(Guid.NewGuid(), "لیسه و کاردک", 3, parent1);
            InsertProGr(Guid.NewGuid(), "ماله", 3, parent1);



            InsertProGr(parent2, "رنگ", 2, null);

            Guid parent21 = Guid.NewGuid();
            InsertProGr(parent21, "رنگ ساختمانی", 3, parent2);
 

            InsertProGr(Guid.NewGuid(), " رنگ اکریلیک", 1, parent21);
           InsertProGr(Guid.NewGuid(), " رنگ روغنی", 1, parent21);
           InsertProGr(Guid.NewGuid(), " رنگ پلاستیک", 1, parent21);
           InsertProGr(Guid.NewGuid(), " رنگ فوری", 1, parent21);
           InsertProGr(Guid.NewGuid(), " آستر و عایق", 1, parent21);
           InsertProGr(Guid.NewGuid(), " بتونه و درزگیر", 1, parent21);
           InsertProGr(Guid.NewGuid(), " حلال", 1, parent21);
           InsertProGr(Guid.NewGuid(), " مادر رنگ", 1, parent21);


            Guid parent22 = Guid.NewGuid();
            InsertProGr(parent22, "رنگ ساختمانی", 3, parent2);
             

                InsertProGr(Guid.NewGuid(), " رنگ‌ اکریلیک", 1, parent22);
               InsertProGr(Guid.NewGuid(), " رنگ مولتی سورفیس", 1, parent22);
               InsertProGr(Guid.NewGuid(), " رنگ فلئورسنت", 1, parent22);
               InsertProGr(Guid.NewGuid(), " رنگ مولتی رزین", 1, parent22);
               InsertProGr(Guid.NewGuid(), " لاینر", 1, parent22);
               InsertProGr(Guid.NewGuid(), " تکسچر", 1, parent22);
               InsertProGr(Guid.NewGuid(), " مدیوم", 1, parent22);
               InsertProGr(Guid.NewGuid(), " رنگ کهنه کاری(آنتیک)", 1, parent22);
               InsertProGr(Guid.NewGuid(), " پودر و اکلیل", 1, parent22);
               InsertProGr(Guid.NewGuid(), " مطلا کاری", 1, parent22);
               InsertProGr(Guid.NewGuid(), " جوهر رنگ", 1, parent22);
               InsertProGr(Guid.NewGuid(), " تثبیت کننده", 1, parent22);


            InsertProGr(Guid.NewGuid(), "اسپری رنگ", 3, parent2);


            InsertProGr(parent3, "چسب", 3, null);
          
           

            InsertProGr(Guid.NewGuid(), " نوار چسب", 1, parent3);
            InsertProGr(Guid.NewGuid(), " چسب فوری", 1, parent3);
           InsertProGr(Guid.NewGuid(), " چسب چوب", 1, parent3);
           InsertProGr(Guid.NewGuid(), " چسب سنگ", 1, parent3);
           InsertProGr(Guid.NewGuid(), " چسب کاشی", 1, parent3);
           InsertProGr(Guid.NewGuid(), " چسب بتن", 1, parent3);
           InsertProGr(Guid.NewGuid(), " پولیش", 1, parent3);
           InsertProGr(Guid.NewGuid(), " چسب چند کاره", 1, parent3);



            db.SaveChanges();
            return string.Empty;
        }

        public void InsertProGr(Guid id, string title, int order, Guid? parentId)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            ProductGroup productGroup = new ProductGroup()
            {
                Id = id,
                Title = title,
                Order = order,
                ParentId = parentId,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now,
                Code = codeGenerator.ReturnProductGroupCode(),
                UrlParam = GetUrlParam(title),

            };

            db.ProductGroups.Add(productGroup);
        }
    }
}
