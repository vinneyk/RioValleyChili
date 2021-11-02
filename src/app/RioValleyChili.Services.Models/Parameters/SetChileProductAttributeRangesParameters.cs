using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetChileProductAttributeRangesParameters : ISetChileProductAttributeRangesParameters
    {
        public string UserToken { get; set; }
        public string ChileProductKey { get; set; }

        public IEnumerable<SetAttributeRangeParameters> AttributeRanges { get; set; }

        IEnumerable<ISetAttributeRangeParameters> ISetChileProductAttributeRangesParameters.AttributeRanges { get { return AttributeRanges; } }
    }
}