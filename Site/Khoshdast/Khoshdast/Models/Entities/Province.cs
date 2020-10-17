
namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    public class Province : BaseEntity
    {
        public Province()
        {
            Cities = new List<City>();
        }

        [StringLength(100)]
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Title { get; set; }

        public virtual ICollection<City> Cities { get; set; }

      
    }
}
