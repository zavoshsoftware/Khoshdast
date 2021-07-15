using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ProductInputViewModel
    {
        public List<ProductItemViewModel> Products { get; set; }

    }
    public class ProductItemViewModel
    { 
        public string Id { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string ImageUrl { get; set; }
    }
}