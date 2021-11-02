namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentInformation_ShipmentDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentInformation", "ShipmentDate", c => c.DateTime());
//            Sql(@"UPDATE dbo.ShipmentInformation
//SET ShipmentDate = ShippedDate
//WHERE ShipmentInformation.DateCreated = ShipmentInformation.DateCreated AND Sequence = Sequence");

            DropColumn("dbo.ShipmentInformation", "ScheduledShipDate");
            DropColumn("dbo.ShipmentInformation", "ShippedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentInformation", "ShippedDate", c => c.DateTime());
            AddColumn("dbo.ShipmentInformation", "ScheduledShipDate", c => c.DateTime());
            Sql(@"UPDATE dbo.ShipmentInformation
                SET ShippedDate = ShipmentDate, ScheduledShipDate = ShipmentDate
                WHERE ShipmentInformation.DateCreated = ShipmentInformation.DateCreated AND Sequence = Sequence");
            DropColumn("dbo.ShipmentInformation", "ShipmentDate");
        }
    }
}
