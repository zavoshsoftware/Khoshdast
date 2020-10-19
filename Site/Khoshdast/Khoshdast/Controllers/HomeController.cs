using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                topPgTitle ="جدیدترین مدل های "+ topProductGroup.Title;

            HomeViewModel home =new HomeViewModel()
            {
                Sliders = db.Sliders.Where(c=>c.IsDeleted==false&&c.IsActive).OrderBy(c=>c.Order).ToList(),
                HomeProductGroups = db.ProductGroups.Where(c =>c.IsInHome && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                BestSaleProducts = db.Products.Where(c => c.IsTopSell && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                NewestProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                TopCategoryProducts = GetTopCategoryProducts(topProductGroup),
                TopProductGroupTitle = topPgTitle,
                HomeBlogs = db.Blogs.Where(c=>c.IsDeleted==false&&c.IsActive).OrderByDescending(c=>c.CreationDate).Take(3).ToList(),
                HomeBrands = db.Brands.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.Order).ToList(),
            };
            return View(home);
        }

        public List<Product> GetTopCategoryProducts(ProductGroup productGroup)
        {
          
            List<Product> list=new List<Product>();

            if (productGroup != null)
            {
                List<ProductGroupRelProduct> productGroupRelProducts =
                    db.ProductGroupRelProducts.Where(c => (c.ProductGroupId == productGroup.Id||c.ProductGroup.ParentId==productGroup.Id) && c.IsDeleted == false)
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
            AboutViewModel about=new AboutViewModel();
            return View(about);
        }
        [Route("Contact")]
        public ActionResult Contact()
        {
            ContactViewModel contact=new ContactViewModel();
            return View(contact);
        }
    }
}