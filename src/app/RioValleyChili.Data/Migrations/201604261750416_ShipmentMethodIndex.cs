namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentMethodIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ShipmentInformation", "ShipmentMethod");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ShipmentInformation", new[] { "ShipmentMethod" });
        }
    }
}
