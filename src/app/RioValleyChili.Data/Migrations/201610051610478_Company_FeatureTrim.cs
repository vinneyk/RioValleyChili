namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Company_FeatureTrim : DbMigration
    {
        public override void Up()
        {
            Sql("DROP VIEW [dbo].[CompanyAncestries]");

            DropForeignKey("dbo.CompanyAddresses", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks");
            DropIndex("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" });
            DropIndex("dbo.CompanyAddresses", new[] { "CompanyId" });
            RenameColumn(table: "dbo.Companies", name: "ParentId", newName: "Company_Id");
            RenameIndex(table: "dbo.Companies", name: "IX_ParentId", newName: "IX_Company_Id");
            DropColumn("dbo.Companies", "NotebookDate");
            DropColumn("dbo.Companies", "NotebookSequence");
            DropTable("dbo.CompanyAddresses");
        }
        
        public override void Down()
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
                .PrimaryKey(t => new { t.CompanyId, t.AddressId });
            
            AddColumn("dbo.Companies", "NotebookSequence", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "NotebookDate", c => c.DateTime(nullable: false, storeType: "date"));
            RenameIndex(table: "dbo.Companies", name: "IX_Company_Id", newName: "IX_ParentId");
            RenameColumn(table: "dbo.Companies", name: "Company_Id", newName: "ParentId");
            CreateIndex("dbo.CompanyAddresses", "CompanyId");
            CreateIndex("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" });
            AddForeignKey("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            AddForeignKey("dbo.CompanyAddresses", "CompanyId", "dbo.Companies", "Id");

            Sql(@"CREATE VIEW [dbo].[CompanyAncestries] AS
                WITH cte (AncestorId, DescendantId)
                    AS
                    (
                        SELECT Id, Id
                        FROM dbo.Companies
                            UNION ALL
                            SELECT c.Id, cte.DescendantId
                            FROM cte
                                INNER JOIN dbo.Companies AS c ON c.ParentId = cte.AncestorId
                    )
                SELECT
                    ISNULL(DescendantId, 0) as AncestorId,
                    ISNULL(AncestorId, 0) as DescendantId
                FROM cte");
        }
    }
}
