namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerDesc", c => c.String());
            AddColumn("dbo.Orders", "PaymentDesc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaymentDesc");
            DropColumn("dbo.Orders", "CustomerDesc");
        }
    }
}
