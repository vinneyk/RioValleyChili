namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Facility_WHID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facilities", "WHID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facilities", "WHID");
        }
    }
}
