namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LocationDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Description", c => c.String(maxLength: 20));
            CreateIndex("dbo.Locations", "Description", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Locations", new[] { "Description" });
            AlterColumn("dbo.Locations", "Description", c => c.String());
        }
    }
}
