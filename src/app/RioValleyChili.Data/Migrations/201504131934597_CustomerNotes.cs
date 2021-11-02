namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CustomerNotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerNotes",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        NoteId = c.Int(nullable: false),
                        Type = c.String(maxLength: 25),
                        Text = c.String(),
                        Bold = c.Boolean(nullable: false),
                        EntryDate = c.DateTime(),
                        EmployeeId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.NoteId })
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.CustomerId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerNotes", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.CustomerNotes", "CustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerNotes", new[] { "EmployeeId" });
            DropIndex("dbo.CustomerNotes", new[] { "CustomerId" });
            DropTable("dbo.CustomerNotes");
        }
    }
}
