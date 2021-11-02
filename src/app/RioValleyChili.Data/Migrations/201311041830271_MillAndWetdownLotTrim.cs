namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MillAndWetdownLotTrim : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropIndex("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropColumn("dbo.MillAndWetdownResultItems", "LotDateCreated");
            DropColumn("dbo.MillAndWetdownResultItems", "LotDateSequence");
            DropColumn("dbo.MillAndWetdownResultItems", "LotTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MillAndWetdownResultItems", "LotTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.MillAndWetdownResultItems", "LotDateSequence", c => c.Int(nullable: false));
            AddColumn("dbo.MillAndWetdownResultItems", "LotDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            CreateIndex("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.MillAndWetdownResultItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
        }
    }
}
