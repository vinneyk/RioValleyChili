using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class TreatmentOrderDetailReturn : InventoryShipmentOrderDetailReturn, ITreatmentOrderDetailReturn
    {
        public DateTime? Returned { get; set; }
        public IInventoryTreatmentReturn InventoryTreatment { get; internal set; }
        public override InventoryOrderEnum InventoryOrderEnum { get { return InventoryOrderEnum.Treatments; } }
    }

    internal class TreatmentOrderSummaryReturn : InventoryShipmenOrderSummaryReturn, ITreatmentOrderSummaryReturn
    {
        public DateTime? Returned { get; set; }
        public IInventoryTreatmentReturn InventoryTreatment { get; internal set; }
    }
}