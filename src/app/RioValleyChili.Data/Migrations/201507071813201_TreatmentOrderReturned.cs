namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TreatmentOrderReturned : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TreatmentOrders", "Returned", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TreatmentOrders", "Returned");
        }
    }
}
