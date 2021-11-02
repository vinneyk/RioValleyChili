namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChileMaterialsReceived : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DehydratedMaterialsReceived", newName: "ChileMaterialsReceived");
            RenameColumn(table: "dbo.ChileMaterialsReceived", name: "ProductionDate", newName: "DateReceived");

            DropForeignKey("dbo.ChileMaterialsReceived", "DehydratorId", "dbo.Companies");
            RenameColumn(table: "dbo.ChileMaterialsReceived", name: "DehydratorId", newName: "SupplierId");
            AddForeignKey("dbo.ChileMaterialsReceived", new[] { "SupplierId" }, "dbo.Companies", new[] { "Id" });

            AddColumn("dbo.ChileMaterialsReceived", "ChileMaterialsReceivedType", c => c.Int(nullable: false));
            AddColumn("dbo.ChileMaterialsReceived", "TreatmentId", c => c.Int(nullable: false, defaultValue: 0));
            AddForeignKey("dbo.ChileMaterialsReceived", new[] { "TreatmentId" }, "dbo.InventoryTreatments", new[] { "Id" });

            AddColumn("dbo.ChileMaterialsReceived", "LoadNumber", c => c.String(maxLength: 20));
            DropColumn("dbo.ChileMaterialsReceived", "Load");

            RenameTable(name: "dbo.DehydratedMaterialsReceivedItems", newName: "ChileMaterialsReceivedItems");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.ChileMaterialsReceivedItems", newName: "DehydratedMaterialsReceivedItems");

            AddColumn("dbo.ChileMaterialsReceived", "Load", c => c.Int(nullable: false));
            DropColumn("dbo.ChileMaterialsReceived", "LoadNumber");

            DropForeignKey("dbo.ChileMaterialsReceived", "SupplierId", "dbo.Companies");
            RenameColumn(table: "dbo.ChileMaterialsReceived", name: "SupplierId", newName: "DehydratorId");
            AddForeignKey("dbo.ChileMaterialsReceived", new[] { "DehydratorId" }, "dbo.Companies", new[] { "Id" });

            DropForeignKey("dbo.ChileMaterialsReceived", "TreatmentId", "dbo.InventoryTreatments");
            DropColumn("dbo.ChileMaterialsReceived", "TreatmentId");
            DropColumn("dbo.ChileMaterialsReceived", "ChileMaterialsReceivedType");

            RenameColumn(table: "dbo.ChileMaterialsReceived", name: "DateReceived", newName: "ProductionDate");
            RenameTable(name: "dbo.ChileMaterialsReceived", newName: "DehydratedMaterialsReceived");
        }
    }
}
