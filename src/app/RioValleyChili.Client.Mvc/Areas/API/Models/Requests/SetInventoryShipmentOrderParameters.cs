using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetInventoryShipmentOrderParameters 
    {
        public string OriginFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? DateOrderReceived { get; set; }
        public string OrderRequestedBy { get; set; }
        public string OrderTakenBy { get; set; }

        public SetShipmentInformationRequestParameter Shipment { get; set; }

        public IEnumerable<SetInventoryPickOrderItemRequest> InventoryPickOrderItems { get; set; }
        public IEnumerable<SetPickedInventoryItemCodesRequestParameter> PickedInventoryItemCodes { get; set; }
    }
}