using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInventoryShipmentOrderCertificateOfAnalysisReturn
    {
        string DestinationName { get; }
        string OrderKey { get; }
        int? MovementNumber { get; }
        string PurchaseOrderNumber { get; }
        DateTime? ShipmentDate { get; }
        InventoryShipmentOrderTypeEnum OrderType { get; }

        IEnumerable<IInventoryShipmentOrderItemAnalysisReturn> Items { get; }
    }
}