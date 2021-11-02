using System.Collections.Generic;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerWithProductSpecReturn 
    {
        internal string CustomerName { get; set; }
        internal string CustomerKey { get { return CustomerKeyReturn.CustomerKey; }}

        internal CustomerKeyReturn CustomerKeyReturn { get; set; }
        internal IEnumerable<CustomerChileProductAttributeRangeReturn> AttributeRanges { get; set; }
    }
}