using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class PickedInventoryParameters
    {
        internal InventoryKey InventoryKey { get; set; }
        internal LocationKey CurrentLocationKey { get; set; }
        internal InventoryPickOrderItemKey OrderItemKey { get; set; }
        internal int Quantity { get; set; }
        internal string CustomerLotCode { get; set; }
        internal string CustomerProductCode { get; set; }

        internal PickedInventoryParameters(IInventoryKey inventoryKey, int quantity, string customerLotCode, string customerProductCode)
        {
            InventoryKey = new InventoryKey(inventoryKey);
            CurrentLocationKey = new LocationKey(InventoryKey);
            Quantity = quantity;
            CustomerLotCode = customerLotCode;
            CustomerProductCode = customerProductCode;
        }

        internal PickedInventoryParameters(IInventoryPickOrderItemKey orderItemKey, IInventoryKey inventoryKey, int quantity, string customerLotCode, string customerProductCode)
            : this(inventoryKey, quantity, customerLotCode, customerProductCode)
        {
            if(orderItemKey != null)
            {
                OrderItemKey = new InventoryPickOrderItemKey(orderItemKey);
            }
        }

        internal PickedInventoryParameters(PickedInventoryItem item)
            : this(item, item.Quantity, item.CustomerLotCode, item.CustomerProductCode) { }

        internal bool Match(PickedInventoryParameters b)
        {
            return InventoryKey.Equals(b.InventoryKey) && CurrentLocationKey.Equals(b.CurrentLocationKey) && CustomerLotCode == b.CustomerLotCode && CustomerProductCode == b.CustomerProductCode &&
                (OrderItemKey == null ? b.OrderItemKey == null : OrderItemKey.Equals(b.OrderItemKey));
        }
    }
}