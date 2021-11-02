using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class ReceiveInventoryDto
    {
        public LotTypeEnum LotType { get; set; }
        public string ProductKey { get; set; }
        public string PackagingReceivedKey { get; set; }
        public string VendorKey { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ShipperNumber { get; set; }

        public IEnumerable<ReceiveInventoryItemDto> Items { get; set; }

        public DateTime? LotDate { get; set; }
        public int? LotSequence { get; set; }

        public class ReceiveInventoryItemDto
        {
            public int Quantity { get; set; }
            public string PackagingProductKey { get; set; }
            public string WarehouseLocationKey { get; set; }
            public string TreatmentKey { get; set; }
            public string ToteKey { get; set; }
        }
    }
}