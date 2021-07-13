using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class AdminProductListViewModel
    {
        public Guid Id { get; set; }
        [Display(Name="بارکد")]
        public string BarCode { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "گروه محصول")]
        public string ProductGroupTitle { get; set; }
        [Display(Name = "قیمت")]
        public string Amount { get; set; }
        [Display(Name = "موجودی")]
        public string Stock { get; set; }
        [Display(Name = "کالای تخفیف دار؟")]
        public bool IsInPromotion { get; set; }
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }
        [Display(Name = "برند")]
        public string BrandTitle { get; set; }
        [Display(Name = "تصویر")]
        public string ImageUrl { get; set; }

        [Display(Name = "تازه های صفحه اصلی")]
        public bool IsNewest { get; set; }
    }
}