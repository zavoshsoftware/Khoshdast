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
    public class ProductGroupDiscount : BaseEntity
    { 

        [Display(Name="گروه محصول")]
        public Guid ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        [Display(Name="مقدار تخفیف")]
        public decimal Amount { get; set; }
        [Display(Name="درصدی است؟")]
        public bool IsPercentage { get; set; }
        [Display(Name="تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }

        internal class Configuration : EntityTypeConfiguration<ProductGroupDiscount>
        {
            public Configuration()
            {
                HasOptional(p => p.ProductGroup).WithMany(j => j.ProductGroupDiscounts).HasForeignKey(p => p.ProductGroupId);
            }
        }
    }
}