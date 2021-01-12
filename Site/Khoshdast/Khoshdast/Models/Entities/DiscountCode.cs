using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class DiscountCode:BaseEntity
    {
        public DiscountCode()
        {
            Orders=new List<Order>();
        }
        [Display(Name = "Code", ResourceType = typeof(Resources.Models.DiscountCode))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(10, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Code { get; set; }
        [Display(Name = "ExpireDate", ResourceType = typeof(Resources.Models.DiscountCode))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "IsPercent", ResourceType = typeof(Resources.Models.DiscountCode))]
        public bool IsPercent { get; set; }
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.DiscountCode))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")] 
        public decimal Amount { get; set; }

        [Display(Name = "IsMultiUsing", ResourceType = typeof(Resources.Models.DiscountCode))]
        public bool IsMultiUsing { get; set; }


        public virtual ICollection<Order> Orders { get; set; }
    }
}
