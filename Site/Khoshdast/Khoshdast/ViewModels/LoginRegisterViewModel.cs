using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class LoginRegisterViewModel:_BaseViewModel
    {
        public string LoginCellNumber { get; set; }
        public string LoginPassword { get; set; }

        public string RegisterCellNumber { get; set; }
        public string RegisterPassword { get; set; }
        public string RegisterFullName { get; set; }
        public string RegisterEmail { get; set; }

        public string ReturnUrl { get; set; }
    }
}