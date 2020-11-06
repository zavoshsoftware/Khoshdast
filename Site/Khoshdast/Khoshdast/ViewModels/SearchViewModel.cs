using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class SearchViewModel : _BaseViewModel
    {
        public List<Product> Products { get; set; }
        public List<PageItem> PageItems { get; set; }
        public string SearchQuery { get; set; }
    }
}