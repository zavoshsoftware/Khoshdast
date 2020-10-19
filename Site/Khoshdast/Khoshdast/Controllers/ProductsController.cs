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

        #region CRUD

        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Brand).Where(p => p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
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
                    Summery = product.Summery,
                    IsTopSell = product.IsTopSale
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
                bool isSelected = false;

                if (selectedProductGroups != null)
                    isSelected = selectedProductGroups.Any(c => c.Id == productGroup.Id);

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
                Stock = oProduct.Stock,
                IsTopSale = oProduct.IsTopSell

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
                    product.IsTopSell = oproduct.IsTopSale;
                    


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
                productGroupRelProduct.DeletionDate = DateTime.Now;
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

        #endregion


        [AllowAnonymous]
        [Route("category/{urlParam}")]
        public ActionResult List(string urlParam, string[] brands)
        {
            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == urlParam);

            if (productGroup == null)
                return Redirect("/");



            ProductListViewModel productList = new ProductListViewModel()
            {
                ProductGroup = productGroup,
                Products = GetProductListByProductGroupId(productGroup.Id, brands),
                SidebarBrands = GetSidebarBrands(brands),
                SidebarProductGroups = GetSidebarProductGroups(),
            };

            return View(productList);
        }




        [AllowAnonymous]
        [Route("product/{code}")]
        public ActionResult Details(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.FirstOrDefault(c=>c.Code==code);
            if (product == null)
            {
                return HttpNotFound();
            }

            ProductDetailViewModel productDetail = new ProductDetailViewModel()
            {
                Product = product,

                ProductComments =
                    db.ProductComments
                        .Where(c => c.ProductId == product.Id && c.IsActive == true && c.IsDeleted == false)
                        .OrderByDescending(c => c.CreationDate).ToList(),

                RelatedProducts = db.Products.Where(c => c.IsDeleted == false && c.IsActive)
                    .OrderByDescending(c => c.CreationDate).Take(6).ToList(),

                ProductGroup = GetProductGroup(product.Id)
            };
            return View(productDetail);
        }


        #region HelperMethods

        public ProductGroup GetProductGroup(Guid productId)
        {
            ProductGroupRelProduct productGroupRel =
                db.ProductGroupRelProducts.FirstOrDefault(c => c.ProductId == productId);

            if (productGroupRel != null)
                return productGroupRel.ProductGroup;

            else
                return db.ProductGroups.FirstOrDefault();
        }

        public List<Product> GetProductListByProductGroupId(Guid productGroupId, string[] brands)
        {
            List<Product> products = new List<Product>();

            List<ProductGroupRelProduct> productGroupRel = db.ProductGroupRelProducts
                .Where(c => (c.ProductGroupId == productGroupId || c.ProductGroup.ParentId == productGroupId) && c.IsDeleted == false).ToList();

            if (brands != null)
            {
                foreach (ProductGroupRelProduct groupRelProduct in productGroupRel)
                {
                    foreach (string brand in brands)
                    {
                        if (groupRelProduct.Product.Brand.UrlParam == brand)
                        {
                            if (!products.Any(c => c.Id == groupRelProduct.ProductId))
                                products.Add(groupRelProduct.Product);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (ProductGroupRelProduct groupRelProduct in productGroupRel)
                {
                    if (!products.Any(c => c.Id == groupRelProduct.ProductId))
                        products.Add(groupRelProduct.Product);
                }
            }

            return products;
        }
        public List<SidebarProductGroup> GetSidebarProductGroups()
        {
            List<SidebarProductGroup> list = new List<SidebarProductGroup>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();


            foreach (ProductGroup productGroup in productGroups)
            {
                list.Add(new SidebarProductGroup()
                {
                    ProductGroup = productGroup,

                    Quantity = db.ProductGroupRelProducts.Count(c =>
                        (c.ProductGroupId == productGroup.Id || c.ProductGroup.ParentId == productGroup.Id) &&
                        c.IsDeleted == false)
                });
            }

            return list;
        }
        public List<SidebarBrand> GetSidebarBrands(string[] selectedBrands)
        {
            List<SidebarBrand> list = new List<SidebarBrand>();

            List<Brand> brands = db.Brands
                .Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();


            foreach (Brand brand in brands)
            {
                list.Add(new SidebarBrand()
                {
                    Brand = brand,
                    IsSelected = false
                });

                if (selectedBrands != null)
                {
                    if (selectedBrands.Any(c => c == brand.UrlParam))
                    {
                        list.LastOrDefault().IsSelected = true;
                    }
                }
            }

            return list;
        }
        #endregion
    }
}

