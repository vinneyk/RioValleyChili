namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PackagingWeights : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PackagingProducts", "PackagingWeight", c => c.Double(nullable: false));
            AddColumn("dbo.PackagingProducts", "PalletWeight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PackagingProducts", "PalletWeight");
            DropColumn("dbo.PackagingProducts", "PackagingWeight");
        }
    }
}
