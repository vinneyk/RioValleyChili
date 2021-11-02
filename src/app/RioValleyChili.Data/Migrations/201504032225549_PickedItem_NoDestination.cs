namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PickedItem_NoDestination : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PickedInventoryItems", "DestinationLocationId", "dbo.Locations");
            DropIndex("dbo.PickedInventoryItems", new[] { "DestinationLocationId" });
            DropColumn("dbo.PickedInventoryItems", "DestinationLocationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PickedInventoryItems", "DestinationLocationId", c => c.Int());
            CreateIndex("dbo.PickedInventoryItems", "DestinationLocationId");
            AddForeignKey("dbo.PickedInventoryItems", "DestinationLocationId", "dbo.Locations", "Id");
        }
    }
}
