namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ContractItemKDetailID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractItems", "KDetailID", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContractItems", "KDetailID");
        }
    }
}
