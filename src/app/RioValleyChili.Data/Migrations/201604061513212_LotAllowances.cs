namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotAllowances : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotContractAllowances",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ContractYear = c.Int(nullable: false),
                        ContractSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.ContractYear, t.ContractSequence })
                .ForeignKey("dbo.Contracts", t => new { t.ContractYear, t.ContractSequence })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.ContractYear, t.ContractSequence });
            
            CreateTable(
                "dbo.LotCustomerAllowances",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.CustomerId })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.LotCustomerOrderAllowances",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        CustomerOrderDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        CustomerOrderSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.CustomerOrderDateCreated, t.CustomerOrderSequence })
                .ForeignKey("dbo.CustomerOrders", t => new { t.CustomerOrderDateCreated, t.CustomerOrderSequence })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.CustomerOrderDateCreated, t.CustomerOrderSequence });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotContractAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotContractAllowances", new[] { "ContractYear", "ContractSequence" }, "dbo.Contracts");
            DropForeignKey("dbo.LotCustomerOrderAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotCustomerOrderAllowances", new[] { "CustomerOrderDateCreated", "CustomerOrderSequence" }, "dbo.CustomerOrders");
            DropForeignKey("dbo.LotCustomerAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotCustomerAllowances", "CustomerId", "dbo.Customers");
            DropIndex("dbo.LotCustomerOrderAllowances", new[] { "CustomerOrderDateCreated", "CustomerOrderSequence" });
            DropIndex("dbo.LotCustomerOrderAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotCustomerAllowances", new[] { "CustomerId" });
            DropIndex("dbo.LotCustomerAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotContractAllowances", new[] { "ContractYear", "ContractSequence" });
            DropIndex("dbo.LotContractAllowances", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropTable("dbo.LotCustomerOrderAllowances");
            DropTable("dbo.LotCustomerAllowances");
            DropTable("dbo.LotContractAllowances");
        }
    }
}
