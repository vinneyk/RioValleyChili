using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInventoryShipmentOrderPickSheetReturn
    {
        InventoryShipmentOrderTypeEnum OrderType { get; }
        string OrderKey { get; }
        int? MovementNumber { get; }
        string PurchaseOrderNumber { get; }

        ShippingLabel ShipFromOrSoldToShippingLabel { get; }

        IShipmentInformationReturn ShipmentInformation { get; }
        IEnumerable<IPickSheetItemReturn> Items { get; }
        IEnumerable<ICustomerNotesReturn> CustomerNotes { get; }
    }
}