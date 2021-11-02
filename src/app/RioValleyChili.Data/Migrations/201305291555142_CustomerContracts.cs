namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerContracts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        BrokerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Id)
                .ForeignKey("dbo.Brokers", t => t.BrokerId)
                .Index(t => t.Id)
                .Index(t => t.BrokerId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        Name = c.String(maxLength: 25),
                        PhoneNumber = c.String(maxLength: 20),
                        Address_AddressLine1 = c.String(maxLength: 50),
                        Address_AddressLine2 = c.String(maxLength: 50),
                        Address_AddressLine3 = c.String(maxLength: 50),
                        Address_City = c.String(maxLength: 75),
                        Address_State = c.String(maxLength: 50),
                        Address_PostalCode = c.String(maxLength: 15),
                        Address_Country = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.ContactId })
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Brokers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        ContractYear = c.Int(nullable: false),
                        ContractSequence = c.Int(nullable: false),
                        ContractType = c.Int(nullable: false),
                        ContractStatus = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        TermBegin = c.DateTime(nullable: false, storeType: "date"),
                        TermEnd = c.DateTime(nullable: false, storeType: "date"),
                        PaymentTerms = c.String(maxLength: 20),
                        NotesToPrint = c.String(maxLength: 300),
                        Comments = c.String(),
                        CustomerId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        BrokerId = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContractYear, t.ContractSequence })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Contacts", t => new { t.CustomerId, t.ContactId })
                .ForeignKey("dbo.Brokers", t => t.BrokerId)
                .Index(t => t.CustomerId)
                .Index(t => new { t.CustomerId, t.ContactId })
                .Index(t => t.BrokerId);
            
            CreateTable(
                "dbo.ContractItems",
                c => new
                    {
                        ContractYear = c.Int(nullable: false),
                        ContractSequence = c.Int(nullable: false),
                        ContractItemSequence = c.Int(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        UseCustomerSpec = c.Boolean(nullable: false),
                        CustomerProductCode = c.String(maxLength: 15),
                        Quantity = c.Int(nullable: false),
                        PriceBase = c.Double(nullable: false),
                        PriceFreight = c.Double(nullable: false),
                        PriceTreatment = c.Double(nullable: false),
                        PriceWarehouse = c.Double(nullable: false),
                        PriceRebate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContractYear, t.ContractSequence, t.ContractItemSequence })
                .ForeignKey("dbo.Contracts", t => new { t.ContractYear, t.ContractSequence })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.ContractYear, t.ContractSequence })
                .Index(t => t.ChileProductId)
                .Index(t => t.PackagingProductId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.CustomerProductCodes",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        Code = c.String(maxLength: 15),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.ChileProductId })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .Index(t => t.CustomerId)
                .Index(t => t.ChileProductId);
            
            CreateTable(
                "dbo.CustomerProductAttributeRanges",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        AttributeShortName = c.String(nullable: false, maxLength: 10),
                        RangeMin = c.Double(nullable: false),
                        RangeMax = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.ChileProductId, t.AttributeShortName })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.AttributeNames", t => t.AttributeShortName)
                .Index(t => t.CustomerId)
                .Index(t => t.ChileProductId)
                .Index(t => t.AttributeShortName);
            
            AddColumn("dbo.ShipmentInformation", "ShipFrom_Address_Country", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipTo_Address_Country", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "FreightBill_Address_Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerProductAttributeRanges", new[] { "AttributeShortName" });
            DropIndex("dbo.CustomerProductAttributeRanges", new[] { "ChileProductId" });
            DropIndex("dbo.CustomerProductAttributeRanges", new[] { "CustomerId" });
            DropIndex("dbo.CustomerProductCodes", new[] { "ChileProductId" });
            DropIndex("dbo.CustomerProductCodes", new[] { "CustomerId" });
            DropIndex("dbo.ContractItems", new[] { "TreatmentId" });
            DropIndex("dbo.ContractItems", new[] { "PackagingProductId" });
            DropIndex("dbo.ContractItems", new[] { "ChileProductId" });
            DropIndex("dbo.ContractItems", new[] { "ContractYear", "ContractSequence" });
            DropIndex("dbo.Contracts", new[] { "BrokerId" });
            DropIndex("dbo.Contracts", new[] { "CustomerId", "ContactId" });
            DropIndex("dbo.Contracts", new[] { "CustomerId" });
            DropIndex("dbo.Brokers", new[] { "Id" });
            DropIndex("dbo.Contacts", new[] { "CompanyId" });
            DropIndex("dbo.Customers", new[] { "BrokerId" });
            DropIndex("dbo.Customers", new[] { "Id" });
            DropForeignKey("dbo.CustomerProductAttributeRanges", "AttributeShortName", "dbo.AttributeNames");
            DropForeignKey("dbo.CustomerProductAttributeRanges", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.CustomerProductAttributeRanges", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerProductCodes", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.CustomerProductCodes", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.ContractItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.ContractItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.ContractItems", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.ContractItems", new[] { "ContractYear", "ContractSequence" }, "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "BrokerId", "dbo.Brokers");
            DropForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId" }, "dbo.Contacts");
            DropForeignKey("dbo.Contracts", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Brokers", "Id", "dbo.Companies");
            DropForeignKey("dbo.Contacts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Customers", "BrokerId", "dbo.Brokers");
            DropForeignKey("dbo.Customers", "Id", "dbo.Companies");
            AlterColumn("dbo.Companies", "Name", c => c.String());
            DropColumn("dbo.ShipmentInformation", "FreightBill_Address_Country");
            DropColumn("dbo.ShipmentInformation", "ShipTo_Address_Country");
            DropColumn("dbo.ShipmentInformation", "ShipFrom_Address_Country");
            DropTable("dbo.CustomerProductAttributeRanges");
            DropTable("dbo.CustomerProductCodes");
            DropTable("dbo.ContractItems");
            DropTable("dbo.Contracts");
            DropTable("dbo.Brokers");
            DropTable("dbo.Contacts");
            DropTable("dbo.Customers");
        }
    }
}
