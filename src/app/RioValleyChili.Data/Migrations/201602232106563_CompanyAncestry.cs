namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    /// <summary>
    /// Based on https://code.msdn.microsoft.com/windowsdesktop/Recursive-or-hierarchical-bf43a96e
    /// </summary>
    public partial class CompanyAncestry : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.CompanyAncestries",
            //    c => new
            //        {
            //            DescendantId = c.Int(nullable: false),
            //            AncestorId = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.DescendantId)
            //    .ForeignKey("dbo.Companies", t => t.AncestorId)
            //    .ForeignKey("dbo.Companies", t => t.DescendantId)
            //    .Index(t => t.DescendantId)
            //    .Index(t => t.AncestorId);
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
        
        public override void Down()
        {
            //DropForeignKey("dbo.CompanyAncestries", "DescendantId", "dbo.Companies");
            //DropForeignKey("dbo.CompanyAncestries", "AncestorId", "dbo.Companies");
            //DropIndex("dbo.CompanyAncestries", new[] { "AncestorId" });
            //DropIndex("dbo.CompanyAncestries", new[] { "DescendantId" });
            //DropTable("dbo.CompanyAncestries");
            Sql("DROP VIEW [dbo].[CompanyAncestries]");
        }
    }
}
