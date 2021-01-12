using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class HomeViewModel:_BaseViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<ProductGroup> HomeProductGroups { get; set; }
        public List<Product> NewestProducts { get; set; }
        public List<Product> BestSaleProducts { get; set; }
        public List<Brand> HomeBrands { get; set; }
        public List<Product> TopCategoryProducts { get; set; }
        public List<Blog> HomeBlogs { get; set; }
        public int SliderQuantity { get { return Sliders.Count(); } }
        public string TopProductGroupTitle { get; set; }

        public TextItem  SliderLeftBanners1 { get; set; }
        public TextItem  SliderLeftBanners2 { get; set; }
        public TextItem HomeMidleBanner1 { get; set; }
        public TextItem HomeMidleBanner2 { get; set; }
        public string HomeMetaDescription { get; set; }

    }
}