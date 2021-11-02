using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryShipmentOrderExtensions
    {
        internal static InventoryShipmentOrder EmptyItems(this InventoryShipmentOrder order)
        {
            if(order == null) { throw new ArgumentNullException("order "); }

            if(order.InventoryPickOrder != null)
            {
                order.InventoryPickOrder.EmptyItems();
                if(order.InventoryPickOrder.PickedInventory != null)
                {
                    order.InventoryPickOrder.PickedInventory.EmptyItems();
                }
            }

            if(order.PickedInventory != null)
            {
                order.PickedInventory.EmptyItems();
            }

            return order;
        }

        internal static InventoryShipmentOrder SetSourceFacility(this InventoryShipmentOrder order, IFacilityKey facilityKey)
        {
            if(order == null) { throw new ArgumentNullException("order"); }

            order.SourceFacility = null;
            order.SourceFacilityId = facilityKey.FacilityKey_Id;

            return order;
        }

        internal static SetOrderHeaderParameters ToOrderHeaderParameters(this InventoryShipmentOrder order, Action<SetOrderHeaderParameters> initialize = null)
        {
            var parameters = new SetOrderHeaderParameters
                {
                    CustomerPurchaseOrderNumber = order.PurchaseOrderNumber,
                    DateOrderReceived = order.DateReceived,
                    OrderTakenBy = order.TakenBy
                };

            if(initialize != null)
            {
                initialize(parameters);
            }

            return parameters;
        }
    }
}