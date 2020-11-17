using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class CustomerOrderListViewModel : _BaseViewModel
    {
        public List<Order> Orders { get; set; }
    }
}