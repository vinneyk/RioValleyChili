namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotProductSpecStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "ProductSpecStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lots", "ProductSpecStatus");
        }
    }
}
