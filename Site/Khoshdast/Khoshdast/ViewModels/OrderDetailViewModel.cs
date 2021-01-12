using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public string PaymentTypeTitle
        {
            get
            {
                if (Order.PaymentTypeTitle == "online")
                    return "پرداخت آنلاین";

                if (Order.PaymentTypeTitle == "recieve")
                    return "پرداخت در محل";

                if (Order.PaymentTypeTitle == "transfer")
                    return "کارت به کارت";

                return "پرداخت آنلاین";
            }
        }


    }

   
}