using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface ISetChileProductAttributeRangesParameters : IUserIdentifiable
    {
        string ChileProductKey { get; }

        IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; }
    }
}