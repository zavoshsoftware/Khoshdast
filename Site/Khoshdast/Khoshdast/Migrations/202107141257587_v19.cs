namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AdditiveAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "DecreaseAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "IsPos", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "PaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "RemainAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "RemainAmount");
            DropColumn("dbo.Orders", "PaymentAmount");
            DropColumn("dbo.Orders", "IsPos");
            DropColumn("dbo.Orders", "DecreaseAmount");
            DropColumn("dbo.Orders", "AdditiveAmount");
        }
    }
}
