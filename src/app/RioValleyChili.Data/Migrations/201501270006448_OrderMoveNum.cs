namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class OrderMoveNum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryShipmentOrders", "MoveNum", c => c.Int());
            DropColumn("dbo.CustomerOrders", "OrderNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "OrderNum", c => c.Int());
            DropColumn("dbo.InventoryShipmentOrders", "MoveNum");
        }
    }
}
