using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class ReceiveInventoryParameters
    {
        internal IReceiveInventoryParameters Parameters { get; set; }

        internal ProductKey ProductKey { get; set; }
        internal PackagingProductKey PackagingReceivedKey { get; set; }
        internal CompanyKey VendorKey { get; set; }
        internal IEnumerable<ReceiveInventoryItemParameters> Items { get; set; }
    }
}