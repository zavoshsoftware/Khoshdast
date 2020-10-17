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
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Brand).Where(p => p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title");

            ProductCrudViewModel product = new ProductCrudViewModel()
            {
                ProductGroups = GetProductGroupCheckboxLists(null)
            };

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCrudViewModel product, HttpPostedFileBase fileupload)
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

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                }
                #endregion
                CodeGenerator codeGenerator = new CodeGenerator();

                Product oProduct = new Product()
                {
                    ImageUrl = newFilenameUrl,
                    Code = codeGenerator.ReturnProductCode().ToString(),
                    IsDeleted = false,
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    Stock = product.Stock,
                    SeedStock = product.SeedStock,
                    Visit = 0,
                    SellNumber = 0,
                    IsAvailable = true,
                    IsActive = product.IsActive,
                    Order = product.Order,
                    Title = product.Title,
                    Amount = product.Amount,
                    Body = product.Body,
                    BrandId = product.BrandId,
                    Description = product.Description,
                    IsInHome = product.IsInHome,
                    IsInPromotion = product.IsInPromotion,
                    DiscountAmount = product.DiscountAmount,
                    PageDescription = product.PageDescription,
                    PageTitle = product.PageTitle,
                    Summery = product.Summery
                };

                db.Products.Add(oProduct);

                PostProductGroupsRelProducts(oProduct.Id, product.ProductGroups);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title", product.BrandId);
            return View(product);
        }

        public List<ProductGroupCheckboxList> GetProductGroupCheckboxLists(List<ProductGroup> selectedProductGroups)
        {
            List<ProductGroupCheckboxList> list = new List<ProductGroupCheckboxList>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.IsDeleted == false && c.IsActive && c.ParentId != null).OrderBy(c => c.ParentId).ToList();

            foreach (ProductGroup productGroup in productGroups)
            {
                bool isSelected = selectedProductGroups.Any(c => c.Id == productGroup.Id);

                list.Add(new ProductGroupCheckboxList()
                {
                    Id = productGroup.Id,
                    Order = productGroup.Order,
                    Title = productGroup.Title,
                    IsSelected = isSelected
                });
            }

            return list;
        }

        public void PostProductGroupsRelProducts(Guid productId, List<ProductGroupCheckboxList> productGroups)
        {
            foreach (ProductGroupCheckboxList productGroupCheckboxList in productGroups)
            {
                if (productGroupCheckboxList.IsSelected)
                {
                    ProductGroupRelProduct productGroupRelProduct = new ProductGroupRelProduct()
                    {
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        IsActive = true,
                        ProductId = productId,
                        ProductGroupId = productGroupCheckboxList.Id,
                        CreationDate = DateTime.Now
                    };

                    db.ProductGroupRelProducts.Add(productGroupRelProduct);
                }
            }
        }

        public void RemoveProductRel(Guid productId)
        {
            List<ProductGroupRelProduct> productGroups =
                db.ProductGroupRelProducts.Where(c => c.ProductId == productId && c.IsDeleted == false).ToList();

            foreach (ProductGroupRelProduct productGroup in productGroups)
            {
                db.ProductGroupRelProducts.Remove(productGroup);
            }
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product oProduct = db.Products.Find(id);
            if (oProduct == null)
            {
                return HttpNotFound();
            }

            ProductCrudViewModel product = new ProductCrudViewModel()
            {
                Id = oProduct.Id,
                ProductGroups = GetProductGroupCheckboxLists(GetProductGroupListByProduct(id.Value)),
                Order = oProduct.Order,
                Title = oProduct.Title,
                IsActive = oProduct.IsActive,
                ImageUrl = oProduct.ImageUrl,
                Description = oProduct.Description,
                BrandId = oProduct.BrandId,
                DiscountAmount = oProduct.DiscountAmount,
                Body = oProduct.Body,
                PageDescription = oProduct.PageDescription,
                IsInPromotion = oProduct.IsInPromotion,
                PageTitle = oProduct.PageTitle,
                Summery = oProduct.Summery,
                Amount = oProduct.Amount,
                IsAvailable = oProduct.IsAvailable,
                SellNumber = oProduct.SellNumber,
                IsInHome = oProduct.IsInHome,
                Visit = oProduct.Visit,
                SeedStock = oProduct.SeedStock,
                Stock = oProduct.Stock
            };
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title", product.BrandId);


            return View(product);
        }

        public List<ProductGroup> GetProductGroupListByProduct(Guid productId)
        {
            List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                .Where(c => c.IsDeleted == false && c.ProductId == productId).ToList();

            List<ProductGroup> productGroups = new List<ProductGroup>();

            foreach (ProductGroupRelProduct productGroupRelProduct in productGroupRelProducts)
            {
                productGroups.Add(productGroupRelProduct.ProductGroup);
            }

            return productGroups;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCrudViewModel oproduct, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                Product product = db.Products.Find(oproduct.Id);
                if (product != null)
                {
                    #region Upload and resize image if needed
                    string newFilenameUrl = string.Empty;
                    if (fileupload != null)
                    {
                        string filename = Path.GetFileName(fileupload.FileName);
                        string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                             + Path.GetExtension(filename);

                        newFilenameUrl = "/Uploads/Product/" + newFilename;
                        string physicalFilename = Server.MapPath(newFilenameUrl);
                        fileupload.SaveAs(physicalFilename);
                        product.ImageUrl = newFilenameUrl;
                    }
                    #endregion

                    product.IsDeleted = false;
                    product.LastModifiedDate = DateTime.Now;

                    product.Stock = oproduct.Stock;
                    if (oproduct.Stock == 0)
                        product.IsAvailable = false;
                    product.IsActive = oproduct.IsActive;
                    product.Order = oproduct.Order;
                    product.Title = oproduct.Title;
                    product.Amount = oproduct.Amount;
                    product.Body = oproduct.Body;
                    product.BrandId = oproduct.BrandId;
                    product.Description = oproduct.Description;
                    product.IsInHome = oproduct.IsInHome;
                    product.IsInPromotion = oproduct.IsInPromotion;
                    product.DiscountAmount = oproduct.DiscountAmount;
                    product.PageDescription = oproduct.PageDescription;
                    product.PageTitle = oproduct.PageTitle;
                    product.Summery = oproduct.Summery;


                    RemoveProductRel(oproduct.Id);
                    PostProductGroupsRelProducts(oproduct.Id, oproduct.ProductGroups);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title", oproduct.BrandId);
            return View(oproduct);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

            List<ProductGroupRelProduct> products = db.ProductGroupRelProducts
                .Where(c => c.ProductId == id && c.IsDeleted == false).ToList();

            foreach (ProductGroupRelProduct productGroupRelProduct in products)
            {
                productGroupRelProduct.IsDeleted = true;
                productGroupRelProduct.DeletionDate=DateTime.Now;
            }

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
