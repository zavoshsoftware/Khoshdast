using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductGroupRelProduct:BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }

        internal class configuration : EntityTypeConfiguration<ProductGroupRelProduct>
        {
            public configuration()
            {
                HasRequired(p => p.ProductGroup).WithMany(t => t.ProductGroupRelProducts).HasForeignKey(p => p.ProductGroupId);
                HasRequired(p => p.Product).WithMany(t => t.ProductGroupRelProducts).HasForeignKey(p => p.ProductId);
            }
        }
    }
}