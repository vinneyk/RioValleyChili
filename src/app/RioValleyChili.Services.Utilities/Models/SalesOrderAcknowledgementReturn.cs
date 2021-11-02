using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderAcknowledgementReturn : ISalesOrderAcknowledgementReturn
    {
        public string OrderKey { get { return SalesOrderKeyReturn.CustomerOrderKey; } }
        public int? MovementNumber { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }
        public int TotalQuantity { get; internal set; }
        public double NetWeight { get; internal set; }
        public double TotalGrossWeight { get; internal set; }
        public double PalletWeight { get; internal set; }
        public double EstimatedShippingWeight { get; internal set; }
        public DateTime? DateReceived { get; internal set; }
        public string RequestedBy { get; internal set; }
        public string TakenBy { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public string Broker { get; internal set; }
        public string OriginFacility { get; internal set; }

        public ShippingLabel SoldToShippingLabel { get; internal set; }

        public IShipmentInformationReturn ShipmentInformation { get; internal set; }
        public IEnumerable<ISalesOrderItemReturn> PickOrderItems { get; internal set; }

        internal SalesOrderKeyReturn SalesOrderKeyReturn { get; set; }
    }
}