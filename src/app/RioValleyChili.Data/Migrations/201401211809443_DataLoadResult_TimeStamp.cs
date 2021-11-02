namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DataLoadResult_TimeStamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataLoadResults", "TimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataLoadResults", "TimeStamp");
        }
    }
}
