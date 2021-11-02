namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class IntraWarehouseOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IntraWarehouseOrders", "RinconID", c => c.DateTime());
            AddColumn("dbo.IntraWarehouseOrders", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("dbo.IntraWarehouseOrders", "TrackingSheetNumber", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.IntraWarehouseOrders", "EmployeeId");
            AddForeignKey("dbo.IntraWarehouseOrders", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.IntraWarehouseOrders", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IntraWarehouseOrders", "User", c => c.String(nullable: false, maxLength: 25));
            DropForeignKey("dbo.IntraWarehouseOrders", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.IntraWarehouseOrders", new[] { "EmployeeId" });
            AlterColumn("dbo.IntraWarehouseOrders", "TrackingSheetNumber", c => c.String(maxLength: 20));
            DropColumn("dbo.IntraWarehouseOrders", "EmployeeId");
            DropColumn("dbo.IntraWarehouseOrders", "RinconID");
        }
    }
}
