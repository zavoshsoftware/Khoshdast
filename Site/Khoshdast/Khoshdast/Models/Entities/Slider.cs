using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Slider:BaseEntity
    {
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.Slider))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "ImageUrl", ResourceType = typeof(Resources.Models.Slider))]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name ="عنوان")]
        public string Title { get; set; }

        [Display(Name ="توضیحات")]
        public string Summery { get; set; }

        [Display(Name ="متن دکمه")]
        public string LinkTitle { get; set; }

        [Display(Name ="صفحه هدف دکمه")]
        public string LandingPage { get; set; }
    }
}