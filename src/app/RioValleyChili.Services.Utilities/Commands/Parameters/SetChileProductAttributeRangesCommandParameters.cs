using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetChileProductAttributeRangesCommandParameters
    {
        internal ISetChileProductAttributeRangesParameters Parameters { get; set; }

        internal ChileProductKey ChileProductKey { get; set; }

        internal IEnumerable<SetChileProductAttributeRangeParameters> AttributeRanges { get; set; }
    }

    internal class SetChileProductAttributeRangeParameters
    {
        internal ISetAttributeRangeParameters Parameters { get; set; }

        internal AttributeNameKey AttributeNameKey { get; set; }
    }
}