namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileProductAttributeRange_EmployeeIdentifiable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChileProductAttributeRanges", "EmployeeId", c => c.Int(nullable: false, defaultValue: 100));
            CreateIndex("dbo.ChileProductAttributeRanges", "EmployeeId");
            AddForeignKey("dbo.ChileProductAttributeRanges", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.ChileProductAttributeRanges", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChileProductAttributeRanges", "User", c => c.String(nullable: false, maxLength: 25, defaultValue: "DataInitialize"));
            DropForeignKey("dbo.ChileProductAttributeRanges", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.ChileProductAttributeRanges", new[] { "EmployeeId" });
            DropColumn("dbo.ChileProductAttributeRanges", "EmployeeId");
        }
    }
}
