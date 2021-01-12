namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "Tags", c => c.String());
            AddColumn("dbo.Products", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Tags");
            DropColumn("dbo.Blogs", "Tags");
        }
    }
}
