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
        public int TotalOrderAmountThisMount { get; set; }
        public int TotalBlog { get; set; }
    }
}