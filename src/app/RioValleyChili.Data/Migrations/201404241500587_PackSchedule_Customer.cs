namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PackSchedule_Customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionBatches", "Notes", c => c.String(maxLength: 255));
            AddColumn("dbo.PackSchedules", "OrderNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.PackSchedules", "CustomerId", c => c.Int());
            AddForeignKey("dbo.PackSchedules", "CustomerId", "dbo.Customers", "Id");
            CreateIndex("dbo.PackSchedules", "CustomerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PackSchedules", new[] { "CustomerId" });
            DropForeignKey("dbo.PackSchedules", "CustomerId", "dbo.Customers");
            DropColumn("dbo.PackSchedules", "CustomerId");
            DropColumn("dbo.PackSchedules", "OrderNumber");
            DropColumn("dbo.ProductionBatches", "Notes");
        }
    }
}
