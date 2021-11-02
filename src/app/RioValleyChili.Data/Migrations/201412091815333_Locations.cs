namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Locations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WarehouseLocationTransitions", "WarehouseLocationId", "dbo.WarehouseLocations");
            DropIndex("dbo.WarehouseLocationTransitions", new[] { "WarehouseLocationId" });
            DropIndex("dbo.CustomerOrders", new[] { "BrokerId" });
            AddColumn("dbo.Locations", "LocID", c => c.Int());
            AlterColumn("dbo.CustomerOrders", "BrokerId", c => c.Int());
            CreateIndex("dbo.CustomerOrders", "BrokerId");
            DropTable("dbo.WarehouseLocationTransitions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WarehouseLocationTransitions",
                c => new
                    {
                        WarehouseLocationId = c.Int(nullable: false),
                        OldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WarehouseLocationId);
            
            DropIndex("dbo.CustomerOrders", new[] { "BrokerId" });
            AlterColumn("dbo.CustomerOrders", "BrokerId", c => c.Int(nullable: false));
            DropColumn("dbo.Locations", "LocID");
            CreateIndex("dbo.CustomerOrders", "BrokerId");
            CreateIndex("dbo.WarehouseLocationTransitions", "WarehouseLocationId");
            AddForeignKey("dbo.WarehouseLocationTransitions", "WarehouseLocationId", "dbo.WarehouseLocations", "Id");
        }
    }
}
