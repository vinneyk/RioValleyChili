using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateAdditiveProductParameters : ICreateAdditiveProductParameters
    {
        public string UserToken { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string AdditiveTypeKey { get; set; }
    }
}