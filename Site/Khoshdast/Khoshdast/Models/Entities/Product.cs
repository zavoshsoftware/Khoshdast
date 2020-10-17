using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
    public class Product:BaseEntity
    {
        public Product()
        {
            OrderDetails=new List<OrderDetail>();
            ProductGroupRelProducts=new List<ProductGroupRelProduct>();
            ProductComments = new List<ProductComment>();
        }
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(15, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Code { get; set; }

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
           
        [Display(Name = "Summery", ResourceType = typeof(Resources.Models.Product))]
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

        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        public string AmountStr
        {
            get { return Amount.ToString("n0")+" تومان"; }
        }

        [NotMapped]
        public string DiscountAmountStr
        {
            get
            {
                if(DiscountAmount!=null)
                return DiscountAmount.Value.ToString("n0")+" تومان";

                return string.Empty;
            }
        }

        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Product))]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "IsInPromotion", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "IsInHome", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInHome { get; set; }

        public virtual ICollection<ProductGroupRelProduct> ProductGroupRelProducts { get; set; }


        [Display(Name = "BrandId", ResourceType = typeof(Resources.Models.Product))]
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
         

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } 
 
   

        [Display(Name = "موجودی")]
        public int Stock { get; set; }

        [Display(Name = "موجودی اولیه")]
        public int SeedStock { get; set; }


        [Display(Name = "بازدید")]
        public int Visit { get; set; }

        [Display(Name = "تعداد فروش")]
        public int SellNumber { get; set; }

  
         
        public virtual ICollection<ProductComment> ProductComments { get; set; }
 
        public bool IsAvailable { get; set; }
        internal class configuration : EntityTypeConfiguration<Product>
        {
            public configuration()
            {
                 HasRequired(p => p.Brand).WithMany(t => t.Products).HasForeignKey(p => p.BrandId);
            }
        }
    }
}