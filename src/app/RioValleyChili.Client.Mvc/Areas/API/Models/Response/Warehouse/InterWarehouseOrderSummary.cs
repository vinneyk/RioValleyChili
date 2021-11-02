using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class InterWarehouseOrderSummary
    {
        public string MovementKey { get; set; }
        public string MoveNum { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public InventoryPickOrderSummary PickOrder { get; set; }
        public PickedInventorySummary PickedInventory { get; set; }
        public ShipmentSummary Shipment { get; set; }
        public FacilityResponse DestinationFacility { get; set; }
        public FacilityResponse OriginFacility { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}