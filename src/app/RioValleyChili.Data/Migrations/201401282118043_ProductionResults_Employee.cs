namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductionResults_Employee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionResults", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionResults", "TimeStamp", c => c.DateTime(nullable: false));
            AddForeignKey("dbo.ProductionResults", "EmployeeId", "dbo.Employees", "EmployeeId");
            CreateIndex("dbo.ProductionResults", "EmployeeId");
            DropColumn("dbo.ProductionResults", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionResults", "User", c => c.String(nullable: false, maxLength: 25));
            DropIndex("dbo.ProductionResults", new[] { "EmployeeId" });
            DropForeignKey("dbo.ProductionResults", "EmployeeId", "dbo.Employees");
            DropColumn("dbo.ProductionResults", "TimeStamp");
            DropColumn("dbo.ProductionResults", "EmployeeId");
        }
    }
}
