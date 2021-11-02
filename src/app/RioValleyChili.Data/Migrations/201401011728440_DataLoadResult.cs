namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DataLoadResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataLoadResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Success = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataLoadResults");
        }
    }
}
