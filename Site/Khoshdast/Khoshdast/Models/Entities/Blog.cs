using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Blog : BaseEntity
    {
        public Blog()
        {
            BlogComments=new List<BlogComment>();
        }

        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        [Display(Name="عنوان مطلب")]
        public string Title { get; set; }

        [Display(Name="خلاصه")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string Summery { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        [Display(Name="پارامتر url")]
        public string UrlParam { get; set; }

        [Display(Name="تعداد بازدید")]
        public int Visit { get; set; }

        [Display(Name = "متن")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string Body { get; set; }

        [Display(Name="گروه مطلب")]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public Guid BlogGroupId { get; set; }
        public virtual BlogGroup BlogGroup { get; set; }

        public virtual ICollection<BlogComment> BlogComments { get; set; }
        internal class Configuration : EntityTypeConfiguration<Blog>
        {
            public Configuration()
            {
                HasRequired(p => p.BlogGroup)
                    .WithMany(j => j.Blogs)
                    .HasForeignKey(p => p.BlogGroupId);
            }
        }
    }
}