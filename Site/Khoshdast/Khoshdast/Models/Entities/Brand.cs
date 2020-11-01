using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
   
    public class Brand : BaseEntity
    {
        public Brand()
        {
            Products = new List<Product>();
        }
        [Display(Name = "Title", ResourceType = typeof(Resources.Models.Brand))]
        [StringLength(200, ErrorMessage = "طول {0} نباید بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string Title { get; set; }
         

        [Display(Name = "Name", ResourceType = typeof(Resources.Models.Brand))]
        [StringLength(200, ErrorMessage = "طول {0} نباید بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string UrlParam { get; set; }

        [Display(Name = "Order", ResourceType = typeof(Resources.Models.Brand))]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public int Order { get; set; }

        [Display(Name = "متن صفحه")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Display(Name = "تصویر نام برند")]
        public string BrandNameImageUrl { get; set; }

        [Display(Name = "در صفحه اصلی باشد؟")]
        public bool IsInHome { get; set; }
        public virtual ICollection<Product> Products { get; set; }

         
    }
}