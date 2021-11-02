using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdatePackagingProductParameters : IUpdatePackagingProductParameters
    {
        public string UserToken { get; set; }
        public string ProductKey { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public double Weight { get; set; }
        public double PackagingWeight { get; set; }
        public double PalletWeight { get; set; }
    }
}