namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DehydratedReceived : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DehydratedMaterialsReceived",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        Load = c.Int(nullable: false),
                        PurchaseOrder = c.String(maxLength: 20),
                        ShipperNumber = c.String(maxLength: 20),
                        SupplierId = c.Int(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.ChileLots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.Companies", t => t.SupplierId)
                .Index(t => t.ChileProductId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        CompanyType = c.Short(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DehydratedMaterialsReceivedItems",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        Tote = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        LocalityGrown = c.String(maxLength: 10),
                        WarehouseLocationId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        GrowerId = c.Int(nullable: false),
                        ChileVarietyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.ItemSequence })
                .ForeignKey("dbo.DehydratedMaterialsReceived", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.Companies", t => t.GrowerId)
                .ForeignKey("dbo.ChileVarieties", t => t.ChileVarietyId)
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.GrowerId)
                .Index(t => t.ChileVarietyId)
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId);
            
            CreateTable(
                "dbo.ChileVarieties",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "PackagingProductId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "ChileVarietyId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "GrowerId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "SupplierId" });
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "ChileProductId" });
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId", "dbo.ChileVarieties");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "GrowerId", "dbo.Companies");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.DehydratedMaterialsReceived");
            DropForeignKey("dbo.DehydratedMaterialsReceived", "SupplierId", "dbo.Companies");
            DropForeignKey("dbo.DehydratedMaterialsReceived", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.DehydratedMaterialsReceived", "ChileProductId", "dbo.ChileProducts");
            DropTable("dbo.ChileVarieties");
            DropTable("dbo.DehydratedMaterialsReceivedItems");
            DropTable("dbo.Companies");
            DropTable("dbo.DehydratedMaterialsReceived");
        }
    }
}
