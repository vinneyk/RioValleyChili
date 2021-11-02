namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SampleOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SampleOrders",
                c => new
                    {
                        Year = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Comments = c.String(),
                        PrintNotes = c.String(),
                        Volume = c.Double(nullable: false),
                        DateDue = c.DateTime(nullable: false, storeType: "date"),
                        DateReceived = c.DateTime(nullable: false, storeType: "date"),
                        DateCompleted = c.DateTime(storeType: "date"),
                        ShipmentMethod = c.String(maxLength: 25),
                        FOB = c.String(maxLength: 30),
                        Status = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        ShipToCompany = c.String(maxLength: 100),
                        ShipTo_Name = c.String(maxLength: 50),
                        ShipTo_Phone = c.String(maxLength: 50),
                        ShipTo_EMail = c.String(maxLength: 50),
                        ShipTo_Fax = c.String(maxLength: 50),
                        ShipTo_Address_AddressLine1 = c.String(maxLength: 50),
                        ShipTo_Address_AddressLine2 = c.String(maxLength: 50),
                        ShipTo_Address_AddressLine3 = c.String(maxLength: 50),
                        ShipTo_Address_City = c.String(maxLength: 75),
                        ShipTo_Address_State = c.String(maxLength: 50),
                        ShipTo_Address_PostalCode = c.String(maxLength: 15),
                        ShipTo_Address_Country = c.String(maxLength: 50),
                        Request_Name = c.String(maxLength: 50),
                        Request_Phone = c.String(maxLength: 50),
                        Request_EMail = c.String(maxLength: 50),
                        Request_Fax = c.String(maxLength: 50),
                        Request_Address_AddressLine1 = c.String(maxLength: 50),
                        Request_Address_AddressLine2 = c.String(maxLength: 50),
                        Request_Address_AddressLine3 = c.String(maxLength: 50),
                        Request_Address_City = c.String(maxLength: 75),
                        Request_Address_State = c.String(maxLength: 50),
                        Request_Address_PostalCode = c.String(maxLength: 15),
                        Request_Address_Country = c.String(maxLength: 50),
                        RequestCustomerId = c.Int(),
                        BrokerId = c.Int(),
                        SampleID = c.Int(),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.Year, t.Sequence })
                .ForeignKey("dbo.Companies", t => t.BrokerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Customers", t => t.RequestCustomerId)
                .Index(t => t.RequestCustomerId)
                .Index(t => t.BrokerId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.SampleOrderItems",
                c => new
                    {
                        SampleOrderYear = c.Int(nullable: false),
                        SampleOrderSequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Description = c.String(maxLength: 50),
                        CustomerProductName = c.String(maxLength: 50),
                        ChileProductId = c.Int(),
                        LotDateCreated = c.DateTime(storeType: "date"),
                        LotDateSequence = c.Int(),
                        LotTypeId = c.Int(),
                        SampleDetailID = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence })
                .ForeignKey("dbo.ChileLots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.SampleOrders", t => new { t.SampleOrderYear, t.SampleOrderSequence })
                .Index(t => new { t.SampleOrderYear, t.SampleOrderSequence })
                .Index(t => t.ChileProductId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.SampleOrderItemMatches",
                c => new
                    {
                        SampleOrderYear = c.Int(nullable: false),
                        SampleOrderSequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        Gran = c.String(maxLength: 50),
                        AvgAsta = c.String(maxLength: 50),
                        AoverB = c.String(maxLength: 50),
                        AvgScov = c.String(maxLength: 50),
                        H2O = c.String(maxLength: 50),
                        Scan = c.String(maxLength: 50),
                        Yeast = c.String(maxLength: 50),
                        Mold = c.String(maxLength: 50),
                        Coli = c.String(maxLength: 50),
                        TPC = c.String(maxLength: 50),
                        EColi = c.String(maxLength: 50),
                        Sal = c.String(maxLength: 50),
                        InsPrts = c.String(maxLength: 50),
                        RodHrs = c.String(maxLength: 50),
                        Notes = c.String(maxLength: 300),
                        RVCMatchID = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence })
                .ForeignKey("dbo.SampleOrderItems", t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence })
                .Index(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence });
            
            CreateTable(
                "dbo.SampleOrderItemSpecs",
                c => new
                    {
                        SampleOrderYear = c.Int(nullable: false),
                        SampleOrderSequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        AstaMin = c.Double(),
                        AstaMax = c.Double(),
                        MoistureMin = c.Double(),
                        MoistureMax = c.Double(),
                        WaterActivityMin = c.Double(),
                        WaterActivityMax = c.Double(),
                        Mesh = c.Double(),
                        AoverB = c.Double(),
                        ScovMin = c.Double(),
                        ScovMax = c.Double(),
                        ScanMin = c.Double(),
                        ScanMax = c.Double(),
                        TPCMin = c.Double(),
                        TPCMax = c.Double(),
                        YeastMin = c.Double(),
                        YeastMax = c.Double(),
                        MoldMin = c.Double(),
                        MoldMax = c.Double(),
                        ColiformsMin = c.Double(),
                        ColiformsMax = c.Double(),
                        EColiMin = c.Double(),
                        EColiMax = c.Double(),
                        SalMin = c.Double(),
                        SalMax = c.Double(),
                        Notes = c.String(maxLength: 300),
                        CustSpecID = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence })
                .ForeignKey("dbo.SampleOrderItems", t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence })
                .Index(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.ItemSequence });
            
            CreateTable(
                "dbo.SampleOrderJournalEntries",
                c => new
                    {
                        SampleOrderYear = c.Int(nullable: false),
                        SampleOrderSequence = c.Int(nullable: false),
                        EntrySequence = c.Int(nullable: false),
                        Date = c.DateTime(storeType: "date"),
                        Text = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        SamNoteID = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SampleOrderYear, t.SampleOrderSequence, t.EntrySequence })
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.SampleOrders", t => new { t.SampleOrderYear, t.SampleOrderSequence })
                .Index(t => new { t.SampleOrderYear, t.SampleOrderSequence })
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SampleOrders", "RequestCustomerId", "dbo.Customers");
            DropForeignKey("dbo.SampleOrderJournalEntries", new[] { "SampleOrderYear", "SampleOrderSequence" }, "dbo.SampleOrders");
            DropForeignKey("dbo.SampleOrderJournalEntries", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SampleOrderItemSpecs", new[] { "SampleOrderYear", "SampleOrderSequence", "ItemSequence" }, "dbo.SampleOrderItems");
            DropForeignKey("dbo.SampleOrderItems", new[] { "SampleOrderYear", "SampleOrderSequence" }, "dbo.SampleOrders");
            DropForeignKey("dbo.SampleOrderItemMatches", new[] { "SampleOrderYear", "SampleOrderSequence", "ItemSequence" }, "dbo.SampleOrderItems");
            DropForeignKey("dbo.SampleOrderItems", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.SampleOrders", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SampleOrders", "BrokerId", "dbo.Companies");
            DropIndex("dbo.SampleOrderJournalEntries", new[] { "EmployeeId" });
            DropIndex("dbo.SampleOrderJournalEntries", new[] { "SampleOrderYear", "SampleOrderSequence" });
            DropIndex("dbo.SampleOrderItemSpecs", new[] { "SampleOrderYear", "SampleOrderSequence", "ItemSequence" });
            DropIndex("dbo.SampleOrderItemMatches", new[] { "SampleOrderYear", "SampleOrderSequence", "ItemSequence" });
            DropIndex("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.SampleOrderItems", new[] { "ChileProductId" });
            DropIndex("dbo.SampleOrderItems", new[] { "SampleOrderYear", "SampleOrderSequence" });
            DropIndex("dbo.SampleOrders", new[] { "EmployeeId" });
            DropIndex("dbo.SampleOrders", new[] { "BrokerId" });
            DropIndex("dbo.SampleOrders", new[] { "RequestCustomerId" });
            DropTable("dbo.SampleOrderJournalEntries");
            DropTable("dbo.SampleOrderItemSpecs");
            DropTable("dbo.SampleOrderItemMatches");
            DropTable("dbo.SampleOrderItems");
            DropTable("dbo.SampleOrders");
        }
    }
}
