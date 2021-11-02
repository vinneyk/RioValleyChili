namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrder_Invoicing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "InvoiceDate", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.CustomerOrders", "InvoiceNotes", c => c.String(maxLength: 300));
            AddColumn("dbo.CustomerOrders", "CreditMemo", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "FreightCharge", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "FreightCharge");
            DropColumn("dbo.CustomerOrders", "CreditMemo");
            DropColumn("dbo.CustomerOrders", "InvoiceNotes");
            DropColumn("dbo.CustomerOrders", "InvoiceDate");
        }
    }
}
