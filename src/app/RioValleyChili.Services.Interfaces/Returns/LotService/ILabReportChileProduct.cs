using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILabReportChileProduct : IProductReturn
    {
        IDictionary<string, IProductAttributeRangeReturn> AttributeRanges { get; }
    }
}