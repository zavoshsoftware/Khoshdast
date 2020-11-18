using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Mvc.Html;

namespace Models
{

    public class Order : BaseEntity
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            ZarinpallAuthorities = new List<ZarinpallAuthority>();
        }

        [Display(Name = "Code", ResourceType = typeof(Resources.Models.Order))]
        [Required]
        public int Code { get; set; }

        [Display(Name = "UserId", ResourceType = typeof(Resources.Models.Order))]
        public Guid? UserId { get; set; }


        [Display(Name = "Address", ResourceType = typeof(Resources.Models.Order))]
        public string Address { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        [Column(TypeName = "Money")]
        public decimal TotalAmount { get; set; }

        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        public string TotalAmountStr
        {
            get { return TotalAmount.ToString("n0") + " تومان"; } 
        }


        [Display(Name = "OrderStatusId", ResourceType = typeof(Resources.Models.Order))]
        [Required]
        public Guid OrderStatusId { get; set; }

        public Guid? CityId { get; set; }

        [Display(Name = "SaleReferenceId", ResourceType = typeof(Resources.Models.Order))]
        public string SaleReferenceId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual City City { get; set; }

        [Display(Name = "IsPaid", ResourceType = typeof(Resources.Models.Order))]
        public bool IsPaid { get; set; }


        public Guid? DiscountCodeId { get; set; }

        public virtual DiscountCode DiscountCode { get; set; }
        [Display(Name = "هزینه حمل")]
        public decimal? ShippingAmount { get; set; }

        [Display(Name = "هزینه جمع فاکتور")]
        public decimal? SubTotal { get; set; }

        [NotMapped]
        [Display(Name = "هزینه جمع فاکتور")]
        public string SubTotalStr
        {
            get
            {
                if (SubTotal != null)
                    return SubTotal.Value.ToString("n0") + " تومان";

                return string.Empty;
            }
        }

        [Display(Name = "مبلغ تخفیف")]
        public decimal? DiscountAmount { get; set; }


        [NotMapped]
        [Display(Name = "مبلغ تخفیف")]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount != null)
                    return DiscountAmount.Value.ToString("n0") + " تومان";

                return string.Empty;
            }
        }

        public virtual List<ZarinpallAuthority> ZarinpallAuthorities { get; set; }
        internal class Configuration : EntityTypeConfiguration<Order>
        {
            public Configuration()
            {
                HasOptional(p => p.User)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.UserId);

                HasRequired(p => p.OrderStatus)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.OrderStatusId);

                HasOptional(p => p.City)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.CityId);

                HasRequired(p => p.DiscountCode)
                    .WithMany(j => j.Orders)
                    .HasForeignKey(p => p.DiscountCodeId);
            }
        }

        [Display(Name="نام تحویل گیرنده")]
        public string DeliverFullName { get; set; }
        [Display(Name="شماره موبایل تحویل گیرنده")]
        public string DeliverCellNumber { get; set; }
        [Display(Name="کد پستی")]
        public string PostalCode { get; set; }
        [Display(Name="تاریخ پرداخت")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name="نحوه پرداخت")]
        public string PaymentTypeTitle { get; set; }


    }
}
