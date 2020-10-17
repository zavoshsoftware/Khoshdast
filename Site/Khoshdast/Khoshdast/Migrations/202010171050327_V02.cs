namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductGroups", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductGroups", "ImageUrl");
        }
    }
}
