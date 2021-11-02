using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerOrderPickedItemExtensions
    {
        internal static SalesOrderPickedItem ConstrainByKeys(this SalesOrderPickedItem pickedItem, ISalesOrderKey salesOrderKey)
        {
            if(pickedItem == null) { throw new ArgumentNullException("pickedItem"); }
            if(salesOrderKey == null) { throw new ArgumentNullException("salesOrderKey"); }

            pickedItem.SalesOrder = null;
            pickedItem.DateCreated = salesOrderKey.SalesOrderKey_DateCreated;
            pickedItem.Sequence = salesOrderKey.SalesOrderKey_Sequence;

            if(pickedItem.PickedInventoryItem != null)
            {
                pickedItem.PickedInventoryItem.PickedInventory = null;
            }

            if(pickedItem.SalesOrderItem != null)
            {
                pickedItem.SalesOrderItem.ConstrainByKeys(salesOrderKey);
            }

            return pickedItem;
        }

        internal static SalesOrderPickedItem SetToCustomerOrderItem(this SalesOrderPickedItem pickedItem, SalesOrderItem orderItem)
        {
            if(pickedItem == null) { throw new ArgumentNullException("orderItem"); }
            if(orderItem == null) { throw new ArgumentNullException("orderItem"); }

            pickedItem.SalesOrder = null;
            pickedItem.DateCreated = orderItem.DateCreated;
            pickedItem.Sequence = orderItem.Sequence;

            pickedItem.SalesOrderItem = null;
            pickedItem.OrderItemSequence = orderItem.ItemSequence;

            pickedItem.PickedInventoryItem.ConstrainByKeys(pickedItem, null, orderItem.InventoryPickOrderItem, orderItem.InventoryPickOrderItem);

            return pickedItem;
        }
    }
}