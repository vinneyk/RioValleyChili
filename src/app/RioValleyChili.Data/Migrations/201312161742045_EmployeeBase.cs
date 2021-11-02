namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MillAndWetdownEntries", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.DehydratedMaterialsReceived", "EmployeeId", c => c.Int(nullable: false));
            AddForeignKey("dbo.MillAndWetdownEntries", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.DehydratedMaterialsReceived", "EmployeeId", "dbo.Employees", "EmployeeId");
            CreateIndex("dbo.MillAndWetdownEntries", "EmployeeId");
            CreateIndex("dbo.DehydratedMaterialsReceived", "EmployeeId");
            DropColumn("dbo.MillAndWetdownEntries", "User");
            DropColumn("dbo.DehydratedMaterialsReceived", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DehydratedMaterialsReceived", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.MillAndWetdownEntries", "User", c => c.String(nullable: false, maxLength: 25));
            DropIndex("dbo.DehydratedMaterialsReceived", new[] { "EmployeeId" });
            DropIndex("dbo.MillAndWetdownEntries", new[] { "EmployeeId" });
            DropForeignKey("dbo.DehydratedMaterialsReceived", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.MillAndWetdownEntries", "EmployeeId", "dbo.Employees");
            DropColumn("dbo.DehydratedMaterialsReceived", "EmployeeId");
            DropColumn("dbo.MillAndWetdownEntries", "EmployeeId");
        }
    }
}
