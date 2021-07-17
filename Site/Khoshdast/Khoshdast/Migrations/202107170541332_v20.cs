namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v20 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "CustomerTypeId", c => c.Guid());
            CreateIndex("dbo.Orders", "CustomerTypeId");
            AddForeignKey("dbo.Orders", "CustomerTypeId", "dbo.CustomerTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CustomerTypeId", "dbo.CustomerTypes");
            DropIndex("dbo.Orders", new[] { "CustomerTypeId" });
            DropColumn("dbo.Orders", "CustomerTypeId");
            DropTable("dbo.CustomerTypes");
        }
    }
}
