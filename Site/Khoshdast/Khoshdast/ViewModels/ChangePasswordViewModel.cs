using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ViewModels;

namespace ViewModels
{
    public class ChangePasswordViewModel:_BaseViewModel
    {
        [Required(ErrorMessage ="وارد کردن کلمه عبور پیشین اجباری است")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "وارد کردن کلمه عبور جدید اجباری است")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "وارد کردن تکرار کلمه عبور جدید اجباری است")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}