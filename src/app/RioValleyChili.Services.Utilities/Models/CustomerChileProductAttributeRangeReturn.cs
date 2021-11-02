using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerChileProductAttributeRangeReturn : ICustomerChileProductAttributeRangeReturn, IAttributeRange
    {
        public string CustomerChileProductAttributeRangeKey { get { return new CustomerProductAttributeRangeKey(KeyReturn); } }
        public string AttributeShortName { get; internal set; }
        public double RangeMin { get; internal set; }
        public double RangeMax { get; internal set; }
        public bool Active { get; set; }
        public string AttributeNameKey_ShortName { get { return KeyReturn.AttributeNameKey_ShortName; } }

        internal CustomerChileProductAttributeRangeKeyReturn KeyReturn { get; set; }
    }
}