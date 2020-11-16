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
        [Display(Name="عنوان")]
        public string Title { get; set; }

        public string Name { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }

        [Display(Name="متن کوتاه")]
        public string Summery { get; set; }
     

        [Display(Name = "متن")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }


        [Display(Name="آدرس لینک")]
        public string LinkUrl { get; set; }
        [Display(Name="متن لینک")]
        public string LinkTitle { get; set; }

        public Guid? TextItemTypeId { get; set; }
        public virtual  TextItemType  TextItemType { get; set; }
    }
}