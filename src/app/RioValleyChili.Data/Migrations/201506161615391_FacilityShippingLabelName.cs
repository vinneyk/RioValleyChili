namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FacilityShippingLabelName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facilities", "ShippingLabelName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facilities", "ShippingLabelName");
        }
    }
}
