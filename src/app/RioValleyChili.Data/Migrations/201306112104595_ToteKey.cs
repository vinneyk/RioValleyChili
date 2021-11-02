namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ToteKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventory", "ToteKey", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.PickedInventoryItems", "ToteKey", c => c.String(maxLength: 15));
            AddColumn("dbo.InventoryAdjustments", "ToteKey", c => c.String(maxLength: 15));
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "ToteKey", c => c.String(maxLength: 15));
            DropPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "WarehouseLocationId", "TreatmentId" });
            AddPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "WarehouseLocationId", "TreatmentId", "ToteKey" });
            DropColumn("dbo.PickedInventoryItems", "Tote");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "Tote");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "Tote", c => c.Int(nullable: false));
            AddColumn("dbo.PickedInventoryItems", "Tote", c => c.Int());
            DropPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "WarehouseLocationId", "TreatmentId", "ToteKey" });
            AddPrimaryKey("dbo.Inventory", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "PackagingProductId", "WarehouseLocationId", "TreatmentId" });
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "ToteKey");
            DropColumn("dbo.InventoryAdjustments", "ToteKey");
            DropColumn("dbo.PickedInventoryItems", "ToteKey");
            DropColumn("dbo.Inventory", "ToteKey");
        }
    }
}
