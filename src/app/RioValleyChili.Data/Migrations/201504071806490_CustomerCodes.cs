namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerCodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PickedInventoryItems", "CustomerProductCode", c => c.String(maxLength: 25));
            AddColumn("dbo.PickedInventoryItems", "CustomerLotCode", c => c.String(maxLength: 25));
            AddColumn("dbo.InventoryPickOrderItems", "CustomerProductCode", c => c.String(maxLength: 25));
            AddColumn("dbo.InventoryPickOrderItems", "CustomerLotCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.ContractItems", "CustomerProductCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.CustomerOrderItems", "CustomerLotCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.CustomerProductCodes", "Code", c => c.String(maxLength: 25));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerProductCodes", "Code", c => c.String(maxLength: 15));
            AlterColumn("dbo.CustomerOrderItems", "CustomerLotCode", c => c.String(maxLength: 15));
            AlterColumn("dbo.ContractItems", "CustomerProductCode", c => c.String(maxLength: 15));
            DropColumn("dbo.InventoryPickOrderItems", "CustomerLotCode");
            DropColumn("dbo.InventoryPickOrderItems", "CustomerProductCode");
            DropColumn("dbo.PickedInventoryItems", "CustomerLotCode");
            DropColumn("dbo.PickedInventoryItems", "CustomerProductCode");
        }
    }
}
