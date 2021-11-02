using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class InventoryAdjustmentParameters : IInventoryAdjustmentParameters
    {
        public string LotKey { get; set; }
        public string WarehouseLocationKey { get; set; }
        public string PackagingProductKey { get; set; }
        public string TreatmentKey { get; set; }
        public string ToteKey { get; set; }
        public int Adjustment { get; set; }
    }
}