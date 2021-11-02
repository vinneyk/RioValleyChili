namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InventoryShipmentOrder_SourceFacility : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerOrders", "ShipFromFacilityId", "dbo.Facilities");
            DropIndex("dbo.CustomerOrders", new[] { "ShipFromFacilityId" });
            AddColumn("dbo.InventoryShipmentOrders", "SourceFacilityId", c => c.Int(nullable: false));
            CreateIndex("dbo.InventoryShipmentOrders", "SourceFacilityId");
            AddForeignKey("dbo.InventoryShipmentOrders", "SourceFacilityId", "dbo.Facilities", "Id");
            DropColumn("dbo.CustomerOrders", "ShipFromFacilityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "ShipFromFacilityId", c => c.Int(nullable: false));
            DropForeignKey("dbo.InventoryShipmentOrders", "SourceFacilityId", "dbo.Facilities");
            DropIndex("dbo.InventoryShipmentOrders", new[] { "SourceFacilityId" });
            DropColumn("dbo.InventoryShipmentOrders", "SourceFacilityId");
            CreateIndex("dbo.CustomerOrders", "ShipFromFacilityId");
            AddForeignKey("dbo.CustomerOrders", "ShipFromFacilityId", "dbo.Facilities", "Id");
        }
    }
}
