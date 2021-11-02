namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InventoryTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryTransactions",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        Description = c.String(),
                        SourceReference = c.String(),
                        PackagingProductId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        ToteKey = c.String(maxLength: 15),
                        Quantity = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.LocationId)
                .Index(t => t.TreatmentId)
                .Index(t => t.EmployeeId);
            
            AddColumn("dbo.Inventory", "TransactionType", c => c.Int(nullable: false));
            AddColumn("dbo.Inventory", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryTransactions", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.InventoryTransactions", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.InventoryTransactions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.InventoryTransactions", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.InventoryTransactions", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.InventoryTransactions", new[] { "EmployeeId" });
            DropIndex("dbo.InventoryTransactions", new[] { "TreatmentId" });
            DropIndex("dbo.InventoryTransactions", new[] { "LocationId" });
            DropIndex("dbo.InventoryTransactions", new[] { "PackagingProductId" });
            DropIndex("dbo.InventoryTransactions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropColumn("dbo.Inventory", "Description");
            DropColumn("dbo.Inventory", "TransactionType");
            DropTable("dbo.InventoryTransactions");
        }
    }
}
