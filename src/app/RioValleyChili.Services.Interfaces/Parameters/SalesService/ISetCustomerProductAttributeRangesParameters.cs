using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ISetCustomerProductAttributeRangesParameters : IUserIdentifiable
    {
        string CustomerKey { get; }
        string ChileProductKey { get; }
        IEnumerable<ISetCustomerProductAttributeRangeParameters> AttributeRanges { get; }
    }

    public interface ISetCustomerProductAttributeRangeParameters
    {
        string AttributeNameKey { get; }
        double RangeMin { get; }
        double RangeMax { get; }
    }
}