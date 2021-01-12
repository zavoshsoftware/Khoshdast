namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TextItemTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TextItems", "Summery", c => c.String());
            AddColumn("dbo.TextItems", "LinkUrl", c => c.String());
            AddColumn("dbo.TextItems", "LinkTitle", c => c.String());
            AddColumn("dbo.TextItems", "TextItemTypeId", c => c.Guid());
            AlterColumn("dbo.TextItems", "Title", c => c.String());
            AlterColumn("dbo.TextItems", "Body", c => c.String(storeType: "ntext"));
            CreateIndex("dbo.TextItems", "TextItemTypeId");
            AddForeignKey("dbo.TextItems", "TextItemTypeId", "dbo.TextItemTypes", "Id");
            DropColumn("dbo.TextItems", "UrlParam");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TextItems", "UrlParam", c => c.String(nullable: false));
            DropForeignKey("dbo.TextItems", "TextItemTypeId", "dbo.TextItemTypes");
            DropIndex("dbo.TextItems", new[] { "TextItemTypeId" });
            AlterColumn("dbo.TextItems", "Body", c => c.String(nullable: false, storeType: "ntext"));
            AlterColumn("dbo.TextItems", "Title", c => c.String(nullable: false));
            DropColumn("dbo.TextItems", "TextItemTypeId");
            DropColumn("dbo.TextItems", "LinkTitle");
            DropColumn("dbo.TextItems", "LinkUrl");
            DropColumn("dbo.TextItems", "Summery");
            DropTable("dbo.TextItemTypes");
        }
    }
}
