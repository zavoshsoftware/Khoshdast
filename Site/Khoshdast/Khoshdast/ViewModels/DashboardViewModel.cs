using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class DashboardViewModel:_BaseViewModel
    {
        [Display(Name ="نام و نام خانوادگی")]
        public string FullName { get; set; }
        [Display(Name ="شماره موبایل")]
        public string UserCellNumber { get; set; }
        [Display(Name ="ایمیل")]
        public string UserEmail { get; set; }
        [Display(Name ="جنسیت")]
        public string GenderTitle { get; set; }
        [Display(Name = "تاریخ ثبت نام")]
        public string CreationDateStr { get; set; }

        [Display(Name ="تعداد سفارشات تکمیل شده")]
        public int TotalCompleteOrderCount { get; set; }
        [Display(Name ="تعداد سفارشات ناقص")]
        public int TotalNotCompleteOrderCount { get; set; }

        public Guid Id { get; set; }
    }
}