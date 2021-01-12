namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogComments", "Response", c => c.String());
            DropColumn("dbo.BlogGroups", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BlogGroups", "ImageUrl", c => c.String());
            DropColumn("dbo.BlogComments", "Response");
        }
    }
}
