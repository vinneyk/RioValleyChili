namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LocationUniqueFacilityDescription : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Locations", new[] { "Description" });
            DropIndex("dbo.Locations", new[] { "FacilityId" });
            CreateIndex("dbo.Locations", new[] { "FacilityId", "Description" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Locations", new[] { "FacilityId", "Description" });
            CreateIndex("dbo.Locations", "FacilityId");
            CreateIndex("dbo.Locations", "Description", unique: true);
        }
    }
}
