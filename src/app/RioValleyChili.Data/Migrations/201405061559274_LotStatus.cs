namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "LotStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Lots", "Contaminated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lots", "Contaminated", c => c.Boolean(nullable: false));
            DropColumn("dbo.Lots", "LotStatus");
        }
    }
}
