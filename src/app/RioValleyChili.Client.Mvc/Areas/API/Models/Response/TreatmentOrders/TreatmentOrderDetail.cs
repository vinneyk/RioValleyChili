using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders
{
    public class TreatmentOrderDetail : InventoryShipmentOrderBase<TreatmentOrderDetail, InventoryPickOrderDetail, Inventory.InventoryPickOrderItemResponse>, ITreatmentOrderAPIModel
    {
        public DateTime? Returned { get; set; }
        public InventoryTreatmentResponse InventoryTreatment { get; set; }

        public bool EnableReturnFromTreatment { get; set; }
        public string StatusDisplayText { get { return this.GetStatusDisplayText(); } }

        ShipmentStatus ITreatmentOrderAPIModel.ShipmentStatus { get { return Shipment.Status; } }
    }
}