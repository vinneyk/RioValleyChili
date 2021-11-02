namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                        ProductType = c.Short(nullable: false),
                        ProductCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChileProducts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ChileTypeId = c.Int(nullable: false),
                        ChileState = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChileTypes", t => t.ChileTypeId)
                .ForeignKey("dbo.Products", t => t.Id)
                .Index(t => t.ChileTypeId)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ChileTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChileProductAttributes",
                c => new
                    {
                        ChileProductId = c.Int(nullable: false),
                        MinAsta = c.Int(nullable: false),
                        MaxAsta = c.Int(nullable: false),
                        MinScoville = c.Int(nullable: false),
                        MaxScoville = c.Int(nullable: false),
                        MinScan = c.Int(nullable: false),
                        MaxScan = c.Int(nullable: false),
                        MinGranulation = c.Int(nullable: false),
                        MaxGranulation = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChileProductId)
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .Index(t => t.ChileProductId);
            
            CreateTable(
                "dbo.ChileProductAttributeRanges",
                c => new
                    {
                        ChileProductId = c.Int(nullable: false),
                        AttributeShortName = c.String(nullable: false, maxLength: 10),
                        RangeMin = c.Double(nullable: false),
                        RangeMax = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChileProductId, t.AttributeShortName })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.AttributeNames", t => t.AttributeShortName)
                .Index(t => t.ChileProductId)
                .Index(t => t.AttributeShortName);
            
            CreateTable(
                "dbo.AttributeNames",
                c => new
                    {
                        ShortName = c.String(nullable: false, maxLength: 10),
                        Name = c.String(maxLength: 25),
                        Active = c.Boolean(nullable: false),
                        ValidForChileInventory = c.Boolean(nullable: false),
                        ValidForAdditiveInventory = c.Boolean(nullable: false),
                        ValidForPackagingInventory = c.Boolean(nullable: false),
                        DefectType = c.Short(nullable: false),
                        LoBacLimit = c.Double(),
                    })
                .PrimaryKey(t => t.ShortName);
            
            CreateTable(
                "dbo.InventoryTreatmentForAttributes",
                c => new
                    {
                        TreatmentId = c.Int(nullable: false),
                        AttributeShortName = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => new { t.TreatmentId, t.AttributeShortName })
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .ForeignKey("dbo.AttributeNames", t => t.AttributeShortName)
                .Index(t => t.TreatmentId)
                .Index(t => t.AttributeShortName);
            
            CreateTable(
                "dbo.InventoryTreatments",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        LongName = c.String(maxLength: 40),
                        ShortName = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChileProductIngredients",
                c => new
                    {
                        ChileProductId = c.Int(nullable: false),
                        AdditiveTypeId = c.Int(nullable: false),
                        Percentage = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChileProductId, t.AdditiveTypeId })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.AdditiveTypes", t => t.AdditiveTypeId)
                .Index(t => t.ChileProductId)
                .Index(t => t.AdditiveTypeId);
            
            CreateTable(
                "dbo.AdditiveTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackagingProducts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AdditiveProducts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        AdditiveTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdditiveTypes", t => t.AdditiveTypeId)
                .ForeignKey("dbo.Products", t => t.Id)
                .Index(t => t.AdditiveTypeId)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Lots",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        DateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        ProductionStatus = c.Int(nullable: false),
                        Contaminated = c.Boolean(nullable: false),
                        OnHold = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.DateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.LotStatus",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        StatusNameId = c.Int(nullable: false),
                        StatusValueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.StatusNameId })
                .ForeignKey("dbo.LotStatusNames", t => t.StatusNameId)
                .ForeignKey("dbo.LotStatusValues", t => t.StatusValueId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.StatusNameId)
                .Index(t => t.StatusValueId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.LotStatusNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LotStatusValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LotAttributes",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        AttributeShortName = c.String(nullable: false, maxLength: 10),
                        AttributeValue = c.Double(nullable: false),
                        DateEntered = c.DateTime(nullable: false),
                        DateTested = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.AttributeShortName })
                .ForeignKey("dbo.AttributeNames", t => t.AttributeShortName)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.AttributeShortName)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.Inventory",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.PackagingProductId, t.WarehouseLocationId, t.TreatmentId })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.WarehouseLocations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        Street = c.String(nullable: false, maxLength: 25),
                        Row = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.Id)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.Id)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationType = c.Short(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Locked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WarehouseType = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 150),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WarehouseLocationTransitions",
                c => new
                    {
                        WarehouseLocationId = c.Int(nullable: false),
                        OldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WarehouseLocationId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .Index(t => t.WarehouseLocationId);
            
            CreateTable(
                "dbo.LotDefects",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        DefectId = c.Int(nullable: false),
                        DefectType = c.Short(nullable: false),
                        Description = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.LotDefectResolutions",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        DefectId = c.Int(nullable: false),
                        ResolutionType = c.Short(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId })
                .ForeignKey("dbo.LotDefects", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId });
            
            CreateTable(
                "dbo.LotAttributeDefects",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        DefectId = c.Int(nullable: false),
                        AttributeShortName = c.String(nullable: false, maxLength: 10),
                        OriginalAttributeValue = c.Double(nullable: false),
                        OriginalAttributeMinLimit = c.Double(nullable: false),
                        OriginalAttributeMaxLimit = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId, t.AttributeShortName })
                .ForeignKey("dbo.LotDefects", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId })
                .ForeignKey("dbo.LotAttributes", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.AttributeShortName })
                .ForeignKey("dbo.AttributeNames", t => t.AttributeShortName)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.DefectId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.AttributeShortName })
                .Index(t => t.AttributeShortName);
            
            CreateTable(
                "dbo.ChileLots",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackScheduleDateCreated = c.DateTime(storeType: "date"),
                        PackScheduleSequence = c.Int(),
                        PickedInventoryDateCreated = c.DateTime(storeType: "date"),
                        PickedInventorySequence = c.Int(),
                        AllAttributesAreLoBac = c.Boolean(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.ProductionBatches", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => t.ChileProductId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.ProductionBatches",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        NumberOfPackagingUnits = c.Int(nullable: false),
                        ProductionHasBeenCompleted = c.Boolean(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.PackSchedules", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence }, cascadeDelete: true)
                .ForeignKey("dbo.PickedInventory", t => new { t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotSequence, t.LotTypeId })
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence })
                .Index(t => new { t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => new { t.LotDateCreated, t.LotSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.PackSchedules",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        SequentialNumber = c.Int(nullable: false),
                        ChileProductId = c.Int(nullable: false),
                        ScheduledProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionDeadline = c.DateTime(storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WorkTypeId = c.Int(nullable: false),
                        UnitsToProduce = c.Int(nullable: false),
                        SummaryOfWork = c.String(maxLength: 250),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.SequentialNumber })
                .ForeignKey("dbo.ChileProducts", t => t.ChileProductId)
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WorkTypes", t => t.WorkTypeId)
                .ForeignKey("dbo.ProductionLocations", t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => t.ChileProductId)
                .Index(t => t.PackagingProductId)
                .Index(t => t.WorkTypeId)
                .Index(t => new { t.ProductionLineLocationId, t.ProductionLineLocationType });
            
            CreateTable(
                "dbo.WorkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 35),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackScheduleTargetParameters",
                c => new
                    {
                        PackScheduleDate = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Asta = c.Int(nullable: false),
                        Scoville = c.Int(nullable: false),
                        Scan = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDate, t.PackScheduleSequence })
                .ForeignKey("dbo.PackSchedules", t => new { t.PackScheduleDate, t.PackScheduleSequence }, cascadeDelete: true)
                .Index(t => new { t.PackScheduleDate, t.PackScheduleSequence });
            
            CreateTable(
                "dbo.ProductionLocations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        Description = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => new { t.Id, t.ProductionLineLocationType })
                .ForeignKey("dbo.Locations", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PickedInventory",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        PickedReason = c.Short(nullable: false),
                        Archived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.PickedInventoryItems",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        FromWarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        ResolutionHeaderId = c.Int(nullable: false),
                        CurrentLocationId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence }, cascadeDelete: true)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.FromWarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .ForeignKey("dbo.Locations", t => t.CurrentLocationId)
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.FromWarehouseLocationId)
                .Index(t => t.TreatmentId)
                .Index(t => t.CurrentLocationId);
            
            CreateTable(
                "dbo.ProductionResults",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        ShiftKey = c.String(maxLength: 25),
                        ProductionStart = c.DateTime(nullable: false),
                        ProductionEnd = c.DateTime(nullable: false),
                        DateTimeEntered = c.DateTime(nullable: false),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.ProductionBatches", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.ProductionLocations", t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => new { t.ProductionLineLocationId, t.ProductionLineLocationType });
            
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
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence, t.Sequence })
                .ForeignKey("dbo.ProductionResults", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.ProductionBatchInstructionReferences",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        InstructionOrder = c.Int(nullable: false),
                        InstructionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence, t.InstructionOrder })
                .ForeignKey("dbo.ProductionBatches", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence }, cascadeDelete: true)
                .ForeignKey("dbo.Instructions", t => t.InstructionId)
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence })
                .Index(t => t.InstructionId);
            
            CreateTable(
                "dbo.Instructions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeId = c.Int(nullable: false),
                        InstructionText = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InstructionTypes", t => t.TypeId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.InstructionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackagingLots",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.AdditiveLots",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        AdditiveProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.AdditiveProducts", t => t.AdditiveProductId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.AdditiveProductId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId });
            
            CreateTable(
                "dbo.ProductionSchedules",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType })
                .ForeignKey("dbo.ProductionLocations", t => new { t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => new { t.ProductionLineLocationId, t.ProductionLineLocationType });
            
            CreateTable(
                "dbo.ScheduledPackSchedules",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType, t.PackScheduleDateCreated, t.PackScheduleSequence })
                .ForeignKey("dbo.ProductionSchedules", t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType })
                .ForeignKey("dbo.PackSchedules", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence })
                .Index(t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence });
            
            CreateTable(
                "dbo.ScheduledInstructions",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        InstructionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType, t.Sequence })
                .ForeignKey("dbo.ProductionSchedules", t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType })
                .ForeignKey("dbo.Instructions", t => t.InstructionId)
                .Index(t => new { t.ProductionDate, t.ProductionLineLocationId, t.ProductionLineLocationType })
                .Index(t => t.InstructionId);
            
            CreateTable(
                "dbo.InventoryPickOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.InventoryPickOrderItems",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        OrderSequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        TreatmentId = c.Int(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.OrderSequence, t.ItemSequence })
                .ForeignKey("dbo.InventoryPickOrders", t => new { t.DateCreated, t.OrderSequence })
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.DateCreated, t.OrderSequence })
                .Index(t => t.ProductId)
                .Index(t => t.PackagingProductId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.TreatmentOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        InventoryTreatmentId = c.Int(nullable: false),
                        ShipmentInfoDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        ShipmentInfoSequence = c.Int(nullable: false),
                        OrderStatus = c.Short(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.InventoryTreatments", t => t.InventoryTreatmentId)
                .ForeignKey("dbo.InventoryPickOrders", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.ShipmentInformation", t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .Index(t => t.InventoryTreatmentId)
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence });
            
            CreateTable(
                "dbo.ShipmentInformation",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        PalletWeight = c.Double(nullable: false),
                        PalletQuantity = c.Int(nullable: false),
                        FreightType = c.String(maxLength: 25),
                        DriverName = c.String(maxLength: 25),
                        CarrierName = c.String(maxLength: 25),
                        TrailerLicenseNumber = c.String(maxLength: 25),
                        ContainerSeal = c.String(maxLength: 25),
                        ShipFrom_Name = c.String(maxLength: 25),
                        ShipFrom_Address_AddressLine1 = c.String(maxLength: 50),
                        ShipFrom_Address_AddressLine2 = c.String(maxLength: 50),
                        ShipFrom_Address_AddressLine3 = c.String(maxLength: 50),
                        ShipFrom_Address_City = c.String(maxLength: 75),
                        ShipFrom_Address_State = c.String(maxLength: 50),
                        ShipFrom_Address_PostalCode = c.String(maxLength: 15),
                        ShipFrom_AdditionalInfo = c.String(maxLength: 100),
                        ShipTo_Name = c.String(maxLength: 25),
                        ShipTo_Address_AddressLine1 = c.String(maxLength: 50),
                        ShipTo_Address_AddressLine2 = c.String(maxLength: 50),
                        ShipTo_Address_AddressLine3 = c.String(maxLength: 50),
                        ShipTo_Address_City = c.String(maxLength: 75),
                        ShipTo_Address_State = c.String(maxLength: 50),
                        ShipTo_Address_PostalCode = c.String(maxLength: 15),
                        ShipTo_AdditionalInfo = c.String(maxLength: 100),
                        FreightBill_Name = c.String(maxLength: 25),
                        FreightBill_Address_AddressLine1 = c.String(maxLength: 50),
                        FreightBill_Address_AddressLine2 = c.String(maxLength: 50),
                        FreightBill_Address_AddressLine3 = c.String(maxLength: 50),
                        FreightBill_Address_City = c.String(maxLength: 75),
                        FreightBill_Address_State = c.String(maxLength: 50),
                        FreightBill_Address_PostalCode = c.String(maxLength: 15),
                        FreightBill_AdditionalInfo = c.String(maxLength: 100),
                        RequiredDeliveryDate = c.DateTime(storeType: "date"),
                        Comments = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.InterWarehouseOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ShipmentInfoDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        ShipmentInfoSequence = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        OrderStatus = c.Short(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryPickOrders", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.ShipmentInformation", t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .Index(t => t.WarehouseLocationId)
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence });
            
            CreateTable(
                "dbo.IntraWarehouseOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        TrackingSheetNumber = c.String(maxLength: 20),
                        OperatorName = c.String(maxLength: 20),
                        MovementDate = c.DateTime(nullable: false, storeType: "date"),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.InventoryAdjustments",
                c => new
                    {
                        TimeStamp = c.DateTime(nullable: false),
                        Sequence = c.Int(nullable: false),
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        PackagingProductId = c.Int(nullable: false),
                        WarehouseLocationId = c.Int(nullable: false),
                        TreatmentId = c.Int(nullable: false),
                        ResolutionsHeaderId = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        QuantityAdjustment = c.Int(nullable: false),
                        Comment = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.TimeStamp, t.Sequence })
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .ForeignKey("dbo.PackagingProducts", t => t.PackagingProductId)
                .ForeignKey("dbo.WarehouseLocations", t => t.WarehouseLocationId)
                .ForeignKey("dbo.InventoryTreatments", t => t.TreatmentId)
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.PackagingProductId)
                .Index(t => t.WarehouseLocationId)
                .Index(t => t.TreatmentId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.InventoryAdjustments", new[] { "TreatmentId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "WarehouseLocationId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "PackagingProductId" });
            DropIndex("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.IntraWarehouseOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "WarehouseLocationId" });
            DropIndex("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.TreatmentOrders", new[] { "InventoryTreatmentId" });
            DropIndex("dbo.InventoryPickOrderItems", new[] { "TreatmentId" });
            DropIndex("dbo.InventoryPickOrderItems", new[] { "PackagingProductId" });
            DropIndex("dbo.InventoryPickOrderItems", new[] { "ProductId" });
            DropIndex("dbo.InventoryPickOrderItems", new[] { "DateCreated", "OrderSequence" });
            DropIndex("dbo.InventoryPickOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.ScheduledInstructions", new[] { "InstructionId" });
            DropIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.AdditiveLots", new[] { "AdditiveProductId" });
            DropIndex("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.PackagingLots", new[] { "PackagingProductId" });
            DropIndex("dbo.Instructions", new[] { "TypeId" });
            DropIndex("dbo.ProductionBatchInstructionReferences", new[] { "InstructionId" });
            DropIndex("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionResultItems", new[] { "TreatmentId" });
            DropIndex("dbo.ProductionResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.ProductionResultItems", new[] { "PackagingProductId" });
            DropIndex("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.PickedInventoryItems", new[] { "CurrentLocationId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "TreatmentId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "FromWarehouseLocationId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "PackagingProductId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.ProductionLocations", new[] { "Id" });
            DropIndex("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" });
            DropIndex("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.PackSchedules", new[] { "WorkTypeId" });
            DropIndex("dbo.PackSchedules", new[] { "PackagingProductId" });
            DropIndex("dbo.PackSchedules", new[] { "ChileProductId" });
            DropIndex("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" });
            DropIndex("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence" });
            DropIndex("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.ChileLots", new[] { "ChileProductId" });
            DropIndex("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.LotAttributeDefects", new[] { "AttributeShortName" });
            DropIndex("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" });
            DropIndex("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "DefectId" });
            DropIndex("dbo.LotDefectResolutions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "DefectId" });
            DropIndex("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.WarehouseLocationTransitions", new[] { "WarehouseLocationId" });
            DropIndex("dbo.WarehouseLocations", new[] { "WarehouseId" });
            DropIndex("dbo.WarehouseLocations", new[] { "Id" });
            DropIndex("dbo.Inventory", new[] { "TreatmentId" });
            DropIndex("dbo.Inventory", new[] { "WarehouseLocationId" });
            DropIndex("dbo.Inventory", new[] { "PackagingProductId" });
            DropIndex("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotAttributes", new[] { "AttributeShortName" });
            DropIndex("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropIndex("dbo.LotStatus", new[] { "StatusValueId" });
            DropIndex("dbo.LotStatus", new[] { "StatusNameId" });
            DropIndex("dbo.AdditiveProducts", new[] { "Id" });
            DropIndex("dbo.AdditiveProducts", new[] { "AdditiveTypeId" });
            DropIndex("dbo.PackagingProducts", new[] { "Id" });
            DropIndex("dbo.ChileProductIngredients", new[] { "AdditiveTypeId" });
            DropIndex("dbo.ChileProductIngredients", new[] { "ChileProductId" });
            DropIndex("dbo.InventoryTreatmentForAttributes", new[] { "AttributeShortName" });
            DropIndex("dbo.InventoryTreatmentForAttributes", new[] { "TreatmentId" });
            DropIndex("dbo.ChileProductAttributeRanges", new[] { "AttributeShortName" });
            DropIndex("dbo.ChileProductAttributeRanges", new[] { "ChileProductId" });
            DropIndex("dbo.ChileProductAttributes", new[] { "ChileProductId" });
            DropIndex("dbo.ChileProducts", new[] { "Id" });
            DropIndex("dbo.ChileProducts", new[] { "ChileTypeId" });
            DropForeignKey("dbo.InventoryAdjustments", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.InventoryAdjustments", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.InventoryAdjustments", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.InventoryAdjustments", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.IntraWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.InterWarehouseOrders", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.TreatmentOrders", "InventoryTreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.InventoryPickOrderItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.InventoryPickOrderItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.InventoryPickOrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InventoryPickOrderItems", new[] { "DateCreated", "OrderSequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.InventoryPickOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.ScheduledInstructions", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.AdditiveLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.AdditiveLots", "AdditiveProductId", "dbo.AdditiveProducts");
            DropForeignKey("dbo.PackagingLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.PackagingLots", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.Instructions", "TypeId", "dbo.InstructionTypes");
            DropForeignKey("dbo.ProductionBatchInstructionReferences", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.ProductionResultItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.ProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.ProductionResultItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.ProductionResultItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionResults");
            DropForeignKey("dbo.ProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.ProductionResults", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.PickedInventoryItems", "CurrentLocationId", "dbo.Locations");
            DropForeignKey("dbo.PickedInventoryItems", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.PickedInventoryItems", "FromWarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.PickedInventoryItems", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.PickedInventoryItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.PickedInventoryItems", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.ProductionLocations", "Id", "dbo.Locations");
            DropForeignKey("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropForeignKey("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.PackSchedules", "WorkTypeId", "dbo.WorkTypes");
            DropForeignKey("dbo.PackSchedules", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.PackSchedules", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.ProductionBatches", new[] { "LotDateCreated", "LotSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ProductionBatches", new[] { "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropForeignKey("dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.ChileLots", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.ChileLots", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.LotAttributeDefects", "AttributeShortName", "dbo.AttributeNames");
            DropForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" }, "dbo.LotAttributes");
            DropForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "DefectId" }, "dbo.LotDefects");
            DropForeignKey("dbo.LotDefectResolutions", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "DefectId" }, "dbo.LotDefects");
            DropForeignKey("dbo.LotDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.WarehouseLocationTransitions", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.WarehouseLocations", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.WarehouseLocations", "Id", "dbo.Locations");
            DropForeignKey("dbo.Inventory", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.Inventory", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.Inventory", "PackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotAttributes", "AttributeShortName", "dbo.AttributeNames");
            DropForeignKey("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotStatus", "StatusValueId", "dbo.LotStatusValues");
            DropForeignKey("dbo.LotStatus", "StatusNameId", "dbo.LotStatusNames");
            DropForeignKey("dbo.AdditiveProducts", "Id", "dbo.Products");
            DropForeignKey("dbo.AdditiveProducts", "AdditiveTypeId", "dbo.AdditiveTypes");
            DropForeignKey("dbo.PackagingProducts", "Id", "dbo.Products");
            DropForeignKey("dbo.ChileProductIngredients", "AdditiveTypeId", "dbo.AdditiveTypes");
            DropForeignKey("dbo.ChileProductIngredients", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.InventoryTreatmentForAttributes", "AttributeShortName", "dbo.AttributeNames");
            DropForeignKey("dbo.InventoryTreatmentForAttributes", "TreatmentId", "dbo.InventoryTreatments");
            DropForeignKey("dbo.ChileProductAttributeRanges", "AttributeShortName", "dbo.AttributeNames");
            DropForeignKey("dbo.ChileProductAttributeRanges", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.ChileProductAttributes", "ChileProductId", "dbo.ChileProducts");
            DropForeignKey("dbo.ChileProducts", "Id", "dbo.Products");
            DropForeignKey("dbo.ChileProducts", "ChileTypeId", "dbo.ChileTypes");
            DropTable("dbo.InventoryAdjustments");
            DropTable("dbo.IntraWarehouseOrders");
            DropTable("dbo.InterWarehouseOrders");
            DropTable("dbo.ShipmentInformation");
            DropTable("dbo.TreatmentOrders");
            DropTable("dbo.InventoryPickOrderItems");
            DropTable("dbo.InventoryPickOrders");
            DropTable("dbo.ScheduledInstructions");
            DropTable("dbo.ScheduledPackSchedules");
            DropTable("dbo.ProductionSchedules");
            DropTable("dbo.AdditiveLots");
            DropTable("dbo.PackagingLots");
            DropTable("dbo.InstructionTypes");
            DropTable("dbo.Instructions");
            DropTable("dbo.ProductionBatchInstructionReferences");
            DropTable("dbo.ProductionResultItems");
            DropTable("dbo.ProductionResults");
            DropTable("dbo.PickedInventoryItems");
            DropTable("dbo.PickedInventory");
            DropTable("dbo.ProductionLocations");
            DropTable("dbo.PackScheduleTargetParameters");
            DropTable("dbo.WorkTypes");
            DropTable("dbo.PackSchedules");
            DropTable("dbo.ProductionBatches");
            DropTable("dbo.ChileLots");
            DropTable("dbo.LotAttributeDefects");
            DropTable("dbo.LotDefectResolutions");
            DropTable("dbo.LotDefects");
            DropTable("dbo.WarehouseLocationTransitions");
            DropTable("dbo.Warehouses");
            DropTable("dbo.Locations");
            DropTable("dbo.WarehouseLocations");
            DropTable("dbo.Inventory");
            DropTable("dbo.LotAttributes");
            DropTable("dbo.LotStatusValues");
            DropTable("dbo.LotStatusNames");
            DropTable("dbo.LotStatus");
            DropTable("dbo.Lots");
            DropTable("dbo.AdditiveProducts");
            DropTable("dbo.PackagingProducts");
            DropTable("dbo.AdditiveTypes");
            DropTable("dbo.ChileProductIngredients");
            DropTable("dbo.InventoryTreatments");
            DropTable("dbo.InventoryTreatmentForAttributes");
            DropTable("dbo.AttributeNames");
            DropTable("dbo.ChileProductAttributeRanges");
            DropTable("dbo.ChileProductAttributes");
            DropTable("dbo.ChileTypes");
            DropTable("dbo.ChileProducts");
            DropTable("dbo.Products");
        }
    }
}
