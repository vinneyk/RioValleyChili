using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetCustomerProductAttributeRangesParameters : ISetCustomerProductAttributeRangesParameters
    {
        public string UserToken { get; set; }
        public string CustomerKey { get; set; }
        public string ChileProductKey { get; set; }
        public IEnumerable<ISetCustomerProductAttributeRangeParameters> AttributeRanges { get; set; }
    }

    public class SetCustomerProductAttributeRangeParameters : ISetCustomerProductAttributeRangeParameters
    {
        public string AttributeNameKey { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
    }
}