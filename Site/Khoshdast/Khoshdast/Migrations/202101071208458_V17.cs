namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V17 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Blogs", "UrlParam", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Blogs", "UrlParam", c => c.String(nullable: false));
        }
    }
}
