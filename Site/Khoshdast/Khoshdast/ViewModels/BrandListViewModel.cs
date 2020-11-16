using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BrandListViewModel:_BaseViewModel
    {
        public List<Brand> Brands { get; set; }
    }
}