using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helpers;
using Models;

namespace ViewModels
{

    public class _BaseViewModel
    {
        private BaseViewModelHelper baseviewmodel = new BaseViewModelHelper();

        public List<MenuItems> MenuItems { get { return baseviewmodel.GetMenuProductGroup(); } }
        public string Address { get { return baseviewmodel.GetTextItemByName("address", "summery"); } }
        public string Phone { get { return baseviewmodel.GetTextItemByName("phone", "summery"); } }
        public string Email { get { return baseviewmodel.GetTextItemByName("email", "summery"); } }
        public string Instagram { get { return baseviewmodel.GetTextItemByName("Instagram", "linkUrl"); } }
        public string Telegram { get { return baseviewmodel.GetTextItemByName("telegram", "linkUrl"); } }
        public string Aparat { get { return baseviewmodel.GetTextItemByName("aparat", "linkUrl"); } }
        public string Youtube { get { return baseviewmodel.GetTextItemByName("youtube", "linkUrl"); } }
    }

    public class MenuItems
    {
        public ProductGroup ParentProductGroup { get; set; }
        public List<ChildMenuItem> ChildProductGroups { get; set; }
        public List<ProductGroup> SimpleProductGroups { get; set; }
        public int ChildCount { get { return ChildProductGroups.Count(); } }

        public List<ProductGroup> NonChildProductGroups { get; set; }

    }

    public class ChildMenuItem
    {
        public ProductGroup ParentProductGroup { get; set; }
        public List<ProductGroup> ChildProductGroups { get; set; }
        public bool IsLastItem { get; set; }

    }
}