namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryPickOrderItem_DropTreatment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryPickOrderItems", "TreatmentId", "dbo.InventoryTreatments");
            DropIndex("dbo.InventoryPickOrderItems", new[] { "TreatmentId" });
            DropColumn("dbo.InventoryPickOrderItems", "TreatmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InventoryPickOrderItems", "TreatmentId", c => c.Int());
            CreateIndex("dbo.InventoryPickOrderItems", "TreatmentId");
            AddForeignKey("dbo.InventoryPickOrderItems", "TreatmentId", "dbo.InventoryTreatments", "Id");
        }
    }
}
