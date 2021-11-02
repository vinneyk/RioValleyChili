namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLotAttributeNavFromLotAttributeDefect : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" }, "dbo.LotAttributes");
            DropIndex("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" });
            AddForeignKey("dbo.LotAttributeDefects", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" }, "dbo.LotAttributes", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId", "AttributeShortName" });
        }
    }
}
