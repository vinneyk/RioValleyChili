namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCompanyDepths : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP VIEW [dbo].[CompanyDepths]");
        }
        
        public override void Down()
        {
            Sql(@"
                CREATE VIEW [dbo].[CompanyDepths] AS
                WITH CompanyList AS
                (
                    SELECT parent.Id, 0 AS Depth
                    FROM dbo.Companies AS parent
                    WHERE parent.ParentId IS NULL
    
                    UNION ALL
    
                    SELECT child.Id, CompanyList.Depth + 1
                    FROM dbo.Companies AS child
                    INNER JOIN CompanyList ON child.ParentId = CompanyList.Id
                    WHERE child.ParentId IS NOT NULL
                )	
                SELECT * FROM CompanyList
            ");
        }
    }
}
