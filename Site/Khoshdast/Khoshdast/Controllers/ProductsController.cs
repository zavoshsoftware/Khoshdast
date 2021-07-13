using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ClosedXML.Excel;
using Helpers;
using Kendo.Mvc.Extensions;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        #region CRUD

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            List<Product> products = db.Products.Include(p => p.Brand).Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.CreationDate).ToList();

            List<AdminProductListViewModel> result = new List<AdminProductListViewModel>();

            foreach (Product product in products)
            {
                result.Add(new AdminProductListViewModel()
                {
                    Id = product.Id,
                    Title = product.Title,
                    IsActive = product.IsActive,
                    ImageUrl = product.ImageUrl,
                    Stock = product.Stock.ToString(),
                    IsInPromotion = product.IsInPromotion,
                    Amount = product.AmountStr,
                    BarCode = product.Barcode,
                    BrandTitle = product.Brand.Title,
                    ProductGroupTitle = GetProductGroups(product.Id),
                    IsNewest = product.IsInHome


                });
            }
            return View(result);
        }

        public string GetProductGroups(Guid productId)
        {
            List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                .Where(c => c.ProductId == productId && c.IsDeleted == false).ToList();

            string groupTitle = "";

            int index = 0;
            foreach (ProductGroupRelProduct productRel in productGroupRelProducts)
            {
                if (index == 0)
                    groupTitle += productRel.ProductGroup.Title;
                else
                    groupTitle += " - " + productRel.ProductGroup.Title;

                index++;
            }

            return groupTitle;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title");

            ProductCrudViewModel product = new ProductCrudViewModel()
            {
                ProductGroups = GetProductGroupCheckboxLists(null)
            };

            return View(product);
        }
        CodeGenerator codeGenerator = new CodeGenerator();

        [Authorize(Roles = "Administrator")]
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
                    Code = codeGenerator.ReturnProductCode(),
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
                    IsTopSell = product.IsTopSale,
                    Barcode = product.Barcode
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

            return list.OrderBy(c => c.Title).ToList();
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

        [Authorize(Roles = "Administrator")]
        public ActionResult SetDiscountForGroup(Guid id)
        {

            ProductGroup oProductGroup = db.ProductGroups.Find(id);
            if (oProductGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.productGroupTitle = oProductGroup.Title;

            ViewBag.productCount = db.ProductGroupRelProducts.Count(c => c.ProductGroupId == id && c.IsDeleted == false);
            return View();
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDiscountForGroup(ProductGroupDiscountViewModel input, Guid id)
        {
            if (ModelState.IsValid)
            {
                ProductGroup oProductGroup = db.ProductGroups.Find(id);

                if (oProductGroup != null)
                {
                    List<ProductGroupRelProduct> productGroupRelProducts = db.ProductGroupRelProducts
                        .Where(c => c.ProductGroupId == oProductGroup.Id && c.IsDeleted == false).ToList();
                    List<Product> products = new List<Product>();

                    foreach (ProductGroupRelProduct productGroupRelProduct in productGroupRelProducts)
                    {
                        if (!products.Contains(productGroupRelProduct.Product))
                            products.Add(productGroupRelProduct.Product);
                    }

                    foreach (Product product in products)
                    {
                        decimal discountAmount = product.Amount - input.Amount;

                        if (input.IsPercent)
                        {
                            discountAmount = product.Amount - (product.Amount * input.Amount / 100);
                        }

                        product.DiscountAmount = discountAmount;
                        product.IsInPromotion = true;
                        product.LastModifiedDate = DateTime.Now;

                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "ProductGroups");
            }
            return View(input);
        }
        [Authorize(Roles = "Administrator")]
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
                IsTopSale = oProduct.IsTopSell,
                Barcode = oProduct.Barcode,

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


        [Authorize(Roles = "Administrator")]
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
                    product.Barcode = oproduct.Barcode;


                    RemoveProductRel(oproduct.Id);
                    PostProductGroupsRelProducts(oproduct.Id, oproduct.ProductGroups);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Title", oproduct.BrandId);
            return View(oproduct);
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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
        public ActionResult List(string urlParam, string[] brands, int? pageId, string sortby)
        {
            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == urlParam);

            if (productGroup == null)
                return Redirect("/");

            ViewBag.url = GetUrl(brands, urlParam);

            if (pageId == null)
                pageId = 1;

            List<Product> products = GetProductListByProductGroupId(productGroup.Id);

            List<SidebarBrand> sidebarBrands = GetSidebarBrands(brands, products);

            products = GetProductListByBrandFilter(products, brands);

            if (products.Count() <= productPagination)
                ViewBag.isLastPage = "true";
            else
                ViewBag.isLastPage = "false";

            ProductListViewModel productList = new ProductListViewModel()
            {
                ProductGroup = productGroup,
                Products = GetProductByPagination(products, pageId, sortby),
                SidebarBrands = sidebarBrands,
                SidebarProductGroups = GetSidebarProductGroups(productGroup),
                BreadcrumbItems = GetBreadCrumb(productGroup.Parent).OrderBy(c => c.Order).ToList(),
                //PageItems = GetPagination(products.Count(), pageId),
                SidebarBanners = db.SidebarBanners.Where(c => c.IsActive && c.IsDeleted == false).ToList()
            };

            return View(productList);
        }


        public ActionResult GetNewPage(string page, string sort, string brands, string category)
        {

            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == category);

            if (productGroup == null)
                return Redirect("/");

            string[] arrayBrands = null;
            if (!string.IsNullOrEmpty(brands))
            {
                arrayBrands = brands.Split('-');
            }
            ViewBag.url = GetUrl(arrayBrands, category);

            if (string.IsNullOrEmpty(sort))
            {
                sort = null;
            }

            int pageId = 1;
            if (page != null)
                pageId = Convert.ToInt32(page);

            bool isLastBatch = false;

            List<Product> products = GetProductListByProductGroupId(productGroup.Id);

            products = GetProductListByBrandFilter(products, arrayBrands);



            products = GetProductByPagination(products, pageId, sort);

            if (products.Count < productPagination)
                isLastBatch = true;

            List<LazyLoadProductCardsItemViewModel> resItem = new List<LazyLoadProductCardsItemViewModel>();

            foreach (var product in products)
            {
                string amount = "";
                if (product.Stock > 0)
                {
                    amount = product.AmountStr;

                    //if (product.IsInPromotion)
                    //    amount = product.DiscountAmountStr;

                    if (product.Amount == 0)
                        amount = (WebConfigurationManager.AppSettings["CallForAmount"]);
                }
                string imageUrl = product.ImageUrl;

                if (!System.IO.File.Exists(Server.MapPath(product.ImageUrl)) || string.IsNullOrEmpty(product.ImageUrl))
                    imageUrl = "/assets/images/no-Photo.jpg";
                string discountAmount = "";

                if (product.IsInPromotion)
                    discountAmount = product.DiscountAmountStr;

                resItem.Add(new LazyLoadProductCardsItemViewModel()
                {
                    Title = product.Title,
                    Amount = amount,
                    ImageUrl = imageUrl,
                    Code = product.Code,
                    Stock = product.Stock,
                    DiscountAmount = discountAmount
                });
            }

            LazyLoadProductCardsViewModel res = new LazyLoadProductCardsViewModel()
            {
                Result = resItem,
                IsLastBatch = isLastBatch.ToString()
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNewPageForBrand(string page, string sort, string brand)
        {

            ViewBag.url = GetUrlByBrand(brand);

            int pageId = 1;
            if (page != null)
                pageId = Convert.ToInt32(page);



            Brand oBrand = db.Brands.FirstOrDefault(c => c.UrlParam == brand);

            List<Product> products = GetProductListByBrandId(oBrand.Id);

            products = GetProductByPagination(products, pageId, sort);

            bool isLastBatch = products.Count < productPagination;

            List<LazyLoadProductCardsItemViewModel> resItem = new List<LazyLoadProductCardsItemViewModel>();

            foreach (var product in products)
            {
                string amount = "";
                if (product.Stock > 0)
                {
                    amount = product.AmountStr;

                    //if (product.IsInPromotion)
                    //    amount = product.DiscountAmountStr;

                    if (product.Amount == 0)
                        amount = (WebConfigurationManager.AppSettings["CallForAmount"]);
                }
                string discountAmount = "";

                if (product.IsInPromotion)
                    discountAmount = product.DiscountAmountStr;

                string imageUrl = product.ImageUrl;

                if (!System.IO.File.Exists(Server.MapPath(product.ImageUrl)) || string.IsNullOrEmpty(product.ImageUrl))
                    imageUrl = "/assets/images/no-Photo.jpg";


                resItem.Add(new LazyLoadProductCardsItemViewModel()
                {
                    Title = product.Title,
                    Amount = amount,
                    ImageUrl = imageUrl,
                    Code = product.Code,
                    Stock = product.Stock,
                    DiscountAmount = discountAmount
                });
            }

            LazyLoadProductCardsViewModel res = new LazyLoadProductCardsViewModel()
            {
                Result = resItem,
                IsLastBatch = isLastBatch.ToString()
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private int productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPagination"]);

        public string GetUrl(string[] brands, string urlParam)
        {
            string url = "/category/" + urlParam;
            ViewBag.hasQs = true;
            if (brands != null)
            {
                url += "?";
                for (int i = 0; i < brands.Length; i++)
                {
                    url += "brands[" + i + "]=" + brands[i];

                    if (i != brands.Length - 1)
                        url += "&";
                }
                ViewBag.hasQs = false;
            }


            return url;
        }

        public string GetUrlByBrand(string urlParam)
        {
            string url = "/brand/" + urlParam;
            ViewBag.hasQs = false;

            return url;
        }

        public List<PageItem> GetPagination(int productCount, int? pageId)
        {
            List<PageItem> result = new List<PageItem>();

            int pageNumbers = (int)Math.Ceiling(productCount / (double)productPagination);

            for (int i = 1; i <= pageNumbers; i++)
            {
                bool isActive = pageId == i;

                PageItem pageItem = new PageItem()
                {
                    PageId = i,
                    IsCurrentPage = isActive
                };
                result.Add(pageItem);
            }

            return result;
        }

        [AllowAnonymous]
        [Route("product/{code}")]
        public ActionResult Details(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int proCode = Convert.ToInt32(code);
            Product product = db.Products.FirstOrDefault(c => c.Code == proCode);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductGroup productGroup = GetProductGroup(product.Id);
            ProductDetailViewModel productDetail = new ProductDetailViewModel()
            {
                Product = product,

                ProductComments =
                    db.ProductComments
                        .Where(c => c.ProductId == product.Id && c.IsActive == true && c.IsDeleted == false)
                        .OrderByDescending(c => c.CreationDate).ToList(),

                RelatedProducts = db.Products.Where(c => c.IsDeleted == false && c.IsActive)
                    .OrderByDescending(c => c.CreationDate).Take(6).ToList(),

                ProductGroup = productGroup,
                BreadcrumbItems = GetBreadCrumb(productGroup).OrderBy(c => c.Order).ToList(),

            };
            return View(productDetail);
        }


        #region HelperMethods

        public List<Product> GetProductByPagination(List<Product> products, int? pageId, string sortby)
        {
            if (sortby == null)
                sortby = "newest";
               //return products.OrderByDescending(c => c.Stock).ThenBy(c => c.Order).Skip((pageId.Value - 1) * productPagination).Take(productPagination)
               // .ToList();
                          

            return SortProductList(products.OrderByDescending(c => c.Stock).ThenBy(c => c.Order).ToList(), sortby).Skip((pageId.Value - 1) * productPagination).Take(productPagination)
                .ToList();
        }

        public List<Product> SortProductList(List<Product> products, string sortby)
        {
            switch (sortby)
            {
                case "newest":
                    {
                        return products.OrderByDescending(c => c.CreationDate).ToList();
                    }

                case "mostsell":
                    {
                        return products.OrderByDescending(c => c.SellNumber).ToList();
                    }

                case "cheapest":
                    {
                        return products.OrderBy(c => c.Amount).ToList();
                    }

                case "expensive":
                    {
                        return products.OrderByDescending(c => c.Amount).ToList();
                    }

                case "mostdiscount":
                    {
                        return products.OrderBy(c => c.DiscountAmount).ToList();
                    }
                default:
                    {
                        return products.OrderBy(c => c.CreationDate).ToList();
                    }
            }
        }

        public ProductGroup GetProductGroup(Guid productId)
        {
            ProductGroupRelProduct productGroupRel =
                db.ProductGroupRelProducts.FirstOrDefault(c => c.ProductId == productId);

            if (productGroupRel != null)
                return productGroupRel.ProductGroup;

            else
                return db.ProductGroups.FirstOrDefault();
        }

        public List<Product> GetProductListByProductGroupId(Guid productGroupId)
        {
            List<Product> products = new List<Product>();

            List<ProductGroupRelProduct> productGroupRel = db.ProductGroupRelProducts
                .Where(c => (c.ProductGroupId == productGroupId || c.ProductGroup.ParentId == productGroupId
                             || c.ProductGroup.Parent.ParentId == productGroupId) && c.IsDeleted == false).ToList();


            foreach (ProductGroupRelProduct groupRelProduct in productGroupRel)
            {
                if (!products.Any(c => c.Id == groupRelProduct.ProductId))
                    products.Add(groupRelProduct.Product);
            }


            return products.OrderByDescending(c => c.Stock).ThenByDescending(c => c.Amount).ToList();
        }
        public List<Product> GetProductListByBrandFilter(List<Product> products, string[] brands)
        {
            if (brands != null)
            {
                List<Product> result = new List<Product>();

                foreach (Product product in products)
                {
                    foreach (string brand in brands)
                    {
                        if (product.Brand.UrlParam == brand)
                        {
                            if (result.All(p => p.Id != product.Id))
                                result.Add(product);
                        }
                    }
                }

                return result;
            }

            return products;
        }

        public List<Product> GetProductListByBrandId(Guid brandId)
        {
            return db.Products.Where(c => c.BrandId == brandId && c.IsActive && c.IsDeleted == false).ToList();
        }
        public List<ProductGroup> GetSidebarProductGroups(ProductGroup currentProductGroup)
        {
            List<SidebarProductGroup> list = new List<SidebarProductGroup>();

            List<ProductGroup> productGroups = db.ProductGroups
                .Where(c => c.ParentId == currentProductGroup.Id && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

            if (!productGroups.Any())
                productGroups = db.ProductGroups
                     .Where(c => c.ParentId == currentProductGroup.ParentId && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();


            return productGroups;

        }
        public List<SidebarProductGroup> GetComplexSidebarProductGroups()
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
                        (c.ProductGroupId == productGroup.Id || c.ProductGroup.ParentId == productGroup.Id || c.ProductGroup.Parent.ParentId == productGroup.Id) &&
                        c.IsDeleted == false),

                    ChildProductGroups = db.ProductGroups.Where(c => c.ParentId == productGroup.Id && c.IsActive && c.IsDeleted == false).ToList()
                });
            }

            return list;
        }

        public List<SidebarBrand> GetSidebarBrands(string[] selectedBrands, List<Product> products)
        {
            List<SidebarBrand> list = new List<SidebarBrand>();

            List<Brand> brands = new List<Brand>();

            foreach (Product product in products)
            {
                if (brands.All(c => c.Id != product.BrandId))
                {
                    brands.Add(product.Brand);
                }
            }


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

            return list.OrderBy(c => c.Brand.Title).ToList();
        }


        public List<BreadcrumbItem> GetBreadCrumb(ProductGroup currenProductGroup)
        {
            List<BreadcrumbItem> result = new List<BreadcrumbItem>();

            //result.Add(GetBreadcrumbItem(currenProductGroup, 10));

            for (int i = 9; i > 1; i--)
            {
                if (currenProductGroup != null)
                {
                    result.Add(GetBreadcrumbItem(currenProductGroup, i));
                    currenProductGroup = GetRecursive(currenProductGroup);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public BreadcrumbItem GetBreadcrumbItem(ProductGroup currentProductGroup, int order)
        {
            BreadcrumbItem breadcrumb = new BreadcrumbItem()
            {
                Title = currentProductGroup.Title,
                UrlParam = currentProductGroup.UrlParam,
                Order = order
            };

            return breadcrumb;
        }
        public ProductGroup GetRecursive(ProductGroup currenProductGroup)
        {
            if (currenProductGroup.ParentId != null)
                return currenProductGroup.Parent;

            return null;
        }
        #endregion


        [AllowAnonymous]
        [Route("brand/{urlParam}")]
        public ActionResult ListByBrand(string urlParam, int? pageId, string sortby)
        {
            Brand brand = db.Brands.FirstOrDefault(c => c.UrlParam == urlParam);

            if (brand == null)
                return Redirect("/");

            ViewBag.url = GetUrlByBrand(urlParam);

            if (pageId == null)
                pageId = 1;

            List<Product> products = GetProductListByBrandId(brand.Id);

            ProductListByBrandViewModel productList = new ProductListByBrandViewModel()
            {
                brand = brand,
                Products = GetProductByPagination(products, pageId, sortby),
                SidebarProductGroups = GetComplexSidebarProductGroups(),
                PageItems = GetPagination(products.Count(), pageId),
                SidebarBanners = db.SidebarBanners.Where(c => c.IsActive && c.IsDeleted == false).ToList()
            };

            return View(productList);
        }

        public ActionResult TestLazy()
        {
            var model = db.Products.Where(c => c.ImageUrl != null).OrderBy(p => p.CreationDate).Take(30); ;
            return View(model);
        }



        public string ChangeBadCode()
        {
            List<Product> products = db.Products.Where(c => c.Code == 1788 && c.IsDeleted == false)
                .OrderBy(c => c.CreationDate).ToList();

            foreach (Product product in products)
            {
                product.Code = codeGenerator.ReturnProductCode();
                product.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }

            return string.Empty;
        }


        public string removebadsproducts()
        {
            Guid italian = new Guid("6CA03143-8F9E-4985-92F8-C52F6047465D");

            var ppg = db.ProductGroupRelProducts.Where(c => c.ProductGroupId == italian);

            foreach (var groupRelProduct in ppg)
            {
                Product p = db.Products.Where(c => c.Id == groupRelProduct.ProductId).FirstOrDefault();

                db.ProductGroupRelProducts.Remove(groupRelProduct);

                if (p != null)
                    db.Products.Remove(p);
            }
            db.SaveChanges();

            return string.Empty;
        }
        public string removeBadChars()
        {
            var products = db.Products.Where(c => c.Title.Contains("طرح‌دار"));

            foreach (var product in products)
            {
                product.Title = product.Title.Replace("طرح‌دار", "طرح دار");
            }
            db.SaveChanges();

            return string.Empty;
        }

        public async Task<ActionResult> ExportProductList()
        {
            /****************************( create excel file )******************************/
          
             string sheetTitle = "Sheet1";
         //  string index  = "ردیف";
           string  productTitle= "محصول";
           string barcode = "بارکد";
           string amount = "قیمت";
            string stock = "موجودی";
            string brand = "برند";
            string group = "گروه";
            var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add(sheetTitle);
            /***************************( table title)****************************/
            //workSheet.Cell("A1").Value = sheetTitle;
            /**************************( table column names)**************************************/
           // workSheet.Cell("A1").Value = index;
            workSheet.Cell("B1").Value = productTitle;
            workSheet.Cell("A1").Value = barcode;
            workSheet.Cell("C1").Value = group;
            workSheet.Cell("D1").Value = brand;
            workSheet.Cell("E1").Value = amount;
            workSheet.Cell("F1").Value = stock;
            /**************************( table rows values)**************************************/
            var products = await db.Products.ToListAsync();
            for (int i = 0; i < products.Count; i++)
            {
                //string indexCelNumber = "A" + (i + 2).ToString();
                string barcodeSheet = "A" + (i + 2).ToString();
                string productSheet = "B" + (i + 2).ToString();
                string groupSheet = "C" + (i + 2).ToString();
                string amountSheet = "E" + (i + 2).ToString();
                string stockInSheet = "F" + (i +2).ToString();
                string brandInSheet = "D" + (i + 2).ToString();
                //workSheet.Cell(indexCelNumber).Value = i.ToString();
                workSheet.Cell(barcodeSheet).Value = products[i].Barcode;
                workSheet.Cell(productSheet).Value = products[i].Title;
                workSheet.Cell(groupSheet).Value = GetProductGroupById(products[i].Id);
                workSheet.Cell(brandInSheet).Value = products[i].Brand.Title;
                workSheet.Cell(stockInSheet).Value = products[i].Stock;
                workSheet.Cell(amountSheet).Value = products[i].Amount*10;
            }

            // Prepare the response

            HttpContext.Response.Clear();

            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.AddHeader("content-disposition", "attachment;filename=\"لیست محصولات.xlsx\"");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workBook.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Response.OutputStream);
                memoryStream.Close();
            }
            HttpContext.Response.End();

            return RedirectToAction("Index", "Products");
        }

        public string GetProductGroupById(Guid id)
        {
            ProductGroupRelProduct productGroupRelProduct =
                db.ProductGroupRelProducts.FirstOrDefault(c => c.ProductId == id);

            if (productGroupRelProduct != null)
                return productGroupRelProduct.ProductGroup.Title;

            return "";
        }
    }
}



