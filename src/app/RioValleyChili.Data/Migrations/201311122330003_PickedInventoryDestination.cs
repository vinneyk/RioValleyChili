namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PickedInventoryDestination : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InterWarehouseOrders", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropIndex("dbo.InterWarehouseOrders", new[] { "WarehouseLocationId" });
            AddColumn("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", c => c.Int());
            AddColumn("dbo.InterWarehouseOrders", "DestinationWarehouseId", c => c.Int(nullable: false));
            AddForeignKey("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", "dbo.WarehouseLocations", "Id");
            AddForeignKey("dbo.InterWarehouseOrders", "DestinationWarehouseId", "dbo.Warehouses", "Id");
            CreateIndex("dbo.PickedInventoryItems", "DestinationWarehouseLocationId");
            CreateIndex("dbo.InterWarehouseOrders", "DestinationWarehouseId");
            DropColumn("dbo.InterWarehouseOrders", "WarehouseLocationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InterWarehouseOrders", "WarehouseLocationId", c => c.Int(nullable: false));
            DropIndex("dbo.InterWarehouseOrders", new[] { "DestinationWarehouseId" });
            DropIndex("dbo.PickedInventoryItems", new[] { "DestinationWarehouseLocationId" });
            DropForeignKey("dbo.InterWarehouseOrders", "DestinationWarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.PickedInventoryItems", "DestinationWarehouseLocationId", "dbo.WarehouseLocations");
            DropColumn("dbo.InterWarehouseOrders", "DestinationWarehouseId");
            DropColumn("dbo.PickedInventoryItems", "DestinationWarehouseLocationId");
            CreateIndex("dbo.InterWarehouseOrders", "WarehouseLocationId");
            AddForeignKey("dbo.InterWarehouseOrders", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
        }
    }
}
