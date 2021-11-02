using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInventoryShipmentOrderBillOfLadingReturn
    {
        InventoryShipmentOrderTypeEnum OrderType { get; }
        string OrderKey { get; }
        int? MoveNum { get; }
        int TotalQuantity { get; }
        double PalletWeight { get; }
        double TotalGrossWeight { get; }
        double TotalNetWeight { get; }
        string PurchaseOrderNumber { get; }
        string SourceFacilityLabelName { get; }
        Address ShipperAddress { get; }

        IShipmentInformationReturn ShipmentInformation { get; }
        IEnumerable<IPackingListPickedInventoryItem> Items { get; }
    }
}