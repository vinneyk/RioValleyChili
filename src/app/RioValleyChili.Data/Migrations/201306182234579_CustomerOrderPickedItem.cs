namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrderPickedItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrderPickedItems",
                c => new
                    {
                        DateCreated = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                        ItemSequence = c.Int(nullable: false),
                        OrderItemSequence = c.Int(),
                    })
                .PrimaryKey(t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .ForeignKey("dbo.CustomerOrders", t => new { t.DateCreated, t.Sequence })
                .ForeignKey("dbo.PickedInventoryItems", t => new { t.DateCreated, t.Sequence, t.ItemSequence })
                .Index(t => new { t.DateCreated, t.Sequence })
                .Index(t => new { t.DateCreated, t.Sequence, t.ItemSequence });
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence", "ItemSequence" });
            DropIndex("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence" });
            DropForeignKey("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence", "ItemSequence" }, "dbo.PickedInventoryItems");
            DropForeignKey("dbo.CustomerOrderPickedItems", new[] { "DateCreated", "Sequence" }, "dbo.CustomerOrders");
            DropTable("dbo.CustomerOrderPickedItems");
        }
    }
}
