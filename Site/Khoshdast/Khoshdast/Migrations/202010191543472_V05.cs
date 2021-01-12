namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsTopSell", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductGroups", "IsInHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductGroups", "IsHomeTopGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductGroups", "IsHomeTopGroup");
            DropColumn("dbo.ProductGroups", "IsInHome");
            DropColumn("dbo.Products", "IsTopSell");
        }
    }
}
