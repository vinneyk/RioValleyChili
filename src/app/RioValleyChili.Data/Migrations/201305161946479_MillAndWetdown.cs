namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MillAndWetdown : DbMigration
    {
        public override void Up()
        {
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
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.ChileLots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.ProductionLocations", t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.ChileProductId)
                .Index(t => new { t.ProductionLineLocationId, t.ProductionLineLocationType });
            
            CreateTable(
                "dbo.MillAndWetdownResultItems",
                c => new
                    {
                        MillAndWetdownEntryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        MillAndWetdownEntrySequence = c.Int(nullable: false),
                        MillAndWetdownResultItemSequence = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MillAndWetdownEntryDateCreated, t.MillAndWetdownEntrySequence, t.MillAndWetdownResultItemSequence })
                .ForeignKey("dbo.MillAndWetdownEntries", t => new { t.MillAndWetdownEntryDateCreated, t.MillAndWetdownEntrySequence })
                .ForeignKey("dbo.ChileLots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .Index(t => new { t.MillAndWetdownEntryDateCreated, t.MillAndWetdownEntrySequence })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.PackagingProductId);
            
            AddColumn("dbo.PickedInventoryItems", "Tote", c => c.Int());
            DropColumn("dbo.PickedInventoryItems", "ResolutionHeaderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PickedInventoryItems", "ResolutionHeaderId", c => c.Int(nullable: false));
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "PackagingProductId" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "ChileProductId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" });
            DropForeignKey("dbo.MillAndWetdownResultItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.MillAndWetdownResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.MillAndWetdownResultItems", new[] { "MillAndWetdownEntryDateCreated", "MillAndWetdownEntrySequence" }, "dbo.MillAndWetdownEntries");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.MillAndWetdownEntries", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.MillAndWetdownEntries", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropColumn("dbo.PickedInventoryItems", "Tote");
            DropTable("dbo.MillAndWetdownResultItems");
            DropTable("dbo.MillAndWetdownEntries");
        }
    }
}
