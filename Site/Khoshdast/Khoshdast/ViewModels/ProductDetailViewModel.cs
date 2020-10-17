using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductDetailViewModel : _BaseViewModel
    {
        public Product Product { get; set; }
        public List<ProductComment> ProductComments { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public ProductGroup ProductGroup { get; set; }

        public int CommentQty
        {
            get { return ProductComments.Count(); }
        }
    }
 
}