namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AttributeDateIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.LotAttributes", "AttributeDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LotAttributes", new[] { "AttributeDate" });
        }
    }
}
