namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerProductSpecs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerProductAttributeRanges", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerProductAttributeRanges", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerProductAttributeRanges", "TimeStamp", c => c.DateTime(nullable: false));
            CreateIndex("dbo.CustomerProductAttributeRanges", "EmployeeId");
            AddForeignKey("dbo.CustomerProductAttributeRanges", "EmployeeId", "dbo.Employees", "EmployeeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerProductAttributeRanges", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.CustomerProductAttributeRanges", new[] { "EmployeeId" });
            DropColumn("dbo.CustomerProductAttributeRanges", "TimeStamp");
            DropColumn("dbo.CustomerProductAttributeRanges", "EmployeeId");
            DropColumn("dbo.CustomerProductAttributeRanges", "Active");
        }
    }
}
