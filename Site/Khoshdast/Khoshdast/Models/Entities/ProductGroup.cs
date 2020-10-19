using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ProductGroup:BaseEntity
    {
        public ProductGroup()
        {
            ProductGroupRelProducts=new List<ProductGroupRelProduct>();
            ProductGroups=new List<ProductGroup>();
        }
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }


        [Display(Name = "پارامتر url")]
        public string UrlParam { get; set; }

        [Display(Name = "تصویر")]
        public string ImageUrl { get; set; }

        [Display(Name = "Body", ResourceType = typeof(Resources.Models.Product))]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }
 
        public virtual ICollection<ProductGroupRelProduct> ProductGroupRelProducts { get; set; }

        public Guid? ParentId  { get; set; }
        public virtual ProductGroup Parent { get; set; }
        public virtual ICollection<ProductGroup> ProductGroups { get; set; }

        [Display(Name = "گروه های زیر اسلایدر باشد؟")]
        public bool IsInHome { get; set; }

        [Display(Name = "گروه برتر صفحه اصلی باشد؟")]
        public bool IsHomeTopGroup { get; set; }
        internal class Configuration : EntityTypeConfiguration<ProductGroup>
        {
            public Configuration()
            {
                HasOptional(p => p.Parent).WithMany(j => j.ProductGroups).HasForeignKey(p => p.ParentId);
            }
        }
    }
}