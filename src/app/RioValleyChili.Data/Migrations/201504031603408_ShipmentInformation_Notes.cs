namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentInformation_Notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentInformation", "InternalNotes", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentInformation", "ExternalNotes", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentInformation", "SpecialInstructions", c => c.String(maxLength: 100));
            DropColumn("dbo.ShipmentInformation", "Comments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentInformation", "Comments", c => c.String(maxLength: 100));
            DropColumn("dbo.ShipmentInformation", "SpecialInstructions");
            DropColumn("dbo.ShipmentInformation", "ExternalNotes");
            DropColumn("dbo.ShipmentInformation", "InternalNotes");
        }
    }
}
