namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrder_PreShipmentSampleRequired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "PreShipmentSampleRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "PreShipmentSampleRequired");
        }
    }
}
