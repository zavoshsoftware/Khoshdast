using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ViewModels;

namespace ViewModels
{
    public class ForgetPasswordViewModel : _BaseViewModel
    {
        [Required(ErrorMessage ="وارد نمودن شماره تلفن همراه اجباری است")]
        public string UserCellNumber { get; set; }
    }
}