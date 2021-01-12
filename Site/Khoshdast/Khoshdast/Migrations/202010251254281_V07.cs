namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V07 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "BirthYear", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "BirthMonth", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "BirthDay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "BirthDay");
            DropColumn("dbo.Users", "BirthMonth");
            DropColumn("dbo.Users", "BirthYear");
        }
    }
}
