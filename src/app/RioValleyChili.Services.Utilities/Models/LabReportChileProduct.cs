using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LabReportChileProduct : ProductBaseReturn, ILabReportChileProduct
    {
        public IDictionary<string, IProductAttributeRangeReturn> AttributeRanges
        {
            get { return _attributeRanges ?? (_attributeRanges = AttributeRangeReturns.ToDictionary(a => a.AttributeNameKey, a => a)); }
        }
        private IDictionary<string, IProductAttributeRangeReturn> _attributeRanges;

        internal IEnumerable<IProductAttributeRangeReturn> AttributeRangeReturns { get; set; }
    }
}