namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductGroupDiscounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductGroupId = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPercentage = c.Boolean(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId, cascadeDelete: true)
                .Index(t => t.ProductGroupId);
            
            AddColumn("dbo.Products", "ProductGroupDiscountId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductGroupDiscounts", "ProductGroupId", "dbo.ProductGroups");
            DropIndex("dbo.ProductGroupDiscounts", new[] { "ProductGroupId" });
            DropColumn("dbo.Products", "ProductGroupDiscountId");
            DropTable("dbo.ProductGroupDiscounts");
        }
    }
}
