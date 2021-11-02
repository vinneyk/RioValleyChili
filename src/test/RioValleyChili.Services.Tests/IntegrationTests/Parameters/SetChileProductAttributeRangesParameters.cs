using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetChileProductAttributeRangesParameters : ISetChileProductAttributeRangesParameters
    {
        public string UserToken { get; set; }
        public string ChileProductKey { get; set; }
        public IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; set; }
    }
}