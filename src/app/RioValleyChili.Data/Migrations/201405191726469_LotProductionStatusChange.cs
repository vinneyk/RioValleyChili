namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotProductionStatusChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "QualityStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Lots", "LotStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lots", "LotStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Lots", "QualityStatus");
        }
    }
}
