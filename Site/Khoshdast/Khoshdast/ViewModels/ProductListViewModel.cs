using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel : _BaseViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public List<Product> Products { get; set; }
        //public List<SidebarProductGroup> SidebarProductGroups { get; set; }
        public List<ProductGroup> SidebarProductGroups { get; set; }
        public List<SidebarBrand> SidebarBrands { get; set; }
        public List<BreadcrumbItem> BreadcrumbItems { get; set; }
        public List<PageItem> PageItems { get; set; }
        public List<SidebarBanner> SidebarBanners { get; set; }
    }

    public class SidebarProductGroup
    {
        public ProductGroup ProductGroup { get; set; }
        public List<ProductGroup> ChildProductGroups { get; set; }
        public int Quantity { get; set; }
    }
    public class SidebarBrand
    {
        public Brand Brand { get; set; }
        public bool IsSelected { get; set; }
    }

    public class BreadcrumbItem
    {
        public string Title { get; set; }
        public string UrlParam { get; set; }
        public int Order { get; set; }
    }

    public class PageItem
    {
        public int PageId { get; set; }
        public bool IsCurrentPage { get; set; }
    }
}