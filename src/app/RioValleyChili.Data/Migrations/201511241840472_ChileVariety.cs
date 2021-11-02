namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileVariety : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "ChileVariety", c => c.String(maxLength: 25));
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "ChileVariety");

            Sql(@"UPDATE DehydratedMaterialsReceivedItems
                    SET ChileVariety = ChileVarieties.Description
                    FROM DehydratedMaterialsReceivedItems INNER JOIN
                        ChileVarieties ON DehydratedMaterialsReceivedItems.ChileVarietyId = ChileVarieties.Id");

            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId", "dbo.ChileVarieties");
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "ChileVarietyId" });
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId");
            DropTable("dbo.ChileVarieties");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ChileVarieties",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId", c => c.Int(nullable: false));
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "ChileVariety" });
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "ChileVariety");
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId");
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "ChileVarietyId", "dbo.ChileVarieties", "Id");
        }
    }
}
