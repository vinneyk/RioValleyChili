using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerChileProductAttributeRangesReturn : ICustomerChileProductAttributeRangesReturn
    {
        public string CustomerKey { get { return CustomerKeyReturn.CustomerKey; } }

        public IEnumerable<ICustomerChileProductAttributeRangeReturn> AttributeRanges { get; internal set; }

        internal ChileProductReturn ChileProduct { get; set; }
        internal CustomerKeyReturn CustomerKeyReturn { get; set; }

        IChileProductReturn ICustomerChileProductAttributeRangesReturn.ChileProduct { get { return ChileProduct; } }
    }
}