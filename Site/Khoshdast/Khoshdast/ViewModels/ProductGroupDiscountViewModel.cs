using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ProductGroupDiscountViewModel
    {
        [Display(Name="مقدار تخفیف")]
        public decimal Amount { get; set; }

        [Display(Name="درصدی است؟")]
        public bool IsPercent { get; set; }
    }
}