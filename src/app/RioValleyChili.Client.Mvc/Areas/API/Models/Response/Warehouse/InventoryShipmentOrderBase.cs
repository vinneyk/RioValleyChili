using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public abstract class InventoryShipmentOrderBase<TThis, TPickOrder, TPickOrderItem> : ILinkedResource<TThis>
        where TThis : InventoryShipmentOrderBase<TThis, TPickOrder, TPickOrderItem>
        where TPickOrder : InventoryPickOrderDetailBase<TPickOrderItem>, new()
        where TPickOrderItem : InventoryPickOrderItemResponse, new()
    {
        public string MovementKey { get; set; }
        public DateTime DateCreated { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool IsLocked { get; internal set; }
        public string PurchaseOrderNumber { get; set; }
        public string OrderRequestedBy { get; set; }
        public string OrderTakenBy { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? DateOrderReceived { get; set; }
        public int? MoveNum { get; set; }

        public FacilityResponse DestinationFacility { get; set; }
        public FacilityResponse OriginFacility { get; set; }
        public ShipmentDetails Shipment { get; set; }
        public TPickOrder PickOrder { get; set; }
        public PickedInventory PickedInventory { get; set; }

        public ResourceLinkCollection Links { get; set; }
        public string HRef { get { return Links.SelfHRef; } }
        public TThis Data { get { return (TThis)this; } }
    }
}