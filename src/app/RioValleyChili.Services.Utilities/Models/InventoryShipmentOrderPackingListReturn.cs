using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderPackingListReturn : IInventoryShipmentOrderPackingListReturn
    {
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }
        public string OrderKey { get { return InventoryShipmentOrderKeyReturn.InventoryShipmentOrderKey; } }
        public int? MovementNumber { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }
        public int TotalQuantity { get; internal set; }
        public int PalletQuantity { get; internal set; }
        public double? PalletWeight { get; internal set; }
        public double ItemSumPalletWeight { get; internal set; }
        public double TotalGrossWeight { get; internal set; }
        public double TotalNetWeight { get; internal set; }
        public ShippingLabel ShipFromOrSoldToShippingLabel { get; internal set; }
        public ShippingLabel ShipToShippingLabel { get; internal set; }
        public IEnumerable<IPackingListPickedInventoryItem> Items { get; internal set; }

        internal InventoryShipmentOrderKeyReturn InventoryShipmentOrderKeyReturn { get; set; }
    }
}