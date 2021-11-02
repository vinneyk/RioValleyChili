namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MiscOrders : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CustomerOrderItems", newName: "SalesOrderItems");
            RenameTable(name: "dbo.CustomerOrders", newName: "SalesOrders");
            RenameTable(name: "dbo.CustomerOrderPickedItems", newName: "SalesOrderPickedItems");
            RenameTable(name: "dbo.LotCustomerOrderAllowances", newName: "LotSalesOrderAllowances");
            DropIndex("dbo.SalesOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            DropIndex("dbo.SalesOrders", new[] { "CustomerId" });
            DropIndex("dbo.SalesOrderPickedItems", new[] { "DateCreated", "Sequence", "ItemSequence" });
            DropIndex("dbo.SalesOrderPickedItems", "IX_DateCreated_Sequence_OrderItemSequence");
            RenameColumn(table: "dbo.LotSalesOrderAllowances", name: "CustomerOrderDateCreated", newName: "SalesOrderDateCreated");
            RenameColumn(table: "dbo.LotSalesOrderAllowances", name: "CustomerOrderSequence", newName: "SalesOrderSequence");
            RenameIndex(table: "dbo.LotSalesOrderAllowances", name: "IX_CustomerOrderDateCreated_CustomerOrderSequence", newName: "IX_SalesOrderDateCreated_SalesOrderSequence");
            AlterColumn("dbo.SalesOrderItems", "ContractYear", c => c.Int());
            AlterColumn("dbo.SalesOrderItems", "ContractSequence", c => c.Int());
            AlterColumn("dbo.SalesOrderItems", "ContractItemSequence", c => c.Int());
            AlterColumn("dbo.SalesOrders", "CustomerId", c => c.Int());
            CreateIndex("dbo.SalesOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            CreateIndex("dbo.SalesOrders", "CustomerId");
            CreateIndex("dbo.SalesOrderPickedItems", new[] { "DateCreated", "Sequence", "OrderItemSequence" });
            CreateIndex("dbo.SalesOrderPickedItems", "ItemSequence", name: "IX_DateCreated_Sequence_ItemSequence");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SalesOrderPickedItems", "IX_DateCreated_Sequence_ItemSequence");
            DropIndex("dbo.SalesOrderPickedItems", new[] { "DateCreated", "Sequence", "OrderItemSequence" });
            DropIndex("dbo.SalesOrders", new[] { "CustomerId" });
            DropIndex("dbo.SalesOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            AlterColumn("dbo.SalesOrders", "CustomerId", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesOrderItems", "ContractItemSequence", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesOrderItems", "ContractSequence", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesOrderItems", "ContractYear", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.LotSalesOrderAllowances", name: "IX_SalesOrderDateCreated_SalesOrderSequence", newName: "IX_CustomerOrderDateCreated_CustomerOrderSequence");
            RenameColumn(table: "dbo.LotSalesOrderAllowances", name: "SalesOrderSequence", newName: "CustomerOrderSequence");
            RenameColumn(table: "dbo.LotSalesOrderAllowances", name: "SalesOrderDateCreated", newName: "CustomerOrderDateCreated");
            CreateIndex("dbo.SalesOrderPickedItems", "OrderItemSequence", name: "IX_DateCreated_Sequence_OrderItemSequence");
            CreateIndex("dbo.SalesOrderPickedItems", new[] { "DateCreated", "Sequence", "ItemSequence" });
            CreateIndex("dbo.SalesOrders", "CustomerId");
            CreateIndex("dbo.SalesOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            RenameTable(name: "dbo.LotSalesOrderAllowances", newName: "LotCustomerOrderAllowances");
            RenameTable(name: "dbo.SalesOrderPickedItems", newName: "CustomerOrderPickedItems");
            RenameTable(name: "dbo.SalesOrders", newName: "CustomerOrders");
            RenameTable(name: "dbo.SalesOrderItems", newName: "CustomerOrderItems");
        }
    }
}
