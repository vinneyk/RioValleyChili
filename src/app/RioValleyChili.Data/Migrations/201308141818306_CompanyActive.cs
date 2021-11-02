namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CompanyActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Active");
        }
    }
}
