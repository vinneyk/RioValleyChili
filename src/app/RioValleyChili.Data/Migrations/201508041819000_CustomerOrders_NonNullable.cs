namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrders_NonNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            DropIndex("dbo.CustomerOrderPickedItems", "IX_DateCreated_Sequence_OrderItemSequence");
            AlterColumn("dbo.CustomerOrderItems", "ContractYear", c => c.Int(nullable: false));
            AlterColumn("dbo.CustomerOrderItems", "ContractSequence", c => c.Int(nullable: false));
            AlterColumn("dbo.CustomerOrderItems", "ContractItemSequence", c => c.Int(nullable: false));
            AlterColumn("dbo.CustomerOrderPickedItems", "OrderItemSequence", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            CreateIndex("dbo.CustomerOrderPickedItems", "OrderItemSequence", name: "IX_DateCreated_Sequence_OrderItemSequence");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerOrderPickedItems", "IX_DateCreated_Sequence_OrderItemSequence");
            DropIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
            AlterColumn("dbo.CustomerOrderPickedItems", "OrderItemSequence", c => c.Int());
            AlterColumn("dbo.CustomerOrderItems", "ContractItemSequence", c => c.Int());
            AlterColumn("dbo.CustomerOrderItems", "ContractSequence", c => c.Int());
            AlterColumn("dbo.CustomerOrderItems", "ContractYear", c => c.Int());
            CreateIndex("dbo.CustomerOrderPickedItems", "OrderItemSequence", name: "IX_DateCreated_Sequence_OrderItemSequence");
            CreateIndex("dbo.CustomerOrderItems", new[] { "ContractYear", "ContractSequence", "ContractItemSequence" });
        }
    }
}
