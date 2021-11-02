namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Lot_Notes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lots", "FK_dbo.Lots_dbo.Notebooks_DateCreated_QualityControlNotebookSequence");
            DropIndex("dbo.Lots", "IX_DateCreated_QualityControlNotebookSequence");
            AddColumn("dbo.Lots", "Notes", c => c.String(maxLength: 255));
            DropColumn("dbo.Lots", "QualityControlNotebookSequence");
            DropColumn("dbo.ProductionBatches", "Notes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionBatches", "Notes", c => c.String(maxLength: 255));
            AddColumn("dbo.Lots", "QualityControlNotebookSequence", c => c.Int(nullable: false));
            DropColumn("dbo.Lots", "Notes");
            AddForeignKey("dbo.Lots", new[] { "LotDateCreated", "QualityControlNotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" }, false, "FK_dbo.Lots_dbo.Notebooks_DateCreated_QualityControlNotebookSequence");
            CreateIndex("dbo.Lots", new[] { "LotDateCreated", "QualityControlNotebookSequence" }, false, "IX_DateCreated_QualityControlNotebookSequence");
        }
    }
}
