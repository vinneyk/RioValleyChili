namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Lot_AttributeDefects : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
        }
    }
}
