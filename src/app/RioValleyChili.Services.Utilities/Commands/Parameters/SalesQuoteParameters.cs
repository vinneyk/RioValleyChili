using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SalesQuoteParameters
    {
        internal ISalesQuoteParameters Parameters { get; set; }

        internal int? SalesQuoteNumber { get; set; }
        internal FacilityKey SourceFacilityKey { get; set; }
        internal CustomerKey CustomerKey { get; set; }
        internal CompanyKey BrokerKey { get; set; }

        internal IEnumerable<SalesQuoteItemParameters> Items { get; set; }
    }

    internal class SalesQuoteItemParameters
    {
        internal ISalesQuoteItemParameters Parameters { get; set; }
        internal SalesQuoteItemKey SalesQuoteItemKey { get; set; }
        internal ProductKey ProductKey { get; set; }
        internal PackagingProductKey PackagingProductKey { get; set; }
        internal InventoryTreatmentKey InventoryTreatmentKey { get; set; }
    }
}