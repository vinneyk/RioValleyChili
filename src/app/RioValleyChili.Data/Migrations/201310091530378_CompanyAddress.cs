namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyAddresses",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        AddressId = c.Int(nullable: false),
                        AddressDescription = c.String(maxLength: 25),
                        Address_AddressLine1 = c.String(maxLength: 50),
                        Address_AddressLine2 = c.String(maxLength: 50),
                        Address_AddressLine3 = c.String(maxLength: 50),
                        Address_City = c.String(maxLength: 75),
                        Address_State = c.String(maxLength: 50),
                        Address_PostalCode = c.String(maxLength: 15),
                        Address_Country = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.AddressId })
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            AddColumn("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", c => c.Int(nullable: false));
            AddColumn("dbo.ContactAddresses", "AddressDescription", c => c.String(maxLength: 25));
            AlterColumn("dbo.Instructions", "InstructionText", c => c.String(maxLength: 250));
            AddForeignKey("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", "dbo.Companies", "Id");
            CreateIndex("dbo.TreatmentOrders", "TreatmentFacilityCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CompanyAddresses", new[] { "CompanyId" });
            DropIndex("dbo.TreatmentOrders", new[] { "TreatmentFacilityCompanyId" });
            DropForeignKey("dbo.CompanyAddresses", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.TreatmentOrders", "TreatmentFacilityCompanyId", "dbo.Companies");
            AlterColumn("dbo.Instructions", "InstructionText", c => c.String(maxLength: 100));
            DropColumn("dbo.ContactAddresses", "AddressDescription");
            DropColumn("dbo.TreatmentOrders", "TreatmentFacilityCompanyId");
            DropTable("dbo.CompanyAddresses");
        }
    }
}
