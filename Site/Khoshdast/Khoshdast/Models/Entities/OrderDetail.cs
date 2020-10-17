using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderDetail : BaseEntity
    {
        [Display(Name = "OrderId", ResourceType = typeof(Resources.Models.OrderDetail))]
        public Guid OrderId { get; set; }

        [Display(Name = "ProductId", ResourceType = typeof(Resources.Models.OrderDetail))]
        public Guid ProductId { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resources.Models.OrderDetail))]
        public int Quantity { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.Models.OrderDetail))]
        [Column(TypeName = "Money")]
        public decimal Price { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.OrderDetail))]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }


        internal class Configuration : EntityTypeConfiguration<OrderDetail>
        {
            public Configuration()
            {
                HasRequired(p => p.Order)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.OrderId);

                HasRequired(p => p.Product)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.ProductId);
            }
        }
    }
}
