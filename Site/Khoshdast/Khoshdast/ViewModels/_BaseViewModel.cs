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

        public List<MenuItems> MenuItems { get {return baseviewmodel.GetMenuProductGroup(); } }
    }

    public class MenuItems
    {
        public ProductGroup ParentProductGroup { get; set; }
        public List<ProductGroup> ChildProductGroups { get; set; }
        public int ChildCount { get { return ChildProductGroups.Count(); } }
    }
}