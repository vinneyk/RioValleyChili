namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PalletWeight_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShipmentInformation", "PalletWeight", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShipmentInformation", "PalletWeight", c => c.Double(nullable: false));
        }
    }
}
