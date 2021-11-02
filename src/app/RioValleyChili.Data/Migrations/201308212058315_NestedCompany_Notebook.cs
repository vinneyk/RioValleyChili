namespace RioValleyChili.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NestedCompany_Notebook : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notebooks",
                c => new
                    {
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Date, t.Sequence });
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        NotebookDate = c.DateTime(nullable: false, storeType: "date"),
                        NotebookSequence = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Text = c.String(),
                        User = c.String(nullable: false, maxLength: 25),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.NotebookDate, t.NotebookSequence, t.Sequence })
                .ForeignKey("dbo.Notebooks", t => new { t.NotebookDate, t.NotebookSequence })
                .Index(t => new { t.NotebookDate, t.NotebookSequence });
            
            AddColumn("dbo.Companies", "NotebookDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Companies", "NotebookSequence", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "ParentId", c => c.Int());
            AddForeignKey("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            AddForeignKey("dbo.Companies", "ParentId", "dbo.Companies", "Id");
            CreateIndex("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" });
            CreateIndex("dbo.Companies", "ParentId");

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

        public override void Down()
        {
            Sql(@"DROP VIEW [dbo].[CompanyDepths]");

            DropIndex("dbo.Notes", new[] { "NotebookDate", "NotebookSequence" });
            DropIndex("dbo.Companies", new[] { "ParentId" });
            DropIndex("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" });
            DropForeignKey("dbo.Notes", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks");
            DropForeignKey("dbo.Companies", "ParentId", "dbo.Companies");
            DropForeignKey("dbo.Companies", new[] { "NotebookDate", "NotebookSequence" }, "dbo.Notebooks");
            DropColumn("dbo.Companies", "ParentId");
            DropColumn("dbo.Companies", "NotebookSequence");
            DropColumn("dbo.Companies", "NotebookDate");
            DropTable("dbo.Notes");
            DropTable("dbo.Notebooks");
        }
    }
}
