using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ShipmentOrderSummaryReturn : IShipmentOrderSummaryReturn
    {
        public string InventoryShipmentOrderKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }
        public ShipmentStatus Status { get; internal set; }
        public double? PalletWeight { get; internal set; }
        public int PalletQuantity { get; internal set; }
        public double TotalWeightOrdered { get; internal set; }
        public double TotalWeightPicked { get; internal set; }
        public ITransitInformation TransitInformation { get; internal set; }
        public IShippingInstructions ShippingInstructions { get; internal set; }

        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }
    }
}