namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeClaims : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Claims", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Claims");
        }
    }
}
