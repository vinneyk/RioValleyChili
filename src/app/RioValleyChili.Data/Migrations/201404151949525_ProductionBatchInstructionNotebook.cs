namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductionBatchInstructionNotebook : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches");
            DropForeignKey("dbo.ProductionBatchInstructionReferences", "InstructionId", "dbo.Instructions");
            DropIndex("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            DropIndex("dbo.ProductionBatchInstructionReferences", new[] { "InstructionId" });
            AddColumn("dbo.ProductionBatches", "InstructionNotebookDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.ProductionBatches", "InstructionNotebookSequence", c => c.Int(nullable: false));
            AddForeignKey("dbo.ProductionBatches", new[] { "InstructionNotebookDateCreated", "InstructionNotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            CreateIndex("dbo.ProductionBatches", new[] { "InstructionNotebookDateCreated", "InstructionNotebookSequence" });
            DropTable("dbo.ProductionBatchInstructionReferences");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductionBatchInstructionReferences",
                c => new
                    {
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        PickedInventoryDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PickedInventorySequence = c.Int(nullable: false),
                        InstructionOrder = c.Int(nullable: false),
                        InstructionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence, t.PickedInventoryDateCreated, t.PickedInventorySequence, t.InstructionOrder });
            
            DropIndex("dbo.ProductionBatches", new[] { "InstructionNotebookDateCreated", "InstructionNotebookSequence" });
            DropForeignKey("dbo.ProductionBatches", new[] { "InstructionNotebookDateCreated", "InstructionNotebookSequence" }, "dbo.Notebooks");
            DropColumn("dbo.ProductionBatches", "InstructionNotebookSequence");
            DropColumn("dbo.ProductionBatches", "InstructionNotebookDateCreated");
            CreateIndex("dbo.ProductionBatchInstructionReferences", "InstructionId");
            CreateIndex("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" });
            AddForeignKey("dbo.ProductionBatchInstructionReferences", "InstructionId", "dbo.Instructions", "Id");
            AddForeignKey("dbo.ProductionBatchInstructionReferences", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, "dbo.ProductionBatches", new[] { "PackScheduleDateCreated", "PackScheduleSequence", "PickedInventoryDateCreated", "PickedInventorySequence" }, cascadeDelete: true);
        }
    }
}
