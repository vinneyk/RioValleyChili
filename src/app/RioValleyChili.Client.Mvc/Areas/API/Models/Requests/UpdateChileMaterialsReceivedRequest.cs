using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateChileMaterialsReceivedRequest
    {
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }
        public string ChileProductKey { get; set; }
        public string TreatmentKey { get; set; }
        public string SupplierKey { get; set; }

        public IEnumerable<UpdateChileMaterialsReceivedItemRequest> Items { get; set; }
    }
}