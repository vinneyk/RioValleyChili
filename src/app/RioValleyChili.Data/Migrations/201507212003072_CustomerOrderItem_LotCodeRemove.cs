namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrderItem_LotCodeRemove : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CustomerOrderItems", "CustomerLotCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrderItems", "CustomerLotCode", c => c.String(maxLength: 25));
        }
    }
}
