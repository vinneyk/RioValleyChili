using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class MillAndWetdownResultItemParameters : IMillAndWetdownResultItemParameters
    {
        public string LocationKey { get; set; }
        public int Quantity { get; set; }
        public string PackagingProductKey { get; set; }
    }
}