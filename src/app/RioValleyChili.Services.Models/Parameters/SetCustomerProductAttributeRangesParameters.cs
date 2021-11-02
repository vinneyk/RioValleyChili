using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetCustomerProductAttributeRangesParameters : ISetCustomerProductAttributeRangesParameters
    {
        public string UserToken { get; set; }
        public string CustomerKey { get; set; }
        public string ChileProductKey { get; set; }
        public IEnumerable<SetCustomerProductAttributeRangeParameters> AttributeRanges { get; set; }

        IEnumerable<ISetCustomerProductAttributeRangeParameters> ISetCustomerProductAttributeRangesParameters.AttributeRanges { get { return AttributeRanges; } }
    }

    public class SetCustomerProductAttributeRangeParameters : ISetCustomerProductAttributeRangeParameters
    {
        public string AttributeNameKey { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
    }
}