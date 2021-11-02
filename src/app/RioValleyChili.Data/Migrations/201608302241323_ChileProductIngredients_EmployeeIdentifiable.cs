namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileProductIngredients_EmployeeIdentifiable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChileProductIngredients", "EmployeeId", c => c.Int(nullable: false, defaultValue: 100));
            CreateIndex("dbo.ChileProductIngredients", "EmployeeId");
            AddForeignKey("dbo.ChileProductIngredients", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.ChileProductIngredients", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChileProductIngredients", "User", c => c.String(nullable: false, maxLength: 25, defaultValue: "DataInitialize"));
            DropForeignKey("dbo.ChileProductIngredients", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.ChileProductIngredients", new[] { "EmployeeId" });
            DropColumn("dbo.ChileProductIngredients", "EmployeeId");
        }
    }
}
