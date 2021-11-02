namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerCodeLengths : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContractItems", "CustomerProductCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.InventoryPickOrderItems", "CustomerProductCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.InventoryPickOrderItems", "CustomerLotCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.PickedInventoryItems", "CustomerProductCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.PickedInventoryItems", "CustomerLotCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.CustomerProductCodes", "Code", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerProductCodes", "Code", c => c.String(maxLength: 25));
            AlterColumn("dbo.PickedInventoryItems", "CustomerLotCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.PickedInventoryItems", "CustomerProductCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.InventoryPickOrderItems", "CustomerLotCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.InventoryPickOrderItems", "CustomerProductCode", c => c.String(maxLength: 25));
            AlterColumn("dbo.ContractItems", "CustomerProductCode", c => c.String(maxLength: 25));
        }
    }
}
