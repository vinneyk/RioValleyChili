namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotDefectResolutionMaxDescriptionLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LotDefectResolutions", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LotDefectResolutions", "Description", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
