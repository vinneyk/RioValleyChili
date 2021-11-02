namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ContactSync : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "OldContextID", c => c.Int());
            AddColumn("dbo.ContactAddresses", "OldContextID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContactAddresses", "OldContextID");
            DropColumn("dbo.Contacts", "OldContextID");
        }
    }
}
