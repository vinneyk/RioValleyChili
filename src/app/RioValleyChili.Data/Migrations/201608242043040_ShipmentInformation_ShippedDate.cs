namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentInformation_ShippedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentInformation", "ShippedDate", c => c.DateTime());
//            Sql(@"UPDATE dbo.ShipmentInformation
//SET ShippedDate = ShipmentDate
//WHERE ShipmentInformation.DateCreated = ShipmentInformation.DateCreated AND Sequence = Sequence");
            
            DropColumn("dbo.InventoryShipmentOrders", "ShipmentDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InventoryShipmentOrders", "ShipmentDate", c => c.DateTime(nullable: false, storeType: "date"));
            Sql(@"UPDATE dbo.ShipmentInformation
                SET ShipmentDate = ShippedDate
                WHERE ShipmentInformation.DateCreated = ShipmentInformation.DateCreated AND Sequence = Sequence");
            DropColumn("dbo.ShipmentInformation", "ShippedDate");
        }
    }
}
