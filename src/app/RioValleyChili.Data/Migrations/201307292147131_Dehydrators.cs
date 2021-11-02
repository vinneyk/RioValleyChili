namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Dehydrators : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DehydratedMaterialsReceived", "SupplierId", "dbo.Companies");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", "dbo.Companies");
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "SupplierId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "DehydratorId" });
            AddColumn("dbo.DehydratedMaterialsReceived", "DehydratorId", c => c.Int(nullable: false));
            AddForeignKey("dbo.DehydratedMaterialsReceived", "DehydratorId", "dbo.Companies", "Id");
            CreateIndex("dbo.DehydratedMaterialsReceived", "DehydratorId");
            DropColumn("dbo.DehydratedMaterialsReceived", "SupplierId");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "DehydratorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", c => c.Int(nullable: false));
            AddColumn("dbo.DehydratedMaterialsReceived", "SupplierId", c => c.Int(nullable: false));
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "DehydratorId" });
            DropForeignKey("dbo.DehydratedMaterialsReceived", "DehydratorId", "dbo.Companies");
            DropColumn("dbo.DehydratedMaterialsReceived", "DehydratorId");
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "DehydratorId");
            CreateIndex("dbo.DehydratedMaterialsReceived", "SupplierId");
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", "dbo.Companies", "Id");
            AddForeignKey("dbo.DehydratedMaterialsReceived", "SupplierId", "dbo.Companies", "Id");
        }
    }
}
