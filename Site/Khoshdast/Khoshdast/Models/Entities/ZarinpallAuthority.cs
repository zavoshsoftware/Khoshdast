using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class ZarinpallAuthority:BaseEntity
    {
        public string Authority { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal Amount { get; set; }
        internal class Configuration : EntityTypeConfiguration<ZarinpallAuthority>
        {
            public Configuration()
            {
                HasRequired(p => p.Order)
                    .WithMany(j => j.ZarinpallAuthorities)
                    .HasForeignKey(p => p.OrderId);
            }
        }
    }
}