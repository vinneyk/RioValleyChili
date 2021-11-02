using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.InventoryService
{
    public interface IReceiveInventoryParameters : IUserIdentifiable
    {
        LotTypeEnum LotType { get; }
        string ProductKey { get; }
        string PackagingReceivedKey { get; }
        string VendorKey { get; }
        string PurchaseOrderNumber { get; }
        string ShipperNumber { get; }
        IEnumerable<IReceiveInventoryItemParameters> Items { get; }

        DateTime? LotDate { get; }
        int? LotSequence { get; }
    }
}
