namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotHold : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LotStatus", "StatusNameId", "dbo.LotStatusNames");
            DropForeignKey("dbo.LotStatus", "StatusValueId", "dbo.LotStatusValues");
            DropForeignKey("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropIndex("dbo.LotStatus", new[] { "StatusNameId" });
            DropIndex("dbo.LotStatus", new[] { "StatusValueId" });
            DropIndex("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddColumn("dbo.Lots", "Hold", c => c.Int());
            AddColumn("dbo.Lots", "HoldDescription", c => c.String(maxLength: 50));
            DropColumn("dbo.Lots", "OnHold");
            DropTable("dbo.LotStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LotStatus",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        StatusNameId = c.Int(nullable: false),
                        StatusValueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.StatusNameId });
            
            AddColumn("dbo.Lots", "OnHold", c => c.Boolean(nullable: false));
            DropColumn("dbo.Lots", "HoldDescription");
            DropColumn("dbo.Lots", "Hold");
            CreateIndex("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            CreateIndex("dbo.LotStatus", "StatusValueId");
            CreateIndex("dbo.LotStatus", "StatusNameId");
            AddForeignKey("dbo.LotStatus", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "DateCreated", "DateSequence", "LotTypeId" });
            AddForeignKey("dbo.LotStatus", "StatusValueId", "dbo.LotStatusValues", "Id");
            AddForeignKey("dbo.LotStatus", "StatusNameId", "dbo.LotStatusNames", "Id");
        }
    }
}
