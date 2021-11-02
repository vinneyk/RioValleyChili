namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PackSchedule_PSNum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PackSchedules", "PSNum", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PackSchedules", "PSNum");
        }
    }
}
