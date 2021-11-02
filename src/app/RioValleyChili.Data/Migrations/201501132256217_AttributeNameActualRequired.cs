namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AttributeNameActualRequired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttributeNames", "ActualValueRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AttributeNames", "ActualValueRequired");
        }
    }
}
