namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SampleOrders_GenericProduct : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots");
            DropForeignKey("dbo.SampleOrderItems", "ChileProductId", "dbo.ChileProducts");
            DropIndex("dbo.SampleOrderItems", new[] { "ChileProductId" });
            AddColumn("dbo.SampleOrderItems", "ProductId", c => c.Int());
            CreateIndex("dbo.SampleOrderItems", "ProductId");
            AddForeignKey("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            AddForeignKey("dbo.SampleOrderItems", "ProductId", "dbo.Products", "Id");

            Sql(@"UPDATE dbo.SampleOrderItems SET ProductId = ChileProductId");

            DropColumn("dbo.SampleOrderItems", "ChileProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SampleOrderItems", "ChileProductId", c => c.Int());

            Sql(@"UPDATE dbo.SampleOrderItems SET ChileProductId = ProductId");

            DropForeignKey("dbo.SampleOrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropIndex("dbo.SampleOrderItems", new[] { "ProductId" });
            DropColumn("dbo.SampleOrderItems", "ProductId");
            CreateIndex("dbo.SampleOrderItems", "ChileProductId");
            AddForeignKey("dbo.SampleOrderItems", "ChileProductId", "dbo.ChileProducts", "Id");
            AddForeignKey("dbo.SampleOrderItems", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.ChileLots", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
        }
    }
}
