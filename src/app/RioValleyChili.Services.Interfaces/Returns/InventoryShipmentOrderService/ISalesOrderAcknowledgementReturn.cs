using System;
using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ISalesOrderAcknowledgementReturn
    {
        string OrderKey { get; }
        int? MovementNumber { get; }
        string PurchaseOrderNumber { get; }
        int TotalQuantity { get; }
        double NetWeight { get; }
        double TotalGrossWeight { get; }
        double PalletWeight { get; }
        double EstimatedShippingWeight { get; }
        DateTime? DateReceived { get; }
        string RequestedBy { get; }
        string TakenBy { get; }
        string PaymentTerms { get; }
        string Broker { get; }
        string OriginFacility { get; }

        ShippingLabel SoldToShippingLabel { get; }

        IShipmentInformationReturn ShipmentInformation { get; }
        IEnumerable<ISalesOrderItemReturn> PickOrderItems { get; }
    }
}