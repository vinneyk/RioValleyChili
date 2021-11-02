namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryAdjustments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.InventoryAdjustments", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.InventoryAdjustments", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.InventoryAdjustments", "TreatmentId", "dbo.InventoryTreatments");
            DropIndex("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "PackagingProductId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "WarehouseLocationId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "TreatmentId" });
            CreateTable(
                "dbo.InventoryAdjustmentItems",
                c => new
                    {
                        TimeStamp = c.DateTime(nullable: false),
                        Sequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        QuantityAdjustment = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        ToteKey = c.String(maxLength: 15),
                    })
                .PrimaryKey(t => new { t.TimeStamp, t.Sequence, t.ItemSequence })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .ForeignKey("dbo.InventoryAdjustments", t => new { t.TimeStamp, t.Sequence })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.TreatmentId)
                .Index(t => new { t.TimeStamp, t.Sequence });
            
            DropColumn("dbo.InventoryAdjustments", "LotDateCreated");
            DropColumn("dbo.InventoryAdjustments", "LotDateSequence");
            DropColumn("dbo.InventoryAdjustments", "LotTypeId");
            DropColumn("dbo.InventoryAdjustments", "PackagingProductId");
            DropColumn("dbo.InventoryAdjustments", "WarehouseLocationId");
            DropColumn("dbo.InventoryAdjustments", "TreatmentId");
            DropColumn("dbo.InventoryAdjustments", "ToteKey");
            DropColumn("dbo.InventoryAdjustments", "ResolutionsHeaderId");
            DropColumn("dbo.InventoryAdjustments", "QuantityAdjustment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InventoryAdjustments", "QuantityAdjustment", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "ResolutionsHeaderId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "ToteKey", c => c.String(maxLength: 15));
            AddColumn("dbo.InventoryAdjustments", "TreatmentId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "WarehouseLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "PackagingProductId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "LotTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "LotDateSequence", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "LotDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "TreatmentId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "PackagingProductId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" }, "dbo.InventoryAdjustments");
            DropForeignKey("dbo.InventoryAdjustmentItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.InventoryAdjustmentItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.InventoryAdjustmentItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropTable("dbo.InventoryAdjustmentItems");
            CreateIndex("dbo.InventoryAdjustments", "TreatmentId");
            CreateIndex("dbo.InventoryAdjustments", "WarehouseLocationId");
            CreateIndex("dbo.InventoryAdjustments", "PackagingProductId");
            CreateIndex("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.InventoryAdjustments", "TreatmentId", "dbo.InventoryTreatments", "Id");
            AddForeignKey("dbo.InventoryAdjustments", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.InventoryAdjustments", "PackagingProductId", "dbo.PackagingProducts", "Id");
            AddForeignKey("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
        }
    }
}
