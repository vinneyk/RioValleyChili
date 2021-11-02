namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PickOrderItem_Customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPickOrderItems", "CustomerId", c => c.Int());
            CreateIndex("dbo.InventoryPickOrderItems", "CustomerId");
            AddForeignKey("dbo.InventoryPickOrderItems", "CustomerId", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryPickOrderItems", "CustomerId", "dbo.Customers");
            DropIndex("dbo.InventoryPickOrderItems", new[] { "CustomerId" });
            DropColumn("dbo.InventoryPickOrderItems", "CustomerId");
        }
    }
}
