namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrders",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        BrokerId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        ShipmentInfoDateCreated = c.DateTime(nullable: false, storeType: "date"),
                        ShipmentInfoSequence = c.Int(nullable: false),
                        OrderStatus = c.Short(nullable: false),
                        OrderTakenBy = c.String(maxLength: 25),
                        DateOrderReceived = c.DateTime(nullable: false, storeType: "date"),
                        PaymentTerms = c.String(maxLength: 25),
                        CustomerPurchaseOrder = c.String(maxLength: 10),
                        SoldTo_Name = c.String(maxLength: 25),
                        SoldTo_Address_AddressLine1 = c.String(maxLength: 50),
                        SoldTo_Address_AddressLine2 = c.String(maxLength: 50),
                        SoldTo_Address_AddressLine3 = c.String(maxLength: 50),
                        SoldTo_Address_City = c.String(maxLength: 75),
                        SoldTo_Address_State = c.String(maxLength: 50),
                        SoldTo_Address_PostalCode = c.String(maxLength: 15),
                        SoldTo_Address_Country = c.String(maxLength: 50),
                        SoldTo_AdditionalInfo = c.String(maxLength: 100),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Contacts", t => new { t.CustomerId, t.ContactId })
                .ForeignKey("dbo.Brokers", t => t.BrokerId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .ForeignKey("dbo.ShipmentInformation", t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .ForeignKey("dbo.PickedInventory", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.InventoryPickOrders", t => new { t.DateCreated, t.Sequence })
                .Index(t => t.CustomerId)
                .Index(t => new { t.CustomerId, t.ContactId })
                .Index(t => t.BrokerId)
                .Index(t => t.WarehouseId)
                .Index(t => new { t.ShipmentInfoDateCreated, t.ShipmentInfoSequence })
                .Index(t => new { t.DateCreated, t.Sequence });
            
            CreateTable(
                "dbo.CustomerOrderItems",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        ContractYear = c.Int(),
                        ContractSequence = c.Int(),
                        ContractItemSequence = c.Int(),
                        CustomerLotCode = c.String(maxLength: 15),
                        PriceBase = c.Double(nullable: false),
                        PriceFreight = c.Double(nullable: false),
                        PriceTreatment = c.Double(nullable: false),
                        PriceWarehouse = c.Double(nullable: false),
                        PriceRebate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .ForeignKey("dbo.CustomerOrders", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.InventoryPickOrderItems", t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.DateCreated, t.Sequence, t.ItemSequence });
            
            AddColumn("dbo.ShipmentInformation", "FreightBillType", c => c.String(maxLength: 25));
            AddColumn("dbo.ShipmentInformation", "ShipmentMethod", c => c.String(maxLength: 25));
            AddColumn("dbo.ShipmentInformation", "ScheduledShipDate", c => c.DateTime());
            AlterColumn("dbo.ShipmentInformation", "RequiredDeliveryDate", c => c.DateTime());
            AlterColumn("dbo.Contracts", "PaymentTerms", c => c.String(maxLength: 25));
            DropColumn("dbo.ShipmentInformation", "FreightType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentInformation", "FreightType", c => c.String(maxLength: 25));
            DropIndex("dbo.CustomerOrderItems", new[] { "DateCreated", "Sequence", "ItemSequence" });
            DropIndex("dbo.CustomerOrderItems", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" });
            DropIndex("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" });
            DropIndex("dbo.CustomerOrders", new[] { "WarehouseId" });
            DropIndex("dbo.CustomerOrders", new[] { "BrokerId" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerId" });
            DropForeignKey("dbo.CustomerOrderItems", new[] { "DateCreated", "Sequence", "ItemSequence" }, "dbo.InventoryPickOrderItems");
            DropForeignKey("dbo.CustomerOrderItems", new[] { "DateCreated", "Sequence" }, "dbo.CustomerOrders");
            DropForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.InventoryPickOrders");
            DropForeignKey("dbo.CustomerOrders", new[] { "DateCreated", "Sequence" }, "dbo.PickedInventory");
            DropForeignKey("dbo.CustomerOrders", new[] { "ShipmentInfoDateCreated", "ShipmentInfoSequence" }, "dbo.ShipmentInformation");
            DropForeignKey("dbo.CustomerOrders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.CustomerOrders", "BrokerId", "dbo.Brokers");
            DropForeignKey("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" }, "dbo.Contacts");
            DropForeignKey("dbo.CustomerOrders", "CustomerId", "dbo.Customers");
            AlterColumn("dbo.Contracts", "PaymentTerms", c => c.String(maxLength: 20));
            AlterColumn("dbo.ShipmentInformation", "RequiredDeliveryDate", c => c.DateTime(storeType: "date"));
            DropColumn("dbo.ShipmentInformation", "ScheduledShipDate");
            DropColumn("dbo.ShipmentInformation", "ShipmentMethod");
            DropColumn("dbo.ShipmentInformation", "FreightBillType");
            DropTable("dbo.CustomerOrderItems");
            DropTable("dbo.CustomerOrders");
        }
    }
}
