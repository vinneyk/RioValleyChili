using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class MillAndWetdownPickedItemParameters : IMillAndWetdownPickedItemParameters
    {
        public string InventoryKey { get; set; }
        public int Quantity { get; set; }
    }
}