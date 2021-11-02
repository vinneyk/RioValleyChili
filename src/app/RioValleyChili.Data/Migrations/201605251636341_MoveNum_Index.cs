namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MoveNum_Index : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.InventoryShipmentOrders", "MoveNum");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InventoryShipmentOrders", new[] { "MoveNum" });
        }
    }
}
