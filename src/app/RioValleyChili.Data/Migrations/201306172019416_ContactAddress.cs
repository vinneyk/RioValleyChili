namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ContactAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactAddresses",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        AddressId = c.Int(nullable: false),
                        Address_AddressLine1 = c.String(maxLength: 50),
                        Address_AddressLine2 = c.String(maxLength: 50),
                        Address_AddressLine3 = c.String(maxLength: 50),
                        Address_City = c.String(maxLength: 75),
                        Address_State = c.String(maxLength: 50),
                        Address_PostalCode = c.String(maxLength: 15),
                        Address_Country = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.ContactId, t.AddressId })
                .ForeignKey("dbo.Contacts", t => new { t.CompanyId, t.ContactId })
                .Index(t => new { t.CompanyId, t.ContactId });
            
            AddColumn("dbo.Contacts", "EMailAddress", c => c.String(maxLength: 40));
            AddColumn("dbo.Contracts", "ContactAddressId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" }, "dbo.ContactAddresses", new[] { "CompanyId", "ContactId", "AddressId" });
            CreateIndex("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" });
            DropColumn("dbo.Contacts", "Address_AddressLine1");
            DropColumn("dbo.Contacts", "Address_AddressLine2");
            DropColumn("dbo.Contacts", "Address_AddressLine3");
            DropColumn("dbo.Contacts", "Address_City");
            DropColumn("dbo.Contacts", "Address_State");
            DropColumn("dbo.Contacts", "Address_PostalCode");
            DropColumn("dbo.Contacts", "Address_Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "Address_Country", c => c.String(maxLength: 50));
            AddColumn("dbo.Contacts", "Address_PostalCode", c => c.String(maxLength: 15));
            AddColumn("dbo.Contacts", "Address_State", c => c.String(maxLength: 50));
            AddColumn("dbo.Contacts", "Address_City", c => c.String(maxLength: 75));
            AddColumn("dbo.Contacts", "Address_AddressLine3", c => c.String(maxLength: 50));
            AddColumn("dbo.Contacts", "Address_AddressLine2", c => c.String(maxLength: 50));
            AddColumn("dbo.Contacts", "Address_AddressLine1", c => c.String(maxLength: 50));
            DropIndex("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" });
            DropIndex("dbo.ContactAddresses", new[] { "CompanyId", "ContactId" });
            DropForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" }, "dbo.ContactAddresses");
            DropForeignKey("dbo.ContactAddresses", new[] { "CompanyId", "ContactId" }, "dbo.Contacts");
            DropColumn("dbo.Contracts", "ContactAddressId");
            DropColumn("dbo.Contacts", "EMailAddress");
            DropTable("dbo.ContactAddresses");
        }
    }
}
