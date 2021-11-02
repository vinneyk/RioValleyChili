using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetAttributeRangeParameters : ISetAttributeRangeParameters
    {
        public string AttributeNameKey { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
    }
}