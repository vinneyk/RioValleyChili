namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Contracts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId" }, "dbo.Contacts");
            DropIndex("dbo.Contracts", new[] { "CustomerId", "ContactId" });
            AddColumn("dbo.Contracts", "CustomerPurchaseOrder", c => c.String(maxLength: 50));
            AddColumn("dbo.Contracts", "ContractId", c => c.Int());
            AddColumn("dbo.Contracts", "CommentsDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Contracts", "CommentsSequence", c => c.Int(nullable: false));
            AddColumn("dbo.Contracts", "ContactName", c => c.String());
            AddColumn("dbo.Contracts", "FOB", c => c.String(maxLength: 30));
            AddColumn("dbo.Contracts", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Contracts", "TermBegin", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.Contracts", "TermEnd", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.CustomerOrders", "CustomerPurchaseOrder", c => c.String(maxLength: 50));
            //CreateIndex("dbo.Contracts", "CustomerId");
            CreateIndex("dbo.Contracts", new[] { "CommentsDate", "CommentsSequence" });
            CreateIndex("dbo.Contracts", "EmployeeId");
            AddForeignKey("dbo.Contracts", new[] { "CommentsDate", "CommentsSequence" }, "dbo.Notebooks", new[] { "Date", "Sequence" });
            AddForeignKey("dbo.Contracts", "EmployeeId", "dbo.Employees", "EmployeeId");
            DropColumn("dbo.Contracts", "Comments");
            DropColumn("dbo.Contracts", "ContactId");
            DropColumn("dbo.Contracts", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contracts", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.Contracts", "ContactId", c => c.Int(nullable: false));
            AddColumn("dbo.Contracts", "Comments", c => c.String());
            DropForeignKey("dbo.Contracts", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Contracts", new[] { "CommentsDate", "CommentsSequence" }, "dbo.Notebooks");
            DropIndex("dbo.Contracts", new[] { "EmployeeId" });
            DropIndex("dbo.Contracts", new[] { "CommentsDate", "CommentsSequence" });
            //DropIndex("dbo.Contracts", new[] { "CustomerId" });
            AlterColumn("dbo.CustomerOrders", "CustomerPurchaseOrder", c => c.String(maxLength: 10));
            AlterColumn("dbo.Contracts", "TermEnd", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Contracts", "TermBegin", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.Contracts", "EmployeeId");
            DropColumn("dbo.Contracts", "FOB");
            DropColumn("dbo.Contracts", "ContactName");
            DropColumn("dbo.Contracts", "CommentsSequence");
            DropColumn("dbo.Contracts", "CommentsDate");
            DropColumn("dbo.Contracts", "ContractId");
            DropColumn("dbo.Contracts", "CustomerPurchaseOrder");
            CreateIndex("dbo.Contracts", new[] { "CustomerId", "ContactId" });
            AddForeignKey("dbo.Contracts", new[] { "CustomerId", "ContactId" }, "dbo.Contacts", new[] { "CompanyId", "ContactId" });
        }
    }
}
