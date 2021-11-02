using System;

namespace RioValleyChili.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SalesOrders : DbMigration
    {
        public override void Up()
        {
            //LotStatusIndeces
            CreateIndex("dbo.Lots", "QualityStatus");
            CreateIndex("dbo.Lots", "ProductionStatus");

            //NullableDestinationFacility
            DropIndex("dbo.InventoryShipmentOrders", new[] { "DestinationFacilityId" });
            DropDefaultConstraint("dbo.InventoryShipmentOrders", "DestinationFacilityId");
            AlterColumn("dbo.InventoryShipmentOrders", "DestinationFacilityId", c => c.Int(nullable: true));
            CreateIndex("dbo.InventoryShipmentOrders", "DestinationFacilityId");

            //CustomerOrderPickedItem_EntryDate_Remove
            DropColumn("dbo.CustomerOrderPickedItems", "EntryDate");
        }

        public override void Down()
        {
            //CustomerOrderPickedItem_EntryDate_Remove
            AddColumn("dbo.CustomerOrderPickedItems", "EntryDate", c => c.DateTime());

            //NullableDestinationFacility
            DropIndex("dbo.InventoryShipmentOrders", new[] { "DestinationFacilityId" });
            DropDefaultConstraint("dbo.InventoryShipmentOrders", "DestinationFacilityId");
            AlterColumn("dbo.InventoryShipmentOrders", "DestinationFacilityId", c => c.Int(nullable: false, defaultValue: 0));
            CreateIndex("dbo.InventoryShipmentOrders", "DestinationFacilityId");

            //LotStatusIndeces
            DropIndex("dbo.Lots", new[] { "ProductionStatus" });
            DropIndex("dbo.Lots", new[] { "QualityStatus" });
        }

        // http://stackoverflow.com/questions/11974439/changing-column-default-values-in-ef5-code-first
        // Because EF AlterColumn won't actually remove existing default value constraints, which keeps defaulting DestinationFacilityId to 0 instead of null.
        private void DropDefaultConstraint(string tableName, string columnName)
        {
            var constraintVariableName = string.Format("@constraint_{0}", Guid.NewGuid().ToString("N"));

            var sql = string.Format(@"
            DECLARE {0} nvarchar(128)
            SELECT {0} = name
            FROM sys.default_constraints
            WHERE parent_object_id = object_id(N'{1}')
            AND col_name(parent_object_id, parent_column_id) = '{2}';
            IF {0} IS NOT NULL
                EXECUTE('ALTER TABLE {1} DROP CONSTRAINT [' + {0} + ']')",
            constraintVariableName,
            tableName,
            columnName);

            Sql(sql);
        }
    }
}
