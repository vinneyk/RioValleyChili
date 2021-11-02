using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderPickSheetReturn : IInventoryShipmentOrderPickSheetReturn
    {
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }
        public string OrderKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public int? MovementNumber { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }

        public ShippingLabel ShipFromOrSoldToShippingLabel
        {
            get
            {
                return _shipFromOrSoldToShippingLabel ?? ShipmentInformation.ShippingInstructions.ShipFromOrSoldToShippingLabel;
            }
            internal set { _shipFromOrSoldToShippingLabel = value; }
        }

        public IShipmentInformationReturn ShipmentInformation { get; internal set; }
        public IEnumerable<IPickSheetItemReturn> Items { get; internal set; }
        public IEnumerable<ICustomerNotesReturn> CustomerNotes { get; internal set; }
        
        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }

        private ShippingLabel _shipFromOrSoldToShippingLabel;
    }
}