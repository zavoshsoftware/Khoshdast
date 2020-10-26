namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V06 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "SaleReferenceId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "SaleReferenceId", c => c.Long());
        }
    }
}
