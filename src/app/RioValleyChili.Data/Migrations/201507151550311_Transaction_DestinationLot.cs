namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Transaction_DestinationLot : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.InventoryTransactions", name: "LotDateCreated", newName: "SourceLotDateCreated");
            RenameColumn(table: "dbo.InventoryTransactions", name: "LotDateSequence", newName: "SourceLotDateSequence");
            RenameColumn(table: "dbo.InventoryTransactions", name: "LotTypeId", newName: "SourceLotTypeId");
            RenameIndex(table: "dbo.InventoryTransactions", name: "IX_LotDateCreated_LotDateSequence_LotTypeId", newName: "IX_SourceLotDateCreated_SourceLotDateSequence_SourceLotTypeId");
            AddColumn("dbo.InventoryTransactions", "DestinationLotDateCreated", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.InventoryTransactions", "DestinationLotDateSequence", c => c.Int());
            AddColumn("dbo.InventoryTransactions", "DestinationLotTypeId", c => c.Int());
            CreateIndex("dbo.InventoryTransactions", new[] { "DestinationLotDateCreated", "DestinationLotDateSequence", "DestinationLotTypeId" });
            AddForeignKey("dbo.InventoryTransactions", new[] { "DestinationLotDateCreated", "DestinationLotDateSequence", "DestinationLotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryTransactions", new[] { "DestinationLotDateCreated", "DestinationLotDateSequence", "DestinationLotTypeId" }, "dbo.Lots");
            DropIndex("dbo.InventoryTransactions", new[] { "DestinationLotDateCreated", "DestinationLotDateSequence", "DestinationLotTypeId" });
            DropColumn("dbo.InventoryTransactions", "DestinationLotTypeId");
            DropColumn("dbo.InventoryTransactions", "DestinationLotDateSequence");
            DropColumn("dbo.InventoryTransactions", "DestinationLotDateCreated");
            RenameIndex(table: "dbo.InventoryTransactions", name: "IX_SourceLotDateCreated_SourceLotDateSequence_SourceLotTypeId", newName: "IX_LotDateCreated_LotDateSequence_LotTypeId");
            RenameColumn(table: "dbo.InventoryTransactions", name: "SourceLotTypeId", newName: "LotTypeId");
            RenameColumn(table: "dbo.InventoryTransactions", name: "SourceLotDateSequence", newName: "LotDateSequence");
            RenameColumn(table: "dbo.InventoryTransactions", name: "SourceLotDateCreated", newName: "LotDateCreated");
        }
    }
}
