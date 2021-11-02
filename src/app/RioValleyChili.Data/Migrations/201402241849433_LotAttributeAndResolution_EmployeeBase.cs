namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotAttributeAndResolution_EmployeeBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LotAttributes", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.LotAttributes", "TimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.LotDefectResolutions", "EmployeeId", c => c.Int(nullable: false));
            AddForeignKey("dbo.LotAttributes", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.LotDefectResolutions", "EmployeeId", "dbo.Employees", "EmployeeId");
            CreateIndex("dbo.LotAttributes", "EmployeeId");
            CreateIndex("dbo.LotDefectResolutions", "EmployeeId");
            DropColumn("dbo.LotDefectResolutions", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LotDefectResolutions", "User", c => c.String(nullable: false, maxLength: 25));
            DropIndex("dbo.LotDefectResolutions", new[] { "EmployeeId" });
            DropIndex("dbo.LotAttributes", new[] { "EmployeeId" });
            DropForeignKey("dbo.LotDefectResolutions", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.LotAttributes", "EmployeeId", "dbo.Employees");
            DropColumn("dbo.LotDefectResolutions", "EmployeeId");
            DropColumn("dbo.LotAttributes", "TimeStamp");
            DropColumn("dbo.LotAttributes", "EmployeeId");
        }
    }
}
