namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileLotProduction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.ProductionResults", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionResults");
            DropForeignKey("dbo.ProductionResultItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.ProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.ProductionResultItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.MillAndWetdownEntries", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.MillAndWetdownEntries", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" }, "dbo.MillAndWetdownEntries");
            DropForeignKey("dbo.MillAndWetdownResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.MillAndWetdownResultItems", "PackagingProductId", "dbo.PackagingProducts");
            DropIndex("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ProductionResults", new[] { "EmployeeId" });
            DropIndex("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionResultItems", new[] { "PackagingProductId" });
            DropIndex("dbo.ProductionResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.ProductionResultItems", new[] { "TreatmentId" });
            DropIndex("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "ChileProductId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "EmployeeId" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "PackagingProductId" });
            RenameColumn(table: "dbo.Lots", name: "DateCreated", newName: "LotDateCreated");
            CreateTable(
                "dbo.ChileLotProductions",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        ProductionType = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PickedInventory", t => new { t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.ChileLots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => new { t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.LotProductionResults",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ShiftKey = c.String(maxLength: 25),
                        ProductionBegin = c.DateTime(nullable: false),
                        ProductionEnd = c.DateTime(nullable: false),
                        DateTimeEntered = c.DateTime(nullable: false),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.ProductionLocations", t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.ChileLotProductions", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => t.EmployeeId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.LotProductionResultItems",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ResultItemSequence = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.ResultItemSequence })
                .ForeignKey("dbo.LotProductionResults", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.TreatmentId);
            
            AddColumn("dbo.Lots", "LotDateSequence", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionBatches", "LotDateSequence", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddPrimaryKey("dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropPrimaryKey("dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            AddPrimaryKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLotProductions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropColumn("dbo.Lots", "DateSequence");
            DropColumn("dbo.ChileLots", "PackScheduleDateCreated");
            DropColumn("dbo.ChileLots", "PackScheduleSequence");
            DropColumn("dbo.ChileLots", "PickedInventoryDateCreated");
            DropColumn("dbo.ChileLots", "PickedInventorySequence");
            DropColumn("dbo.ProductionBatches", "PickedInventoryDateCreated");
            DropColumn("dbo.ProductionBatches", "PickedInventorySequence");
            DropColumn("dbo.ProductionBatches", "LotSequence");
            DropTable("dbo.ProductionResults");
            DropTable("dbo.ProductionResultItems");
            DropTable("dbo.MillAndWetdownEntries");
            DropTable("dbo.MillAndWetdownResultItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MillAndWetdownResultItems",
                c => new
                    {
                        MillAndWetdownEntryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        MillAndWetdownEntrySequence = c.Int(nullable: false),
                        MillAndWetdownResultItemSequence = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MillAndWetdownEntryDateCreated, t.MillAndWetdownEntrySequence, t.MillAndWetdownResultItemSequence });
            
            CreateTable(
                "dbo.MillAndWetdownEntries",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ShiftKey = c.String(maxLength: 25),
                        ProductionBegin = c.DateTime(nullable: false),
                        ProductionEnd = c.DateTime(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.ProductionResultItems",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence, t.Sequence });
            
            CreateTable(
                "dbo.ProductionResults",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        ShiftKey = c.String(maxLength: 25),
                        ProductionStart = c.DateTime(nullable: false),
                        ProductionEnd = c.DateTime(nullable: false),
                        DateTimeEntered = c.DateTime(nullable: false),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence });
            
            AddColumn("dbo.ProductionBatches", "LotSequence", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionBatches", "PickedInventorySequence", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionBatches", "PickedInventoryDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.ChileLots", "PickedInventorySequence", c => c.Int());
            AddColumn("dbo.ChileLots", "PickedInventoryDateCreated", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.ChileLots", "PackScheduleSequence", c => c.Int());
            AddColumn("dbo.ChileLots", "PackScheduleDateCreated", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.Lots", "DateSequence", c => c.Int(nullable: false));
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotProductionResultItems", new[] { "TreatmentId" });
            DropIndex("dbo.LotProductionResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.LotProductionResultItems", new[] { "PackagingProductId" });
            DropIndex("dbo.LotProductionResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotProductionResults", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotProductionResults", new[] { "EmployeeId" });
            DropIndex("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ChileLotProductions", new[] { "EmployeeId" });
            DropIndex("dbo.ChileLotProductions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ChileLotProductions", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLotProductions");
            DropForeignKey("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotProductionResultItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.LotProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.LotProductionResultItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.LotProductionResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.LotProductionResults");
            DropForeignKey("dbo.LotProductionResults", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLotProductions");
            DropForeignKey("dbo.LotProductionResults", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ChileLotProductions", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ChileLotProductions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.ChileLotProductions", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropPrimaryKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddPrimaryKey("dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropPrimaryKey("dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddPrimaryKey("dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            DropColumn("dbo.ProductionBatches", "LotDateSequence");
            DropColumn("dbo.Lots", "LotDateSequence");
            DropTable("dbo.LotProductionResultItems");
            DropTable("dbo.LotProductionResults");
            DropTable("dbo.ChileLotProductions");
            RenameColumn(table: "dbo.Lots", name: "LotDateCreated", newName: "DateCreated");
            CreateIndex("dbo.MillAndWetdownResultItems", "PackagingProductId");
            CreateIndex("dbo.MillAndWetdownResultItems", "WarehouseLocationId");
            CreateIndex("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" });
            CreateIndex("dbo.MillAndWetdownEntries", "EmployeeId");
            CreateIndex("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.MillAndWetdownEntries", "ChileProductId");
            CreateIndex("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" });
            CreateIndex("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.ProductionResultItems", "TreatmentId");
            CreateIndex("dbo.ProductionResultItems", "WarehouseLocationId");
            CreateIndex("dbo.ProductionResultItems", "PackagingProductId");
            CreateIndex("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            CreateIndex("dbo.ProductionResults", "EmployeeId");
            CreateIndex("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            CreateIndex("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" });
            CreateIndex("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" });
            CreateIndex("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            CreateIndex("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.MillAndWetdownResultItems", "PackagingProductId", "dbo.PackagingProducts", "Id");
            AddForeignKey("dbo.MillAndWetdownResultItems", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" }, "dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.MillAndWetdownEntries", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations", new[] { "Id", "ProductionLineLocationType" });
            AddForeignKey("dbo.MillAndWetdownEntries", "ChileProductId", "dbo.ChileProducts", "Id");
            AddForeignKey("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.ProductionResultItems", "TreatmentId", "dbo.InventoryTreatments", "Id");
            AddForeignKey("dbo.ProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.ProductionResultItems", "PackagingProductId", "dbo.PackagingProducts", "Id");
            AddForeignKey("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            AddForeignKey("dbo.ProductionResults", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations", new[] { "Id", "ProductionLineLocationType" });
            AddForeignKey("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            AddForeignKey("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.PickedInventory", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            AddForeignKey("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
        }
    }
}
