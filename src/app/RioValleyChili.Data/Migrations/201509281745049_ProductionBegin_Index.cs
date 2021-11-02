namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductionBegin_Index : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.LotProductionResults", "ProductionBegin");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LotProductionResults", new[] { "ProductionBegin" });
        }
    }
}
