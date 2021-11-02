using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class ReceiveInventoryItemParameters : IReceiveInventoryItemParameters
    {
        public int Quantity { get; set; }
        public string PackagingProductKey { get; set; }
        public string WarehouseLocationKey { get; set; }
        public string TreatmentKey { get; set; }
        public string ToteKey { get; set; }
    }
}