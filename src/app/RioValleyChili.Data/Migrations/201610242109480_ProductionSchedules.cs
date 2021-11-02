namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductionSchedules : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScheduledInstructions", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropIndex("dbo.ScheduledInstructions", new[] { "InstructionId" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropIndex("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" });
            CreateTable(
                "dbo.ProductionScheduleItems",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        FlushBefore = c.Boolean(nullable: false),
                        FlushAfter = c.Boolean(nullable: false),
                        FlushBeforeInstructions = c.String(),
                        FlushAfterInstructions = c.String(),
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.Index })
                .ForeignKey("dbo.PackSchedules", t => new { t.PackScheduleDateCreated, t.PackScheduleSequence })
                .ForeignKey("dbo.ProductionSchedules", t => new { t.ProductionDate, t.ProductionLineLocationId })
                .Index(t => new { t.ProductionDate, t.ProductionLineLocationId })
                .Index(t => new { t.PackScheduleDateCreated, t.PackScheduleSequence });
            
            AddColumn("dbo.ProductionSchedules", "EmployeeId", c => c.Int(nullable: false, defaultValue: 100));
            CreateIndex("dbo.ProductionSchedules", "EmployeeId");
            AddForeignKey("dbo.ProductionSchedules", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.ProductionSchedules", "User");
            DropTable("dbo.ScheduledInstructions");
            DropTable("dbo.ScheduledPackSchedules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ScheduledPackSchedules",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        PackScheduleDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.PackScheduleDateCreated, t.PackScheduleSequence });
            
            CreateTable(
                "dbo.ScheduledInstructions",
                c => new
                    {
                        ProductionDate = c.DateTime(nullable: false, storeType: "date"),
                        ProductionLineLocationId = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        InstructionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionDate, t.ProductionLineLocationId, t.Sequence });
            
            AddColumn("dbo.ProductionSchedules", "User", c => c.String(nullable: false, maxLength: 25));
            DropForeignKey("dbo.ProductionScheduleItems", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules");
            DropForeignKey("dbo.ProductionSchedules", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ProductionScheduleItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropIndex("dbo.ProductionSchedules", new[] { "EmployeeId" });
            DropIndex("dbo.ProductionScheduleItems", new[] { "PackScheduleDateCreated", "PackScheduleSequence" });
            DropIndex("dbo.ProductionScheduleItems", new[] { "ProductionDate", "ProductionLineLocationId" });
            DropColumn("dbo.ProductionSchedules", "EmployeeId");
            DropTable("dbo.ProductionScheduleItems");
            CreateIndex("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" });
            CreateIndex("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            CreateIndex("dbo.ScheduledInstructions", "InstructionId");
            CreateIndex("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledPackSchedules", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledPackSchedules", new[] { "PackScheduleDateCreated", "PackScheduleSequence" }, "dbo.PackSchedules", new[] { "DateCreated", "SequentialNumber" });
            AddForeignKey("dbo.ScheduledInstructions", new[] { "ProductionDate", "ProductionLineLocationId" }, "dbo.ProductionSchedules", new[] { "ProductionDate", "ProductionLineLocationId" });
            AddForeignKey("dbo.ScheduledInstructions", "InstructionId", "dbo.Instructions", "Id");
        }
    }
}
