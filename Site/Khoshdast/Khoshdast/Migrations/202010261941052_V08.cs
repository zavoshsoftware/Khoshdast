namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V08 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductGroups", "Code", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductGroups", "Code");
        }
    }
}
