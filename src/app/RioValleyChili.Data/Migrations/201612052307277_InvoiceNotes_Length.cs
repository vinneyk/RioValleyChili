namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceNotes_Length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesOrders", "InvoiceNotes", c => c.String(maxLength: 600));
        }
        
        public override void Down()
        {
            Sql(@"UPDATE dbo.SalesOrders SET InvoiceNotes = LEFT(InvoiceNotes, 300)");
            AlterColumn("dbo.SalesOrders", "InvoiceNotes", c => c.String(maxLength: 300));
        }
    }
}
