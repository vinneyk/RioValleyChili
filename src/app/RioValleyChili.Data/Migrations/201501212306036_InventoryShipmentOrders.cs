namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InventoryShipmentOrders : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropIndex("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            CreateTable(
                "dbo.InventoryShipmentOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ShipmentInfoDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        ShipmentInfoSequence = c.Int(nullable: false),
                        OrderType = c.Short(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.InventoryPickOrders", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.ShipmentInformation", t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence });
            
            AddForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" });
            DropColumn("dbo.CustomerOrders", "ShipmentInfoDateCreated");
            DropColumn("dbo.CustomerOrders", "ShipmentInfoSequence");
            DropColumn("dbo.InterWarehouseOrders", "ShipmentInfoDateCreated");
            DropColumn("dbo.InterWarehouseOrders", "ShipmentInfoSequence");
            DropColumn("dbo.TreatmentOrders", "ShipmentInfoDateCreated");
            DropColumn("dbo.TreatmentOrders", "ShipmentInfoSequence");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TreatmentOrders", "ShipmentInfoSequence", c => c.Int(nullable: false));
            AddColumn("dbo.TreatmentOrders", "ShipmentInfoDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.InterWarehouseOrders", "ShipmentInfoSequence", c => c.Int(nullable: false));
            AddColumn("dbo.InterWarehouseOrders", "ShipmentInfoDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.CustomerOrders", "ShipmentInfoSequence", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "ShipmentInfoDateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            DropForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders");
            DropForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders");
            DropForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryShipmentOrders");
            DropForeignKey("dbo.InventoryShipmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropIndex("dbo.InventoryShipmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.InventoryShipmentOrders", new[] { "DateCreated", "Sequence" });
            DropTable("dbo.InventoryShipmentOrders");
            CreateIndex("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            CreateIndex("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            CreateIndex("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            AddForeignKey("dbo.TreatmentOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.TreatmentOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InterWarehouseOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.InterWarehouseOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory", new[] { "DateCreated", "Sequence" });
            AddForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders", new[] { "DateCreated", "Sequence" });
        }
    }
}
