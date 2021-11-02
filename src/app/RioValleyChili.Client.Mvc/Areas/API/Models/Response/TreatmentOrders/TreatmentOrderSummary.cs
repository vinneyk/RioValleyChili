using System;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.TreatmentOrders
{
    public class TreatmentOrderSummary : InterWarehouseOrderSummary, ITreatmentOrderAPIModel
    {
        public DateTime? Returned { get; set; }
        public InventoryTreatmentResponse InventoryTreatment { get; set; }

        public bool EnableReturnFromTreatment { get; set; }
        public string StatusDisplayText { get { return this.GetStatusDisplayText(); } }

        public ShipmentStatus ShipmentStatus { get { return Shipment.Status; } }
    }
}