using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetInventoryShipmentInformationParameters : ISetInventoryShipmentInformationParameters
    {
        public SetInventoryShipmentInformationParameters() { }

        internal SetInventoryShipmentInformationParameters(IShipmentDetailReturn detailReturn)
        {
            PalletWeight = detailReturn.PalletWeight;
            PalletQuantity = detailReturn.PalletQuantity;
            InventoryShipmentOrderKey = detailReturn.ShipmentKey;

            ShippingInstructions = detailReturn.ShippingInstructions != null ? new SetShippingInstructionsParameters(detailReturn.ShippingInstructions) : null;
            TransitInformation = detailReturn.TransitInformation != null ? new SetTransitInformationParameter(detailReturn.TransitInformation) : null;
        }

        public string InventoryShipmentOrderKey { get; set; }
        public double? PalletWeight { get; set; }
        public int PalletQuantity { get; set; }
        
        public SetShippingInstructionsParameters ShippingInstructions { get; set; }
        public SetTransitInformationParameter TransitInformation { get; set; }

        #region Explicit Interface Implementations

        ISetShippingInstructions ISetShipmentInformation.ShippingInstructions { get { return ShippingInstructions; } }
        ISetTransitInformation ISetShipmentInformation.TransitInformation { get { return TransitInformation; } }

        #endregion
    }
}