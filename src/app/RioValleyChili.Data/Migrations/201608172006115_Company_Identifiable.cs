namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Company_Identifiable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "TimeStamp", c => c.DateTime(nullable: false));
            Sql(@"UPDATE dbo.Companies SET TimeStamp = GETUTCDATE()");
            Sql(@"UPDATE dbo.Companies SET EmployeeId = 100");
            CreateIndex("dbo.Companies", "EmployeeId");
            AddForeignKey("dbo.Companies", "EmployeeId", "dbo.Employees", "EmployeeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Companies", new[] { "EmployeeId" });
            DropColumn("dbo.Companies", "TimeStamp");
            DropColumn("dbo.Companies", "EmployeeId");
        }
    }
}
