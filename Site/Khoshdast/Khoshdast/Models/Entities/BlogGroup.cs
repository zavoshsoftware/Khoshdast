using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class BlogGroup : BaseEntity
    {
        public BlogGroup()
        {
            Blogs = new List<Blog>();
        }
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        [Display(Name="گروه وبلاگ")]
        public string Title { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        [Display(Name="خلاصه گروه وبلاگ")]
        public string Summery { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        [Display(Name="پارامتر Url")]
        public string UrlParam { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }

    }
}