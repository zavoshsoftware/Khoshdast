using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class OrderInsertViewModel
    {
        public List<PosInsertViewModel> OrderDetails { get; set; }
        public decimal SubTotal { get; set; }
    }
}