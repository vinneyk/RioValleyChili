namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotPackaging : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lots", "ReceivedPackagingProductId", c => c.Int(nullable: false));
            AddColumn("dbo.Lots", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.Lots", "TimeStamp", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Lots", "ReceivedPackagingProductId");
            CreateIndex("dbo.Lots", "EmployeeId");
            AddForeignKey("dbo.Lots", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.Lots", "ReceivedPackagingProductId", "dbo.PackagingProducts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lots", "ReceivedPackagingProductId", "dbo.PackagingProducts");
            DropForeignKey("dbo.Lots", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Lots", new[] { "EmployeeId" });
            DropIndex("dbo.Lots", new[] { "ReceivedPackagingProductId" });
            DropColumn("dbo.Lots", "TimeStamp");
            DropColumn("dbo.Lots", "EmployeeId");
            DropColumn("dbo.Lots", "ReceivedPackagingProductId");
        }
    }
}
