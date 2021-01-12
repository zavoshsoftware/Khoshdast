using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class BlogComment:BaseEntity
    {
        [Display(Name = "نام")]
        [StringLength(200, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Email { get; set; }

        [Display(Name = "پیغام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Message { get; set; }

        [Display(Name = "پاسخ")]
        [DataType(DataType.MultilineText)]
        public string Response { get; set; }

        [Display(Name = "سایت")]
        public string Website { get; set; }

        [Display(Name="عنوان مطلب")]
        public Guid BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        internal class Configuration : EntityTypeConfiguration<BlogComment>
        {
            public Configuration()
            {
                HasRequired(p => p.Blog)
                    .WithMany(j => j.BlogComments)
                    .HasForeignKey(p => p.BlogId);
            }
        }
    }
}