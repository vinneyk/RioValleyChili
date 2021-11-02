namespace RioValleyChili.Client.Mvc.Core.Security
{
    public static class ClaimTypes
    {
        public const string Prerelease = "prerelease";
        public const string Solutionhead = "solutionhead";
        public const string SuperUser = "superuser";
        public const string UserToken = "UserToken";
        public const string SessionToken = "SessionToken";

        public const string Sales = "Sales";
        public const string Vendors = "Vendors";


        public static class Admin
        {
            public const string KillswitchDisengage = "KillswitchDisengage";
        }

        public static class QualityControlClaimTypes
        {
            public const string QualityControl = "QualityControl";
            public const string QAHolds = "QAHolds";
            public const string LotAttributes = "LabResults";
            public const string LotStatus = "LotStatus";
            public const string DefectResolutions = "DefectResolutions";
            public const string CustomerProductSpec = "CustomerProductSpec";
        }

        public static class InventoryClaimTypes
        {
            public const string Inventory = "Inventory";
            public const string LotInventory = "LotInventory";
            public const string LotHistory = "LotHistory";
            public const string ReceiveInventory = "ReceiveInventory";
            public const string InventoryTreatments = "InventoryTreatments";
            public const string InventoryAdjustments = "InventoryAdjustments";
            public const string DehydratedMaterials = "DehydratedMaterials";
        }

        public static class ProductionClaimTypes
        {
            public const string Production = "Production";
            public const string PackSchedules = "PackSchedules";
            public const string ProductionSchedules = "ProductionSchedules";
            public const string ProductionResults = "ProductionResults";
            public const string MillAndWetdown = "MillAndWetdown";
            public const string ProductionBatch = "ProductionBatch";
        }

        public static class SalesClaimTypes
        {
            public const string CustomerContracts = "CustomerContracts";
            public const string SalesReports = "SalesReports";
        }

        public static class WarehouseClaimTypes
        {
            public const string IntrawarehouseMovements = "IntrawarehouseMovements";
            public const string InterwarehouseMovements = "InterwarehouseMovements";
            public const string WarehouseLocations = "WarehouseLocations";
            public const string Warehouses = "Warehouses";
            public const string Shipments = "Warehouses";
            public const string TreatmentOrders = "TreatmentOrders";
        }
    }
}