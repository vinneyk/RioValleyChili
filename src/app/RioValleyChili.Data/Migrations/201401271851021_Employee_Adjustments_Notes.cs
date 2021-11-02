namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Employee_Adjustments_Notes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" }, "dbo.InventoryAdjustments");
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" });

            /// Had to replace commented line with the ones following it since the migration shouldn't just change
            /// the name of the column, but it's data type altogether. -RI 2014/1/27
            //RenameColumn(table: "dbo.InventoryAdjustmentItems", name: "TimeStamp", newName: "AdjustmentDate");
            DropPrimaryKey("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence", "ItemSequence" });
            AddColumn("dbo.InventoryAdjustmentItems", "AdjustmentDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddPrimaryKey("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence", "ItemSequence" });

            AddColumn("dbo.Notes", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustments", "AdjustmentDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.InventoryAdjustments", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryAdjustmentItems", "EmployeeId", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.InventoryAdjustments", new[] { "TimeStamp", "Sequence" });
            AddPrimaryKey("dbo.InventoryAdjustments", new[] { "AdjustmentDate", "Sequence" });
            AddForeignKey("dbo.Notes", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.InventoryAdjustments", "EmployeeId", "dbo.Employees", "EmployeeId");
            AddForeignKey("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence" }, "dbo.InventoryAdjustments", new[] { "AdjustmentDate", "Sequence" });
            AddForeignKey("dbo.InventoryAdjustmentItems", "EmployeeId", "dbo.Employees", "EmployeeId");
            CreateIndex("dbo.Notes", "EmployeeId");
            CreateIndex("dbo.InventoryAdjustments", "EmployeeId");
            CreateIndex("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence" });
            CreateIndex("dbo.InventoryAdjustmentItems", "EmployeeId");
            DropColumn("dbo.Notes", "User");
            DropColumn("dbo.InventoryAdjustments", "User");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InventoryAdjustments", "User", c => c.String(nullable: false, maxLength: 25));
            AddColumn("dbo.Notes", "User", c => c.String(nullable: false, maxLength: 25));
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "EmployeeId" });
            DropIndex("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence" });
            DropIndex("dbo.InventoryAdjustments", new[] { "EmployeeId" });
            DropIndex("dbo.Notes", new[] { "EmployeeId" });
            DropForeignKey("dbo.InventoryAdjustmentItems", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence" }, "dbo.InventoryAdjustments");
            DropForeignKey("dbo.InventoryAdjustments", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Notes", "EmployeeId", "dbo.Employees");
            DropPrimaryKey("dbo.InventoryAdjustments", new[] { "AdjustmentDate", "Sequence" });
            AddPrimaryKey("dbo.InventoryAdjustments", new[] { "TimeStamp", "Sequence" });
            DropColumn("dbo.InventoryAdjustmentItems", "EmployeeId");
            DropColumn("dbo.InventoryAdjustments", "EmployeeId");
            DropColumn("dbo.InventoryAdjustments", "AdjustmentDate");
            DropColumn("dbo.Notes", "EmployeeId");
            
            /// Had to replace commented line with the ones following it since the migration shouldn't just change
            /// the name of the column, but it's data type altogether. -RI 2014/1/27
            //RenameColumn(table: "dbo.InventoryAdjustmentItems", name: "AdjustmentDate", newName: "TimeStamp");
            DropPrimaryKey("dbo.InventoryAdjustmentItems", new[] { "AdjustmentDate", "Sequence", "ItemSequence" });
            DropColumn("dbo.InventoryAdjustmentItems", "AdjustmentDate");
            AddPrimaryKey("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence", "ItemSequence" });

            CreateIndex("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" });
            AddForeignKey("dbo.InventoryAdjustmentItems", new[] { "TimeStamp", "Sequence" }, "dbo.InventoryAdjustments", new[] { "TimeStamp", "Sequence" });
        }
    }
}
