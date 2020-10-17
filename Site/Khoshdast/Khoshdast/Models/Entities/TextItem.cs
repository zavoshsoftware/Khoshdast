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
    public class TextItem : BaseEntity
    {
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string Title { get; set; }
       

        public string ImageUrl { get; set; }


        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string UrlParam { get; set; }
     

        [Display(Name = "متن")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        [Required(ErrorMessage = "فیلد {0} اجباری می باشد.")]
        public string Body { get; set; }

        public string Name { get; set; }
    }
}