using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;

namespace Khoshdast.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SiteMapGeneratorController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        [Route("seo/sitemap")]
        public ActionResult Sitemap(string language)
        {
            Sitemap sm = new Sitemap();

            StaticPageSiteMap(sm);
            ProductGroupsSiteMap(sm);

            ProductsSiteMap(sm);
            BlogDetailSiteMap(sm);
            return new XmlResult(sm);
        }



        public void ProductsSiteMap(Sitemap sm)
        {
            List<Product> products = db.Products.Where(current => current.IsDeleted == false && current.IsActive).ToList();

            foreach (Product product in products)
            {
                var encoded = HttpUtility.UrlPathEncode("https://khoshdast.ir/product/" + product.Code);
                AddToSiteMap(sm, encoded, 0.7D, Location.eChangeFrequency.monthly);
            }
        }
        public void ProductGroupsSiteMap(Sitemap sm)
        {
            List<Models.ProductGroup> productGroups = db.ProductGroups.Where(current => current.IsDeleted == false && current.IsActive).ToList();

            foreach (ProductGroup productGroup in productGroups)
            {
                var encoded = HttpUtility.UrlPathEncode("https://khoshdast.ir/category/" + productGroup.UrlParam);
                AddToSiteMap(sm, encoded, 0.9D, Location.eChangeFrequency.daily);
            }
        }
        public void BlogDetailSiteMap(Sitemap sm)
        {
            List<Models.Blog> blogs = db.Blogs.Where(current => current.IsDeleted == false &&   current.IsActive ).ToList();

            foreach (Blog blog in blogs)
            {
                var encoded = HttpUtility.UrlPathEncode("https://khoshdast.ir/blog/post/" + blog.UrlParam);
                AddToSiteMap(sm, encoded, 0.6D, Location.eChangeFrequency.monthly);
            }
        }
        public void StaticPageSiteMap(Sitemap sm)
        {
            AddToSiteMap(sm, "https://khoshdast.ir/", 0.9D, Location.eChangeFrequency.weekly);

            AddToSiteMap(sm, "https://khoshdast.ir/about", 0.5D, Location.eChangeFrequency.yearly);

            AddToSiteMap(sm, "https://khoshdast.ir/contact", 0.5D, Location.eChangeFrequency.yearly);
        }

        public void AddToSiteMap(Sitemap sm, string url, double? priority, Location.eChangeFrequency frequency)
        {
            sm.Add(new Location()
            {
                Url = url,
                LastModified = DateTime.UtcNow,
                Priority = priority,
                ChangeFrequency = frequency
            });
        }
    }
}