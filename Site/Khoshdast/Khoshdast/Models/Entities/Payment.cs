using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class Payment : BaseEntity
    {
        [Display(Name = "مبلغ پرداختی")]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [Display(Name = "نوع پرداخت")]
        public Guid PaymentTypeId { get; set; }

        [Display(Name = "بیعانه")]
        public bool IsDeposit { get; set; }

        [Display(Name = "شناسه پرداخت")]
        public string Code { get; set; }

        [Display(Name = "پیوست")]
        public string FileAttched { get; set; }
        /***********************( new fild )***************************/
        [Display(Name = "تاریخ پرداخت")]
        [UIHint("PersianDatePicker")]
        public DateTime PaymentDay { get; set; }
        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; }

        public virtual PaymentType PaymentType { get; set; }

        internal class configuration : EntityTypeConfiguration<Payment>
        {
            public configuration()
            {
                HasRequired(p => p.PaymentType).WithMany(t => t.Payments).HasForeignKey(p => p.PaymentTypeId);
            }
        }


        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Models.Payment))]
        public string AmountStr
        {
            get { return Amount.ToString("N0"); }
        }
    }
}