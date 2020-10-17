using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel:_BaseViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public List<Product> Products { get; set; }
        public List<SidebarProductGroup> SidebarProductGroups { get; set; }
        public List<SidebarBrand> SidebarBrands { get; set; }
    }

    public class SidebarProductGroup
    {
        public ProductGroup ProductGroup { get; set; }
        public int Quantity { get; set; }
    }
    public class SidebarBrand
    {
        public Brand Brand { get; set; }
        public bool IsSelected { get; set; }
    }
}