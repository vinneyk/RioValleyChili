namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SalesQuotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesQuoteItems",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        PriceBase = c.Double(nullable: false),
                        PriceFreight = c.Double(nullable: false),
                        PriceTreatment = c.Double(nullable: false),
                        PriceWarehouse = c.Double(nullable: false),
                        PriceRebate = c.Double(nullable: false),
                        CustomerProductCode = c.String(maxLength: 100),
                        QDetailID = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.SalesQuotes", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => t.ProductId)
                .Index(t => t.PackagingProductId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.SalesQuotes",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        QuoteNum = c.Int(),
                        QuoteDate = c.DateTime(nullable: false),
                        DateReceived = c.DateTime(storeType: "date"),
                        CalledBy = c.String(),
                        TakenBy = c.String(),
                        SourceFacilityId = c.Int(),
                        CustomerId = c.Int(),
                        BrokerId = c.Int(),
                        ShipmentInfoDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        ShipmentInfoSequence = c.Int(nullable: false),
                        SoldTo_Name = c.String(maxLength: 50),
                        SoldTo_Phone = c.String(maxLength: 50),
                        SoldTo_EMail = c.String(maxLength: 50),
                        SoldTo_Fax = c.String(maxLength: 50),
                        SoldTo_Address_AddressLine1 = c.String(maxLength: 50),
                        SoldTo_Address_AddressLine2 = c.String(maxLength: 50),
                        SoldTo_Address_AddressLine3 = c.String(maxLength: 50),
                        SoldTo_Address_City = c.String(maxLength: 75),
                        SoldTo_Address_State = c.String(maxLength: 50),
                        SoldTo_Address_PostalCode = c.String(maxLength: 15),
                        SoldTo_Address_Country = c.String(maxLength: 50),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.Companies", t => t.BrokerId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.ShipmentInformation", t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .ForeignKey("dbo.Facilities", t => t.SourceFacilityId)
                .Index(t => t.SourceFacilityId)
                .Index(t => t.CustomerId)
                .Index(t => t.BrokerId)
                .Index(t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesQuoteItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.SalesQuotes", "SourceFacilityId", "dbo.Facilities");
            DropForeignKey("dbo.SalesQuotes", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.SalesQuoteItems", new[] { "DateCreated", "Sequence" }, "dbo.SalesQuotes");
            DropForeignKey("dbo.SalesQuotes", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SalesQuotes", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.SalesQuotes", "BrokerId", "dbo.Companies");
            DropForeignKey("dbo.SalesQuoteItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SalesQuoteItems", "PackagingProductId", "dbo.PackagingProducts");
            DropIndex("dbo.SalesQuotes", new[] { "EmployeeId" });
            DropIndex("dbo.SalesQuotes", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.SalesQuotes", new[] { "BrokerId" });
            DropIndex("dbo.SalesQuotes", new[] { "CustomerId" });
            DropIndex("dbo.SalesQuotes", new[] { "SourceFacilityId" });
            DropIndex("dbo.SalesQuoteItems", new[] { "TreatmentId" });
            DropIndex("dbo.SalesQuoteItems", new[] { "PackagingProductId" });
            DropIndex("dbo.SalesQuoteItems", new[] { "ProductId" });
            DropIndex("dbo.SalesQuoteItems", new[] { "DateCreated", "Sequence" });
            DropTable("dbo.SalesQuotes");
            DropTable("dbo.SalesQuoteItems");
        }
    }
}
