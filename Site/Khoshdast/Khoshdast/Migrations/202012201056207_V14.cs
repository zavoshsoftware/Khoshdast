namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V14 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Code", c => c.String(maxLength: 15));
        }
    }
}
