using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class SidebarBanner:BaseEntity
    {
        [Display(Name="اولویت نمایش")]
        public int Order { get; set; }
        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }
        [Display(Name="آدرس لینک")]
        public string LinkUrl { get; set; }
        [Display(Name="متن جایگزین تصویر")]
        public string ImageAlt { get; set; }
    }
}