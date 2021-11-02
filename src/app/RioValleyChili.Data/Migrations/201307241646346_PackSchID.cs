namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PackSchID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PackSchedules", "PackSchID", c => c.DateTime(nullable: false));
            AddColumn("dbo.InventoryPickOrderItems", "TreatmentId", c => c.Int(nullable: false));
            AddForeignKey("dbo.InventoryPickOrderItems", "TreatmentId", "dbo.InventoryTreatments", "Id");
            CreateIndex("dbo.InventoryPickOrderItems", "TreatmentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InventoryPickOrderItems", new[] { "TreatmentId" });
            DropForeignKey("dbo.InventoryPickOrderItems", "TreatmentId", "dbo.InventoryTreatments");
            DropColumn("dbo.InventoryPickOrderItems", "TreatmentId");
            DropColumn("dbo.PackSchedules", "PackSchID");
        }
    }
}
