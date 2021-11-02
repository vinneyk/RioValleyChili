using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerChileProductAttributeRangeKeyReturn : ICustomerProductAttributeRangeKey
    {
        public int CustomerKey_Id { get; internal set; }
        public int ChileProductKey_ProductId { get; internal set; }
        public string AttributeNameKey_ShortName { get; internal set; }
    }
}