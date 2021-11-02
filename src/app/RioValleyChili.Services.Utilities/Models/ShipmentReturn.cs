using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ShipmentReturn : IShipmentDetailReturn
    {
        public string ShipmentKey { get { return ShipmentInformationKeyReturn.ShipmentInfoKey; } }
        public int PalletQuantity { get; set; }
        public double? PalletWeight { get; set; }
        public DateTime? ShipmentDate { get; set; }

        public IShippingInstructions ShippingInstructions { get; set; }
        public ITransitInformation TransitInformation { get; set; }
        public InventoryOrderEnum InventoryOrderEnum { get; set; }
        public ShipmentStatus Status { get; set; }

        internal ShipmentInformationKeyReturn ShipmentInformationKeyReturn { get; set; }

    }
}
