using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetPickedInventoryItemParameters : IPickedInventoryItemParameters
    {
        public string OrderItemKey { get; set; }
        public string InventoryKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
    }
}