namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Lot_InventoryReceived : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "VendorId", c => c.Int());
            AddColumn("dbo.Lots", "PurchaseOrderNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.Lots", "ShipperNumber", c => c.String(maxLength: 50));
            CreateIndex("dbo.Lots", "VendorId");
            AddForeignKey("dbo.Lots", "VendorId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lots", "VendorId", "dbo.Companies");
            DropIndex("dbo.Lots", new[] { "VendorId" });
            DropColumn("dbo.Lots", "ShipperNumber");
            DropColumn("dbo.Lots", "PurchaseOrderNumber");
            DropColumn("dbo.Lots", "VendorId");
        }
    }
}
