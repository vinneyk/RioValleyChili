using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetCustomerProductAttributeRangesParameters
    {
        public ISetCustomerProductAttributeRangesParameters Parameters { get; set; }

        public CustomerKey CustomerKey { get; set; }
        public ChileProductKey ChileProductKey { get; set; }

        public IEnumerable<SetCustomerProductAttributeRangeParameters> AttributeRanges { get; set; }
    }

    internal class SetCustomerProductAttributeRangeParameters
    {
        public ISetCustomerProductAttributeRangeParameters Parameters { get; set; }

        public AttributeNameKey AttributeNameKey { get; set; }
    }
}