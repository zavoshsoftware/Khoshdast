using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProduct { get; set; }
        public int CurrentOrderCount { get; set; }
        public int TotalOrderCount { get; set; }
        public int TotalOrderCountThisMount { get; set; }
        public decimal TotalOrderAmountThisMount { get; set; }
        public int TotalBlog { get; set; }
        public List<RecentOrder> RecentOrders { get; set; }
    }

    public class RecentOrder
    {
        public DateTime CreationDate { get; set; }
        public int TotalCount { get; set; }
        public string TotalAmount { get; set; }
    }
}