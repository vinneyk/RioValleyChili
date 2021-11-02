namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotProductSpecStatuses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "ProductSpecComplete", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lots", "ProductSpecOutOfRange", c => c.Boolean(nullable: false));
            DropColumn("dbo.Lots", "ProductSpecStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lots", "ProductSpecStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Lots", "ProductSpecOutOfRange");
            DropColumn("dbo.Lots", "ProductSpecComplete");
        }
    }
}
