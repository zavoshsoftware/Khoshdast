namespace Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Role: BaseEntity
    {
        public Role()
        {
            Users = new List<User>();
        }


        [Required]
        [StringLength(50)]
        [Display(Name = "RoleTitle", ResourceType = typeof(Resources.Models.Role))]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "RoleName", ResourceType = typeof(Resources.Models.Role))]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
