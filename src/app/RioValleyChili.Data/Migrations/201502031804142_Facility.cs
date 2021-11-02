namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Facility : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Inventory", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.WarehouseLocations", "Id", "dbo.Locations");
            DropForeignKey("dbo.WarehouseLocations", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.PickedInventoryItems", "FromWarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.ProductionLocations", "Id", "dbo.Locations");
            DropForeignKey("dbo.LotProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.CustomerOrders", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.CustomerOrders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InterWarehouseOrders", "DestinationWarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders");
            DropForeignKey("dbo.InventoryAdjustmentItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", "dbo.Companies");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropForeignKey("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations");
            DropForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropIndex("dbo.PickedInventoryItems", new[] { "FromWarehouseLocationId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "DestinationWarehouseLocationId" });
            DropIndex("dbo.WarehouseLocations", new[] { "Id" });
            DropIndex("dbo.WarehouseLocations", new[] { "WarehouseId" });
            DropIndex("dbo.Inventory", new[] { "WarehouseLocationId" });
            DropIndex("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ProductionLocations", new[] { "Id" });
            DropIndex("dbo.LotProductionResultItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.CustomerOrders", new[] { "WarehouseId" });
            DropIndex("dbo.CustomerOrders", new[] { "EmployeeId" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "DestinationWarehouseId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "WarehouseLocationId" });
            DropIndex("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            DropIndex("dbo.TreatmentOrders", new[] { "TreatmentFacilityCompanyId" });
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "WarehouseLocationId" });
            DropPrimaryKey("dbo.Inventory");
            DropPrimaryKey("dbo.ProductionSchedules");
            DropPrimaryKey("dbo.ScheduledInstructions");
            DropPrimaryKey("dbo.ScheduledPackSchedules");
            CreateTable(
                "dbo.Facilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacilityType = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 150),
                        Active = c.Boolean(nullable: false),
                        PhoneNumber = c.String(maxLength: 50),
                        EMailAddress = c.String(maxLength: 50),
                        Address_AddressLine1 = c.String(maxLength: 50),
                        Address_AddressLine2 = c.String(maxLength: 50),
                        Address_AddressLine3 = c.String(maxLength: 50),
                        Address_City = c.String(maxLength: 75),
                        Address_State = c.String(maxLength: 50),
                        Address_PostalCode = c.String(maxLength: 15),
                        Address_Country = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PickedInventoryItems", "FromLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.PickedInventoryItems", "DestinationLocationId", c => c.Int());
            AddColumn("dbo.PickedInventoryItems", "DetailID", c => c.DateTime());
            AddColumn("dbo.Locations", "Description", c => c.String());
            AddColumn("dbo.Locations", "FacilityId", c => c.Int(nullable: false));
            AddColumn("dbo.Inventory", "LocationId", c => c.Int(nullable: false));
            AddColumn("dbo.LotProductionResultItems", "LocationId", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "ShipFromFacilityId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryShipmentOrders", "OrderStatus", c => c.Short(nullable: false));
            AddColumn("dbo.InventoryShipmentOrders", "DestinationFacilityId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryShipmentOrders", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryShipmentOrders", "TimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.InventoryAdjustmentItems", "LocationId", c => c.Int(nullable: false));
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "LocationId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "LocationId", "TreatmentId", "ToteKey" });
            AddPrimaryKey("dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddPrimaryKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "Sequence" });
            AddPrimaryKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "PackScheduleDateCreated", "PackScheduleSequence" });
            CreateIndex("dbo.PickedInventoryItems", "FromLocationId");
            CreateIndex("dbo.PickedInventoryItems", "DestinationLocationId");
            CreateIndex("dbo.Locations", "FacilityId");
            CreateIndex("dbo.LotProductionResults", "ProductionLineLocationId");
            CreateIndex("dbo.LotProductionResultItems", "LocationId");
            CreateIndex("dbo.Inventory", "LocationId");
            CreateIndex("dbo.CustomerOrders", "ShipFromFacilityId");
            CreateIndex("dbo.InventoryShipmentOrders", "DestinationFacilityId");
            CreateIndex("dbo.InventoryShipmentOrders", "EmployeeId");
            CreateIndex("dbo.InventoryAdjustmentItems", "LocationId");
            CreateIndex("dbo.PackSchedules", "ProductionLineLocationId");
            CreateIndex("dbo.ProductionSchedules", "ProductionLineLocationId");
            CreateIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" });
            CreateIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "LocationId");
            AddForeignKey("dbo.Locations", "FacilityId", "dbo.Facilities", "Id");
            AddForeignKey("dbo.PickedInventoryItems", "DestinationLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.PickedInventoryItems", "FromLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.LotProductionResultItems", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Inventory", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.InventoryShipmentOrders", "DestinationFacilityId", "dbo.Facilities", "Id");
            AddForeignKey("dbo.InventoryShipmentOrders", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.CustomerOrders", "ShipFromFacilityId", "dbo.Facilities", "Id");
            AddForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Facilities", "Id");
            AddForeignKey("dbo.InventoryAdjustmentItems", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.LotProductionResults", "ProductionLineLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.PackSchedules", "ProductionLineLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.ProductionSchedules", "ProductionLineLocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropColumn("dbo.PickedInventoryItems", "FromWarehouseLocationId");
            DropColumn("dbo.PickedInventoryItems", "DestinationWarehouseLocationId");
            DropColumn("dbo.Inventory", "WarehouseLocationId");
            DropColumn("dbo.LotProductionResults", "ProductionLineLocationType");
            DropColumn("dbo.LotProductionResultItems", "WarehouseLocationId");
            DropColumn("dbo.CustomerOrders", "WarehouseId");
            DropColumn("dbo.CustomerOrders", "EmployeeId");
            DropColumn("dbo.CustomerOrders", "TimeStamp");
            DropColumn("dbo.InventoryAdjustmentItems", "WarehouseLocationId");
            DropColumn("dbo.PackSchedules", "ProductionLineLocationType");
            DropColumn("dbo.ProductionSchedules", "ProductionLineLocationType");
            DropColumn("dbo.ScheduledInstructions", "ProductionLineLocationType");
            DropColumn("dbo.ScheduledPackSchedules", "ProductionLineLocationType");
            DropColumn("dbo.TreatmentOrders", "OrderStatus");
            DropColumn("dbo.TreatmentOrders", "TreatmentFacilityCompanyId");
            DropColumn("dbo.TreatmentOrders", "User");
            DropColumn("dbo.TreatmentOrders", "TimeStamp");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId");
            DropTable("dbo.WarehouseLocations");
            DropTable("dbo.Warehouses");
            DropTable("dbo.ProductionLocations");
            DropTable("dbo.InterWarehouseOrders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InterWarehouseOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        DestinationWarehouseId = c.Int(nullable: false),
                        OrderStatus = c.Short(nullable: false),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.ProductionLocations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ProductionLineLocationType = c.Int(nullable: false),
                        Description = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => new { t.Id, t.ProductionLineLocationType });
            
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
                "dbo.WarehouseLocations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        Street = c.String(nullable: false, maxLength: 25),
                        Row = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.TreatmentOrders", "TimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.TreatmentOrders", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", c => c.Int(nullable: false));
            AddColumn("dbo.TreatmentOrders", "OrderStatus", c => c.Short(nullable: false));
            AddColumn("dbo.ScheduledPackSchedules", "ProductionLineLocationType", c => c.Int(nullable: false));
            AddColumn("dbo.ScheduledInstructions", "ProductionLineLocationType", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionSchedules", "ProductionLineLocationType", c => c.Int(nullable: false));
            AddColumn("dbo.PackSchedules", "ProductionLineLocationType", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustmentItems", "WarehouseLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "TimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.CustomerOrders", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "WarehouseId", c => c.Int(nullable: false));
            AddColumn("dbo.LotProductionResultItems", "WarehouseLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.LotProductionResults", "ProductionLineLocationType", c => c.Int(nullable: false));
            AddColumn("dbo.Inventory", "WarehouseLocationId", c => c.Int(nullable: false));
            AddColumn("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", c => c.Int());
            AddColumn("dbo.PickedInventoryItems", "FromWarehouseLocationId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ProductionSchedules", "ProductionLineLocationId", "dbo.Locations");
            DropForeignKey("dbo.PackSchedules", "ProductionLineLocationId", "dbo.Locations");
            DropForeignKey("dbo.LotProductionResults", "ProductionLineLocationId", "dbo.Locations");
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.InventoryAdjustmentItems", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Facilities");
            DropForeignKey("dbo.CustomerOrders", "ShipFromFacilityId", "dbo.Facilities");
            DropForeignKey("dbo.InventoryShipmentOrders", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.InventoryShipmentOrders", "DestinationFacilityId", "dbo.Facilities");
            DropForeignKey("dbo.Inventory", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.LotProductionResultItems", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.PickedInventoryItems", "FromLocationId", "dbo.Locations");
            DropForeignKey("dbo.PickedInventoryItems", "DestinationLocationId", "dbo.Locations");
            DropForeignKey("dbo.Locations", "FacilityId", "dbo.Facilities");
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "LocationId" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropIndex("dbo.ProductionSchedules", new[] { "ProductionLineLocationId" });
            DropIndex("dbo.PackSchedules", new[] { "ProductionLineLocationId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "LocationId" });
            DropIndex("dbo.InventoryShipmentOrders", new[] { "EmployeeId" });
            DropIndex("dbo.InventoryShipmentOrders", new[] { "DestinationFacilityId" });
            DropIndex("dbo.CustomerOrders", new[] { "ShipFromFacilityId" });
            DropIndex("dbo.Inventory", new[] { "LocationId" });
            DropIndex("dbo.LotProductionResultItems", new[] { "LocationId" });
            DropIndex("dbo.LotProductionResults", new[] { "ProductionLineLocationId" });
            DropIndex("dbo.Locations", new[] { "FacilityId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "DestinationLocationId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "FromLocationId" });
            DropPrimaryKey("dbo.ScheduledPackSchedules");
            DropPrimaryKey("dbo.ScheduledInstructions");
            DropPrimaryKey("dbo.ProductionSchedules");
            DropPrimaryKey("dbo.Inventory");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "LocationId");
            DropColumn("dbo.InventoryAdjustmentItems", "LocationId");
            DropColumn("dbo.InventoryShipmentOrders", "TimeStamp");
            DropColumn("dbo.InventoryShipmentOrders", "EmployeeId");
            DropColumn("dbo.InventoryShipmentOrders", "DestinationFacilityId");
            DropColumn("dbo.InventoryShipmentOrders", "OrderStatus");
            DropColumn("dbo.CustomerOrders", "ShipFromFacilityId");
            DropColumn("dbo.LotProductionResultItems", "LocationId");
            DropColumn("dbo.Inventory", "LocationId");
            DropColumn("dbo.Locations", "FacilityId");
            DropColumn("dbo.Locations", "Description");
            DropColumn("dbo.PickedInventoryItems", "DetailID");
            DropColumn("dbo.PickedInventoryItems", "DestinationLocationId");
            DropColumn("dbo.PickedInventoryItems", "FromLocationId");
            DropTable("dbo.Facilities");
            AddPrimaryKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType", "PackScheduleDateCreated", "PackScheduleSequence" });
            AddPrimaryKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType", "Sequence" });
            AddPrimaryKey("dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            AddPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "WarehouseLocationId", "TreatmentId", "ToteKey" });
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId");
            CreateIndex("dbo.TreatmentOrders", "TreatmentFacilityCompanyId");
            CreateIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.InventoryAdjustmentItems", "WarehouseLocationId");
            CreateIndex("dbo.InterWarehouseOrders", "DestinationWarehouseId");
            CreateIndex("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" });
            CreateIndex("dbo.CustomerOrders", "EmployeeId");
            CreateIndex("dbo.CustomerOrders", "WarehouseId");
            CreateIndex("dbo.LotProductionResultItems", "WarehouseLocationId");
            CreateIndex("dbo.ProductionLocations", "Id");
            CreateIndex("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" });
            CreateIndex("dbo.Inventory", "WarehouseLocationId");
            CreateIndex("dbo.WarehouseLocations", "WarehouseId");
            CreateIndex("dbo.WarehouseLocations", "Id");
            CreateIndex("dbo.PickedInventoryItems", "DestinationWarehouseLocationId");
            CreateIndex("dbo.PickedInventoryItems", "FromWarehouseLocationId");
            AddForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            AddForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId", "ProductionLineLocationType" });
            AddForeignKey("dbo.ProductionSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations", new[] { "Id", "ProductionLineLocationType" });
            AddForeignKey("dbo.PackSchedules", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations", new[] { "Id", "ProductionLineLocationType" });
            AddForeignKey("dbo.LotProductionResults", new[] { "ProductionLineLocationId", "ProductionLineLocationType" }, "dbo.ProductionLocations", new[] { "Id", "ProductionLineLocationType" });
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", "dbo.Companies", "Id");
            AddForeignKey("dbo.InventoryAdjustmentItems", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InterWarehouseOrders", "DestinationWarehouseId", "dbo.Warehouses", "Id");
            AddForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Warehouses", "Id");
            AddForeignKey("dbo.CustomerOrders", "WarehouseId", "dbo.Warehouses", "Id");
            AddForeignKey("dbo.CustomerOrders", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.LotProductionResultItems", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.ProductionLocations", "Id", "dbo.Locations", "Id");
            AddForeignKey("dbo.PickedInventoryItems", "FromWarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.WarehouseLocations", "WarehouseId", "dbo.Warehouses", "Id");
            AddForeignKey("dbo.WarehouseLocations", "Id", "dbo.Locations", "Id");
            AddForeignKey("dbo.Inventory", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
        }
    }
}
