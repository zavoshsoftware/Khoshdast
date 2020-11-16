using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListByBrandViewModel : _BaseViewModel
    {
        public Brand brand { get; set; }
        public List<Product> Products { get; set; }
        public List<SidebarProductGroup> SidebarProductGroups { get; set; }
        public List<BreadcrumbItem> BreadcrumbItems { get; set; }
        public List<PageItem> PageItems { get; set; }
        public TextItem SidebarBanner { get; set; }
    }

    
}