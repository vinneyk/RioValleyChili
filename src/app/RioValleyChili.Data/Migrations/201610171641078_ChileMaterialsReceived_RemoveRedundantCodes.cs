namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileMaterialsReceived_RemoveRedundantCodes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ChileMaterialsReceived", "PurchaseOrder");
            DropColumn("dbo.ChileMaterialsReceived", "ShipperNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChileMaterialsReceived", "ShipperNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.ChileMaterialsReceived", "PurchaseOrder", c => c.String(maxLength: 20));
        }
    }
}
