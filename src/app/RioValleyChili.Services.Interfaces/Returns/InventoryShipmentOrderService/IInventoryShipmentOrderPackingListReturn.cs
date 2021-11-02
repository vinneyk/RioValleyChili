using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInventoryShipmentOrderPackingListReturn
    {
        InventoryShipmentOrderTypeEnum OrderType { get; }
        string OrderKey { get; }
        int? MovementNumber { get; }
        DateTime? ShipmentDate { get; }
        string PurchaseOrderNumber { get; }

        int TotalQuantity { get; }
        int PalletQuantity { get; }
        double? PalletWeight { get; }
        double ItemSumPalletWeight { get; }
        double TotalGrossWeight { get; }
        double TotalNetWeight { get; }

        ShippingLabel ShipFromOrSoldToShippingLabel { get; }
        ShippingLabel ShipToShippingLabel { get; }

        IEnumerable<IPackingListPickedInventoryItem> Items { get; }
    }
}