namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V09 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Brands", "Body", c => c.String(storeType: "ntext"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Brands", "Body", c => c.String(nullable: false, storeType: "ntext"));
        }
    }
}
