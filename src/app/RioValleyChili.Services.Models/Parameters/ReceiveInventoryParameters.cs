using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class ReceiveInventoryParameters : IReceiveInventoryParameters
    {
        public string UserToken { get; set; }
        public LotTypeEnum LotType { get; set; }
        public string ProductKey { get; set; }
        public string PackagingReceivedKey { get; set; }
        public string VendorKey { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ShipperNumber { get; set; }
        public IEnumerable<ReceiveInventoryItemParameters> Items { get; set; }

        IEnumerable<IReceiveInventoryItemParameters> IReceiveInventoryParameters.Items { get { return Items; } }

        public DateTime? LotDate { get; set; }
        public int? LotSequence { get; set; }
    }
}