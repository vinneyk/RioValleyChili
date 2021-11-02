namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EmployeesTable_NonReferential : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 15),
                        DisplayName = c.String(nullable: false, maxLength: 25),
                        EmailAddress = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId);

            CreateIndex("dbo.Employees", "UserName", unique: true);
        }
        
        public override void Down()
        {
            DropTable("dbo.Employees");
        }
    }
}
