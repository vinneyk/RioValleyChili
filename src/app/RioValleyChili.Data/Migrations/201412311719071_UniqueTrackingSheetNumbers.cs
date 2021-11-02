namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UniqueTrackingSheetNumbers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IntraWarehouseOrders", "TrackingSheetNumber", c => c.Decimal(nullable: false, storeType: "money"));
            CreateIndex("dbo.IntraWarehouseOrders", "TrackingSheetNumber", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.IntraWarehouseOrders", new[] { "TrackingSheetNumber" });
            AlterColumn("dbo.IntraWarehouseOrders", "TrackingSheetNumber", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
