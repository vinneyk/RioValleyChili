namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveChileProductAttributes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChileProductAttributes", "ChileProductId", "dbo.ChileProducts");
            DropIndex("dbo.ChileProductAttributes", new[] { "ChileProductId" });
            DropTable("dbo.ChileProductAttributes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ChileProductAttributes",
                c => new
                    {
                        ChileProductId = c.Int(nullable: false),
                        MinAsta = c.Int(nullable: false),
                        MaxAsta = c.Int(nullable: false),
                        MinScoville = c.Int(nullable: false),
                        MaxScoville = c.Int(nullable: false),
                        MinScan = c.Int(nullable: false),
                        MaxScan = c.Int(nullable: false),
                        MinGranulation = c.Int(nullable: false),
                        MaxGranulation = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChileProductId);
            
            CreateIndex("dbo.ChileProductAttributes", "ChileProductId");
            AddForeignKey("dbo.ChileProductAttributes", "ChileProductId", "dbo.ChileProducts", "Id");
        }
    }
}
