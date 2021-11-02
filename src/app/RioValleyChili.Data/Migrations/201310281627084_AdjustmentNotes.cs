namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AdjustmentNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryAdjustments", "NotebookDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.InventoryAdjustments", "NotebookSequence", c => c.Int(nullable: false));
            AddForeignKey("dbo.InventoryAdjustments", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            CreateIndex("dbo.InventoryAdjustments", new[] { "NotebookDate", "NotebookSequence" });
            DropColumn("dbo.InventoryAdjustments", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InventoryAdjustments", "Comment", c => c.String(maxLength: 50));
            DropIndex("dbo.InventoryAdjustments", new[] { "NotebookDate", "NotebookSequence" });
            DropForeignKey("dbo.InventoryAdjustments", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks");
            DropColumn("dbo.InventoryAdjustments", "NotebookSequence");
            DropColumn("dbo.InventoryAdjustments", "NotebookDate");
        }
    }
}
