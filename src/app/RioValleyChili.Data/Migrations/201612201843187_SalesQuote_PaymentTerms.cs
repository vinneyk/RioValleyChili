namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SalesQuote_PaymentTerms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesQuotes", "PaymentTerms", c => c.String(maxLength: 25));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesQuotes", "PaymentTerms");
        }
    }
}
