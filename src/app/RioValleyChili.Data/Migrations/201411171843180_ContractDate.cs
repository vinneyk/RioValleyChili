namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ContractDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "TimeCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Contracts", "ContractDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.Contracts", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contracts", "DateCreated", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.Contracts", "ContractDate");
            DropColumn("dbo.Contracts", "TimeCreated");
        }
    }
}
