using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerChileProductAttributeRangesReturn
    {
        string CustomerKey { get; }
        IChileProductReturn ChileProduct { get; }

        IEnumerable<ICustomerChileProductAttributeRangeReturn> AttributeRanges { get; }
    }
}