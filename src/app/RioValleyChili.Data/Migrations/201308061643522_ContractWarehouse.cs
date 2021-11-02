namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ContractWarehouse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" }, "dbo.ContactAddresses");
            DropIndex("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" });
            AddColumn("dbo.Contracts", "DefaultPickFromWarehouseId", c => c.Int(nullable: false));
            AddColumn("dbo.Contracts", "ContactAddress_AddressLine1", c => c.String(maxLength: 50));
            AddColumn("dbo.Contracts", "ContactAddress_AddressLine2", c => c.String(maxLength: 50));
            AddColumn("dbo.Contracts", "ContactAddress_AddressLine3", c => c.String(maxLength: 50));
            AddColumn("dbo.Contracts", "ContactAddress_City", c => c.String(maxLength: 75));
            AddColumn("dbo.Contracts", "ContactAddress_State", c => c.String(maxLength: 50));
            AddColumn("dbo.Contracts", "ContactAddress_PostalCode", c => c.String(maxLength: 15));
            AddColumn("dbo.Contracts", "ContactAddress_Country", c => c.String(maxLength: 50));
            AddForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Warehouses", "Id");
            CreateIndex("dbo.Contracts", "DefaultPickFromWarehouseId");
            DropColumn("dbo.Contracts", "ContactAddressId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contracts", "ContactAddressId", c => c.Int(nullable: false));
            DropIndex("dbo.Contracts", new[] { "DefaultPickFromWarehouseId" });
            DropForeignKey("dbo.Contracts", "DefaultPickFromWarehouseId", "dbo.Warehouses");
            DropColumn("dbo.Contracts", "ContactAddress_Country");
            DropColumn("dbo.Contracts", "ContactAddress_PostalCode");
            DropColumn("dbo.Contracts", "ContactAddress_State");
            DropColumn("dbo.Contracts", "ContactAddress_City");
            DropColumn("dbo.Contracts", "ContactAddress_AddressLine3");
            DropColumn("dbo.Contracts", "ContactAddress_AddressLine2");
            DropColumn("dbo.Contracts", "ContactAddress_AddressLine1");
            DropColumn("dbo.Contracts", "DefaultPickFromWarehouseId");
            CreateIndex("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" });
            AddForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId", "ContactAddressId" }, "dbo.ContactAddresses", new[] { "CompanyId", "ContactId", "AddressId" });
        }
    }
}
