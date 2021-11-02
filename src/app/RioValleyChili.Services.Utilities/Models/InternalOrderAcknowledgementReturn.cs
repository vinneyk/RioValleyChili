using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InternalOrderAcknowledgementReturn : IInternalOrderAcknowledgementReturn
    {
        public string OrderKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public int? MovementNumber { get; internal set; }
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }
        public int TotalQuantity { get; internal set; }
        public double NetWeight { get; internal set; }
        public double TotalGrossWeight { get; internal set; }
        public double PalletWeight { get; internal set; }
        public double EstimatedShippingWeight { get; internal set; }
        public DateTime? DateReceived { get; internal set; }
        public string RequestedBy { get; internal set; }
        public string TakenBy { get; internal set; }
        public string OriginFacility { get; internal set; }

        public IShipmentInformationReturn ShipmentInformation { get; set; }
        public ISalesOrderInternalAcknowledgementReturn SalesOrder { get; set; }

        public IEnumerable<IPickOrderItemReturn> PickOrderItems { get; set; }
        public IEnumerable<ICustomerNotesReturn> CustomerNotes { get; internal set; }

        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }
    }
}