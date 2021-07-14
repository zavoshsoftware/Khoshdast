using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class PaymentType :BaseEntity
    {
        public PaymentType()
        {
            Payments = new List<Payment>();
        }
        [Display(Name = "نوع پرداخت")]
        public string Title { get; set; }
        public int Order { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}