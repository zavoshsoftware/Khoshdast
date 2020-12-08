using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace Khoshdast.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            string topPgTitle = "جدیدترین محصولات";

            ProductGroup topProductGroup =
                db.ProductGroups.FirstOrDefault(c => c.IsHomeTopGroup && c.IsDeleted == false && c.IsActive);

            if (topProductGroup != null)
                topPgTitle = "جدیدترین مدل های " + topProductGroup.Title;

            List<TextItem> sliderBanners = db.TextItems.Where(c => c.TextItemType.Name == "sliderleft").ToList();
            List<TextItem> homeMidkeBanners = db.TextItems.Where(c => c.TextItemType.Name == "twohomebanner").ToList();

            HomeViewModel home = new HomeViewModel()
            {
                Sliders = db.Sliders.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                HomeProductGroups = db.ProductGroups.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Take(8).ToList(),
                BestSaleProducts = db.Products.Where(c => c.IsTopSell && c.Stock > 0 && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Take(8).ToList(),
                NewestProducts = db.Products.Where(c => c.IsInHome && c.Stock > 0 && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Take(8).ToList(),
                TopCategoryProducts = GetTopCategoryProducts(topProductGroup),
                TopProductGroupTitle = topPgTitle,
                HomeBlogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList(),
                HomeBrands = db.Brands.Where(c => c.BrandNameImageUrl != null && c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.Order).Take(10).ToList(),
                SliderLeftBanners1 = sliderBanners.FirstOrDefault(),
                SliderLeftBanners2 = sliderBanners.LastOrDefault(),
                HomeMidleBanner1 = homeMidkeBanners.FirstOrDefault(),
                HomeMidleBanner2 = homeMidkeBanners.LastOrDefault(),
                HomeMetaDescription = GetTextByName("metadesc")
            };
            return View(home);
        }

        public string GetTextByName(string name)
        {
            var textItem = db.TextItems.Where(c => c.Name == name).Select(c => c.Summery).FirstOrDefault();

            if (textItem != null)
                return textItem;
            return string.Empty;
        }
        public List<Product> GetTopCategoryProducts(ProductGroup productGroup)
        {

            List<Product> list = new List<Product>();

            if (productGroup != null)
            {
                List<ProductGroupRelProduct> productGroupRelProducts =
                    db.ProductGroupRelProducts.Where(c => (c.ProductGroupId == productGroup.Id || c.ProductGroup.ParentId == productGroup.Id) && c.IsDeleted == false)
                        .ToList();

                foreach (ProductGroupRelProduct pro in productGroupRelProducts)
                {
                    if (pro.Product.Stock > 0)
                        list.Add(pro.Product);
                }
            }
            return list;
        }
        [Route("payment")]
        public ActionResult payment()
        {
            return View();
        }

        [Route("About")]
        public ActionResult About()
        {
            AboutViewModel about = new AboutViewModel()
            {
                About = db.TextItems.FirstOrDefault(c => c.Name == "abouttext"),
                WhyUsText = db.TextItems.Where(c => c.TextItemType.Name == "whyus").ToList()
            };
            return View(about);
        }
        [Route("Contact")]
        public ActionResult Contact()
        {
            ContactViewModel contact = new ContactViewModel();
            return View(contact);
        }

        [Route("result")]
        public ActionResult Result(string searchQuery, int? pageId)
        {
            List<Product> products = db.Products
                .Where(c => (c.Title.Contains(searchQuery) ||c.Brand.Title.Contains(searchQuery))&& c.IsDeleted == false && c.IsActive).ToList();

            string[] searchArray = searchQuery.Split(' ');
            foreach (string searchItem in searchArray)
            {
                List<Product> segmentProducts = db.Products
                    .Where(c => (c.Title.Contains(searchItem) || c.Brand.Title.Contains(searchItem)) && c.IsDeleted == false && c.IsActive).ToList();


                foreach (Product product in segmentProducts)
                {
                    if (products.All(c => c.Id == product.Id))
                    {
                        products.Add(product);
                    }
                }
            }

            if (pageId == null)
                pageId = 1;

            SearchViewModel search = new SearchViewModel()
            {
                Products = GetProductByPagination(products, pageId),
                PageItems = GetPagination(products.Count(), pageId),
                SearchQuery = searchQuery,
                SidebarBanners = db.SidebarBanners.Where(c => c.IsActive && c.IsDeleted == false).ToList(),
                SidebarProductGroups = GetComplexSidebarProductGroups(),
            };

            return View(search);
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
        private int productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPagination"]);

        public List<Product> GetProductByPagination(List<Product> products, int? pageId)
        {
            List<Product> result = products.OrderBy(c => c.Order).Skip((pageId.Value-1) * productPagination).Take(productPagination)
                .ToList();

            return result;
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
    }
}