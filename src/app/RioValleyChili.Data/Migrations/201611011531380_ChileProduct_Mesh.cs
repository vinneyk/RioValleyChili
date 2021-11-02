namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChileProduct_Mesh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChileProducts", "Mesh", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChileProducts", "Mesh");
        }
    }
}
