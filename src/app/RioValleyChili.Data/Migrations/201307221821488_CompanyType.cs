namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CompanyType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "BrokerId", "dbo.Brokers");
            DropForeignKey("dbo.Brokers", "Id", "dbo.Companies");
            DropForeignKey("dbo.Contracts", "BrokerId", "dbo.Brokers");
            DropForeignKey("dbo.CustomerOrders", "BrokerId", "dbo.Brokers");
            DropIndex("dbo.Customers", new[] { "BrokerId" });
            DropIndex("dbo.Brokers", new[] { "Id" });
            DropIndex("dbo.Contracts", new[] { "BrokerId" });
            DropIndex("dbo.CustomerOrders", new[] { "BrokerId" });
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Customers", "BrokerId", "dbo.Companies", "Id");
            AddForeignKey("dbo.Contracts", "BrokerId", "dbo.Companies", "Id");
            AddForeignKey("dbo.CustomerOrders", "BrokerId", "dbo.Companies", "Id");
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", "dbo.Companies", "Id");
            CreateIndex("dbo.Customers", "BrokerId");
            CreateIndex("dbo.Contracts", "BrokerId");
            CreateIndex("dbo.CustomerOrders", "BrokerId");
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "DehydratorId");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "LocalityGrown");
            DropTable("dbo.Brokers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Brokers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "LocalityGrown", c => c.String(maxLength: 10));
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "DehydratorId" });
            DropIndex("dbo.CustomerOrders", new[] { "BrokerId" });
            DropIndex("dbo.Contracts", new[] { "BrokerId" });
            DropIndex("dbo.Customers", new[] { "BrokerId" });
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "DehydratorId", "dbo.Companies");
            DropForeignKey("dbo.CustomerOrders", "BrokerId", "dbo.Companies");
            DropForeignKey("dbo.Contracts", "BrokerId", "dbo.Companies");
            DropForeignKey("dbo.Customers", "BrokerId", "dbo.Companies");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "DehydratorId");
            CreateIndex("dbo.CustomerOrders", "BrokerId");
            CreateIndex("dbo.Contracts", "BrokerId");
            CreateIndex("dbo.Brokers", "Id");
            CreateIndex("dbo.Customers", "BrokerId");
            AddForeignKey("dbo.CustomerOrders", "BrokerId", "dbo.Brokers", "Id");
            AddForeignKey("dbo.Contracts", "BrokerId", "dbo.Brokers", "Id");
            AddForeignKey("dbo.Brokers", "Id", "dbo.Companies", "Id");
            AddForeignKey("dbo.Customers", "BrokerId", "dbo.Brokers", "Id");
        }
    }
}
