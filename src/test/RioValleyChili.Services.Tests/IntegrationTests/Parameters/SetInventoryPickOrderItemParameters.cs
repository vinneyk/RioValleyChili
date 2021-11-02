using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetInventoryPickOrderItemParameters : ISetInventoryPickOrderItemParameters
    {
        public string ProductKey { get; set; }
        public string PackagingKey { get; set; }
        public string TreatmentKey { get; set; }
        public string CustomerKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerProductCode { get; set; }
        public string CustomerLotCode { get; set; }
    }
}