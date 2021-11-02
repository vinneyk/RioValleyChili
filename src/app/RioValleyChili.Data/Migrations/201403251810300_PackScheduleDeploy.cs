namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PackScheduleDeploy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" }, "dbo.PackSchedules");
            DropIndex("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" });
            AddColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetWeight", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetAsta", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetScoville", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetScan", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionBatches", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetWeight", c => c.Double(nullable: false));
            AddColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetAsta", c => c.Double(nullable: false));
            AddColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetScoville", c => c.Double(nullable: false));
            AddColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetScan", c => c.Double(nullable: false));
            AddColumn("dbo.PackSchedules", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.PickedInventory", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.PickedInventory", "TimeStamp", c => c.DateTime(nullable: false));
            AddForeignKey("dbo.ProductionBatches", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.PackSchedules", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.PickedInventory", "EmployeeId", "dbo.Employees", "EmployeeId");
            CreateIndex("dbo.ProductionBatches", "EmployeeId");
            CreateIndex("dbo.PackSchedules", "EmployeeId");
            CreateIndex("dbo.PickedInventory", "EmployeeId");
            DropColumn("dbo.ProductionBatches", "NumberOfPackagingUnits");
            DropColumn("dbo.ProductionBatches", "User");
            DropColumn("dbo.PackSchedules", "UnitsToProduce");
            DropColumn("dbo.PackSchedules", "User");
            DropTable("dbo.PackScheduleTargetParameters");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PackScheduleTargetParameters",
                c => new
                    {
                        PackScheduleDate = c.DateTime(nullable: false, storeType: "date"),
                        PackScheduleSequence = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Asta = c.Int(nullable: false),
                        Scoville = c.Int(nullable: false),
                        Scan = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackScheduleDate, t.PackScheduleSequence });
            
            AddColumn("dbo.PackSchedules", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.PackSchedules", "UnitsToProduce", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionBatches", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.ProductionBatches", "NumberOfPackagingUnits", c => c.Int(nullable: false));
            DropIndex("dbo.PickedInventory", new[] { "EmployeeId" });
            DropIndex("dbo.PackSchedules", new[] { "EmployeeId" });
            DropIndex("dbo.ProductionBatches", new[] { "EmployeeId" });
            DropForeignKey("dbo.PickedInventory", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.PackSchedules", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ProductionBatches", "EmployeeId", "dbo.Employees");
            DropColumn("dbo.PickedInventory", "TimeStamp");
            DropColumn("dbo.PickedInventory", "EmployeeId");
            DropColumn("dbo.PackSchedules", "EmployeeId");
            DropColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetScan");
            DropColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetScoville");
            DropColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetAsta");
            DropColumn("dbo.PackSchedules", "DefaultBatchTargetParameters_BatchTargetWeight");
            DropColumn("dbo.ProductionBatches", "EmployeeId");
            DropColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetScan");
            DropColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetScoville");
            DropColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetAsta");
            DropColumn("dbo.ProductionBatches", "TargetParameters_BatchTargetWeight");
            CreateIndex("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" });
            AddForeignKey("dbo.PackScheduleTargetParameters", new[] { "PackScheduleDate", "PackScheduleSequence" }, "dbo.PackSchedules", new[] { "DateCreated", "SequentialNumber" }, cascadeDelete: true);
        }
    }
}
