namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ShipmentInformation_NoteLengths : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShipmentInformation", "InternalNotes", c => c.String(maxLength: 600));
            AlterColumn("dbo.ShipmentInformation", "ExternalNotes", c => c.String(maxLength: 600));
            AlterColumn("dbo.ShipmentInformation", "SpecialInstructions", c => c.String(maxLength: 600));
        }
        
        public override void Down()
        {
            Sql(@"UPDATE dbo.ShipmentInformation SET SpecialInstructions = LEFT(SpecialInstructions, 100), ExternalNotes = LEFT(ExternalNotes, 100), InternalNotes = LEFT(InternalNotes, 100)");
            AlterColumn("dbo.ShipmentInformation", "SpecialInstructions", c => c.String(maxLength: 100));
            AlterColumn("dbo.ShipmentInformation", "ExternalNotes", c => c.String(maxLength: 100));
            AlterColumn("dbo.ShipmentInformation", "InternalNotes", c => c.String(maxLength: 100));
        }
    }
}
