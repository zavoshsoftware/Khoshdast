using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace ViewModels
{
    public class ProductCrudViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }
 
        [Display(Name = "Title", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "PageTitle", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitle { get; set; }

        [Display(Name = "PageDescription", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescription { get; set; }

        [Display(Name = "ImageUrl", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        [DataType(DataType.MultilineText)]
        public string Summery { get; set; }

        [Display(Name = "Body", ResourceType = typeof(Resources.Models.Product))]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }
        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Product))]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "IsInPromotion", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "تازه های صفحه اصلی؟")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInHome { get; set; }
        [Display(Name = "BrandId", ResourceType = typeof(Resources.Models.Product))]
        public Guid BrandId { get; set; }


        [Display(Name = "موجودی")]
        public int Stock { get; set; }

        [Display(Name = "موجودی اولیه")]
        public int SeedStock { get; set; }


        [Display(Name = "بازدید")]
        public int Visit { get; set; }

        [Display(Name = "تعداد فروش")]
        public int SellNumber { get; set; }

        [Display(Name = "پرفروش ترین ها؟")]
        public bool IsTopSale  { get; set; }
        public bool IsAvailable { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources.Models.BaseEntity))]
        public bool IsActive { get; set; }
        [AllowHtml]
        [Display(Name = "یادداشت")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public List<ProductGroupCheckboxList> ProductGroups { get; set; }
    }

    public class ProductGroupCheckboxList
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsSelected { get; set; }
    }
}