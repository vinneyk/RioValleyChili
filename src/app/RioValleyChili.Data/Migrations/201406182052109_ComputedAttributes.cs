namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ComputedAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LotAttributes", "AttributeDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.LotAttributes", "Computed", c => c.Boolean(nullable: false));
            DropColumn("dbo.LotAttributes", "DateEntered");
            DropColumn("dbo.LotAttributes", "DateTested");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LotAttributes", "DateTested", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.LotAttributes", "DateEntered", c => c.DateTime(nullable: false));
            DropColumn("dbo.LotAttributes", "Computed");
            DropColumn("dbo.LotAttributes", "AttributeDate");
        }
    }
}
