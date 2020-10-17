using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class DiscountHelper
    {
        public decimal CalculateDiscountAmount(Models.DiscountCode discountCode, decimal totalAmount)
        {
            if (discountCode.IsPercent)
                return totalAmount * discountCode.Amount / 100;
            else
                return discountCode.Amount;
        }
    }
}