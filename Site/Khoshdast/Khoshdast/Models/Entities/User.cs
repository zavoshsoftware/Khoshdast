using Resources;

namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.Linq;

    public class User : BaseEntity
    {
        public User()
        {
            Orders = new List<Order>();
        }


        [Display(Name = "Password", ResourceType = typeof(Resources.Models.User))]
        [StringLength(150, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Password { get; set; }

        [Display(Name = "CellNum", ResourceType = typeof(Resources.Models.User))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(20, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [RegularExpression(@"(^(09|9)[0123456789][0123456789]\d{7}$)|(^(09|9)[0123456789][0123456789]\d{7}$)", ErrorMessageResourceName = "MobilExpersionValidation", ErrorMessageResourceType = typeof(Messages))]
        public string CellNum { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Resources.Models.User))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(250, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string FullName { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Resources.Models.User))]
        public int? Code { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resources.Models.User))]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceName = "EmailExpersionValidation", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }

        [Display(Name = "GenderTitle", ResourceType = typeof(Resources.Models.Gender))]
        public Guid? GenderId { get; set; }

        public Guid RoleId { get; set; }

        [Display(Name = "RemainCredit", ResourceType = typeof(Resources.Models.User))]
        public decimal? RemainCredit { get; set; }

        public int BirthYear { get; set; }
        public int BirthMonth { get; set; }
        public int BirthDay { get; set; }
     
        public virtual Gender Gender { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }


    }
}

