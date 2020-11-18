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
                BestSaleProducts = db.Products.Where(c => c.IsTopSell && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                NewestProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                TopCategoryProducts = GetTopCategoryProducts(topProductGroup),
                TopProductGroupTitle = topPgTitle,
                HomeBlogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList(),
                HomeBrands = db.Brands.Where(c =>c.BrandNameImageUrl!=null&& c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.Order).Take(10).ToList(),
                SliderLeftBanners1 = sliderBanners.FirstOrDefault(),
                SliderLeftBanners2 = sliderBanners.LastOrDefault(),
                HomeMidleBanner1 = homeMidkeBanners.FirstOrDefault(),
                HomeMidleBanner2 = homeMidkeBanners.LastOrDefault(),
            };
            return View(home);
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
                About = db.TextItems.FirstOrDefault(c=>c.Name== "abouttext"),
                WhyUsText = db.TextItems.Where(c=>c.TextItemType.Name== "whyus").ToList()
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
                .Where(c => c.Title.Contains(searchQuery) && c.IsDeleted == false && c.IsActive).ToList();

            if (pageId == null)
                pageId = 1;

            SearchViewModel search = new SearchViewModel()
            {
                Products = GetProductByPagination(products, pageId),
                PageItems = GetPagination(products.Count(), pageId),
                SearchQuery = searchQuery
            };

            return View(search);
        }

        private int productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPagination"]);

        public List<Product> GetProductByPagination(List<Product> products, int? pageId)
        {
            List<Product> result = products.OrderBy(c => c.Order).Skip(pageId.Value * productPagination).Take(productPagination)
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