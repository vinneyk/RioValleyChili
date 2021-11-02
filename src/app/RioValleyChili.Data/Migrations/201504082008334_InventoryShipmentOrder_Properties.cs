namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InventoryShipmentOrder_Properties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryShipmentOrders", "PurchaseOrderNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.InventoryShipmentOrders", "ShipmentDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.InventoryShipmentOrders", "DateReceived", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.InventoryShipmentOrders", "RequestedBy", c => c.String(maxLength: 60));
            AddColumn("dbo.InventoryShipmentOrders", "TakenBy", c => c.String(maxLength: 60));
            DropColumn("dbo.CustomerOrders", "OrderTakenBy");
            DropColumn("dbo.CustomerOrders", "DateOrderReceived");
            DropColumn("dbo.CustomerOrders", "CustomerPurchaseOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "CustomerPurchaseOrder", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerOrders", "DateOrderReceived", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.CustomerOrders", "OrderTakenBy", c => c.String(maxLength: 25));
            DropColumn("dbo.InventoryShipmentOrders", "TakenBy");
            DropColumn("dbo.InventoryShipmentOrders", "RequestedBy");
            DropColumn("dbo.InventoryShipmentOrders", "DateReceived");
            DropColumn("dbo.InventoryShipmentOrders", "ShipmentDate");
            DropColumn("dbo.InventoryShipmentOrders", "PurchaseOrderNumber");
        }
    }
}
