namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotHistories",
                c => new
                    {
                        LotDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        LotDateSequence = c.Int(nullable: false),
                        LotTypeId = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Serialized = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId, t.Sequence })
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Lots", t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => new { t.LotDateCreated, t.LotDateSequence, t.LotTypeId })
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotHistories", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" }, "dbo.Lots");
            DropForeignKey("dbo.LotHistories", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.LotHistories", new[] { "EmployeeId" });
            DropIndex("dbo.LotHistories", new[] { "LotDateCreated", "LotDateSequence", "LotTypeId" });
            DropTable("dbo.LotHistories");
        }
    }
}
