using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Models
{
    public class ProductComment : BaseEntity
    {
        [Display(Name = "نام")]
        [StringLength(200, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Email { get; set; }

        [Display(Name = "پیغام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        [Display(Name = "پاسخ")]
        [DataType(DataType.MultilineText)]
        public string Response { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        internal class Configuration : EntityTypeConfiguration<ProductComment>
        {
            public Configuration()
            {
                HasRequired(p => p.Product)
                    .WithMany(j => j.ProductComments)
                    .HasForeignKey(p => p.ProductId);
            }
        }
    }
}