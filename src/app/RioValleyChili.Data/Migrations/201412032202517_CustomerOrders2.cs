namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrders2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" }, "dbo.Contacts");
            DropIndex("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" });
            AddColumn("dbo.CustomerOrders", "OrderNum", c => c.Int());
            AddColumn("dbo.CustomerOrders", "SoldTo_Phone", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerOrders", "SoldTo_EMail", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerOrders", "SoldTo_Fax", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerOrders", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrderItems", "ODetail", c => c.DateTime());
            AddColumn("dbo.CustomerOrderPickedItems", "EntryDate", c => c.DateTime());
            AddColumn("dbo.ShipmentInformation", "ShipFrom_Phone", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipFrom_EMail", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipFrom_Fax", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipTo_Phone", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipTo_EMail", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "ShipTo_Fax", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "FreightBill_Phone", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "FreightBill_EMail", c => c.String(maxLength: 50));
            AddColumn("dbo.ShipmentInformation", "FreightBill_Fax", c => c.String(maxLength: 50));
            CreateIndex("dbo.CustomerOrders", "EmployeeId");
            CreateIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            CreateIndex("dbo.CustomerOrderPickedItems", "OrderItemSequence", name: "IX_DateCreated_Sequence_OrderItemSequence");
            AddForeignKey("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" }, "dbo.ContractItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            AddForeignKey("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence", "OrderItemSequence" }, "dbo.CustomerOrderItems", new[] { "DateCreated", "Sequence", "ItemSequence" });
            AddForeignKey("dbo.CustomerOrders", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.CustomerOrders", "ContactId");
            DropColumn("dbo.CustomerOrders", "SoldTo_AdditionalInfo");
            DropColumn("dbo.CustomerOrders", "User");
            DropColumn("dbo.ShipmentInformation", "ShipFrom_AdditionalInfo");
            DropColumn("dbo.ShipmentInformation", "ShipTo_AdditionalInfo");
            DropColumn("dbo.ShipmentInformation", "FreightBill_AdditionalInfo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentInformation", "FreightBill_AdditionalInfo", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentInformation", "ShipTo_AdditionalInfo", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentInformation", "ShipFrom_AdditionalInfo", c => c.String(maxLength: 100));
            AddColumn("dbo.CustomerOrders", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.CustomerOrders", "SoldTo_AdditionalInfo", c => c.String(maxLength: 100));
            AddColumn("dbo.CustomerOrders", "ContactId", c => c.Int(nullable: false));
            DropForeignKey("dbo.CustomerOrders", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence", "OrderItemSequence" }, "dbo.CustomerOrderItems");
            DropForeignKey("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" }, "dbo.ContractItems");
            DropIndex("dbo.CustomerOrderPickedItems", "IX_DateCreated_Sequence_OrderItemSequence");
            DropIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            DropIndex("dbo.CustomerOrders", new[] { "EmployeeId" });
            DropColumn("dbo.ShipmentInformation", "FreightBill_Fax");
            DropColumn("dbo.ShipmentInformation", "FreightBill_EMail");
            DropColumn("dbo.ShipmentInformation", "FreightBill_Phone");
            DropColumn("dbo.ShipmentInformation", "ShipTo_Fax");
            DropColumn("dbo.ShipmentInformation", "ShipTo_EMail");
            DropColumn("dbo.ShipmentInformation", "ShipTo_Phone");
            DropColumn("dbo.ShipmentInformation", "ShipFrom_Fax");
            DropColumn("dbo.ShipmentInformation", "ShipFrom_EMail");
            DropColumn("dbo.ShipmentInformation", "ShipFrom_Phone");
            DropColumn("dbo.CustomerOrderPickedItems", "EntryDate");
            DropColumn("dbo.CustomerOrderItems", "ODetail");
            DropColumn("dbo.CustomerOrders", "EmployeeId");
            DropColumn("dbo.CustomerOrders", "SoldTo_Fax");
            DropColumn("dbo.CustomerOrders", "SoldTo_EMail");
            DropColumn("dbo.CustomerOrders", "SoldTo_Phone");
            DropColumn("dbo.CustomerOrders", "OrderNum");
            CreateIndex("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" });
            AddForeignKey("dbo.CustomerOrders", new[] { "CustomerId", "ContactId" }, "dbo.Contacts", new[] { "CompanyId", "ContactId" });
        }
    }
}
