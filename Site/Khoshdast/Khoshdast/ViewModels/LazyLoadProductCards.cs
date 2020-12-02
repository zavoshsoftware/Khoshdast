  using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class LazyLoadProductCardsViewModel
    {
        public List<LazyLoadProductCardsItemViewModel> Result { get; set; }
        public string IsLastBatch { get; set; }
    }

    public class LazyLoadProductCardsItemViewModel
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string Code { get; set; }
        public int Stock { get; set; }
    }
}