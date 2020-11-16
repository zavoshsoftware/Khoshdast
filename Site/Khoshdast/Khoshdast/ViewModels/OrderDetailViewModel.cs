using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}