namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShippingLabelName_Length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerOrders", "SoldTo_Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.ShipmentInformation", "ShipFrom_Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.ShipmentInformation", "ShipTo_Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.ShipmentInformation", "FreightBill_Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShipmentInformation", "FreightBill_Name", c => c.String(maxLength: 25));
            AlterColumn("dbo.ShipmentInformation", "ShipTo_Name", c => c.String(maxLength: 25));
            AlterColumn("dbo.ShipmentInformation", "ShipFrom_Name", c => c.String(maxLength: 25));
            AlterColumn("dbo.CustomerOrders", "SoldTo_Name", c => c.String(maxLength: 25));
        }
    }
}
