using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderBillOfLadingReturn : IInventoryShipmentOrderBillOfLadingReturn
    {
        //todo: Are PaymentTerms == ShipmentInformation.TransitInformation.FreightType? -RI 2015/04/15
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }
        public string OrderKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public int? MoveNum { get; internal set; }
        public int TotalQuantity { get; internal set; }
        public double PalletWeight { get; internal set; }
        public double TotalGrossWeight { get; internal set; }
        public double TotalNetWeight { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }
        public string SourceFacilityLabelName { get; internal set; }
        public Address ShipperAddress { get { return OrderType == InventoryShipmentOrderTypeEnum.SalesOrder ? SourceFacilityAddress : ShipmentInformation.ShippingInstructions.ShipFromOrSoldToShippingLabel.Address; } }
        public IShipmentInformationReturn ShipmentInformation { get; internal set; }
        public IEnumerable<IPackingListPickedInventoryItem> Items { get; internal set; }

        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }
        internal Address SourceFacilityAddress { get; set; }
        
    }
}