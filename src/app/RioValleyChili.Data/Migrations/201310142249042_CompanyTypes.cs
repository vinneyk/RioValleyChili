namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CompanyTypes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DehydratedMaterialsReceivedItems", "GrowerId", "dbo.Companies");
            DropIndex("dbo.DehydratedMaterialsReceivedItems", new[] { "GrowerId" });
            CreateTable(
                "dbo.CompanyTypeRecords",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        CompanyType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.CompanyType })
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "GrowerCode", c => c.String(maxLength: 20));
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "Name", c => c.String(maxLength: 60));
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.Contacts", "EMailAddress", c => c.String(maxLength: 50));
            DropColumn("dbo.Companies", "CompanyType");
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "GrowerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DehydratedMaterialsReceivedItems", "GrowerId", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "CompanyType", c => c.Short(nullable: false));
            DropIndex("dbo.CompanyTypeRecords", new[] { "CompanyId" });
            DropForeignKey("dbo.CompanyTypeRecords", "CompanyId", "dbo.Companies");
            AlterColumn("dbo.Contacts", "EMailAddress", c => c.String(maxLength: 40));
            AlterColumn("dbo.Contacts", "PhoneNumber", c => c.String(maxLength: 20));
            AlterColumn("dbo.Contacts", "Name", c => c.String(maxLength: 25));
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 50));
            DropColumn("dbo.DehydratedMaterialsReceivedItems", "GrowerCode");
            DropTable("dbo.CompanyTypeRecords");
            CreateIndex("dbo.DehydratedMaterialsReceivedItems", "GrowerId");
            AddForeignKey("dbo.DehydratedMaterialsReceivedItems", "GrowerId", "dbo.Companies", "Id");
        }
    }
}
