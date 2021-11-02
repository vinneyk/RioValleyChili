namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DataLoadCompletion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataLoadResults", "RanToCompletion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataLoadResults", "RanToCompletion");
        }
    }
}
