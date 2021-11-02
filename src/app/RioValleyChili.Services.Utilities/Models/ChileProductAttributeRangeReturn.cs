using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileProductAttributeRangeReturn : IProductAttributeRangeReturn, IAttributeRange
    {
        public string AttributeNameKey { get; set; }

        public string AttributeName { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        string IAttributeNameKey.AttributeNameKey_ShortName { get { return AttributeNameKey; } }
        public double RangeMin { get { return MinValue; } set { MinValue = value; } }
        public double RangeMax { get { return MaxValue; } set { MaxValue = value; } }
    }
}