using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderStatus : BaseEntity
    {
        public OrderStatus()
        {
            Orders = new List<Order>();
        }

        [Display(Name = "Title", ResourceType = typeof(Resources.Models.OrderStatus))]
        [StringLength(30)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Resources.Models.OrderStatus))]
        [Required]
        public int Code { get; set; }

      
        public virtual ICollection<Order> Orders { get; set; }

    }
}
