using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInternalOrderAcknowledgementReturn
    {
        string OrderKey { get; }
        int? MovementNumber { get; }
        InventoryShipmentOrderTypeEnum OrderType { get; }
        string PurchaseOrderNumber { get; }
        int TotalQuantity { get; }
        double NetWeight { get; }
        double TotalGrossWeight { get; }
        double PalletWeight { get; }
        double EstimatedShippingWeight { get; }
        DateTime? DateReceived { get; }
        string RequestedBy { get; }
        string TakenBy { get; }
        string OriginFacility { get; }

        IShipmentInformationReturn ShipmentInformation { get; }
        ISalesOrderInternalAcknowledgementReturn SalesOrder { get; }

        IEnumerable<IPickOrderItemReturn> PickOrderItems { get; }
        IEnumerable<ICustomerNotesReturn> CustomerNotes { get; }
    }
}