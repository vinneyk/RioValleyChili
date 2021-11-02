namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotQualityControlNotebook : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "QualityControlNotebookSequence", c => c.Int(nullable: false));
            AddForeignKey("dbo.Lots", new[] { "DateCreated", "QualityControlNotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            CreateIndex("dbo.Lots", new[] { "DateCreated", "QualityControlNotebookSequence" });
        }
        
        public override void Down()
        {
            DropIndex("dbo.Lots", new[] { "DateCreated", "QualityControlNotebookSequence" });
            DropForeignKey("dbo.Lots", new[] { "DateCreated", "QualityControlNotebookSequence" }, "dbo.Notebooks");
            DropColumn("dbo.Lots", "QualityControlNotebookSequence");
        }
    }
}
