using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class ModifySalesOrderPickedInventoryItemParameters : ModifyPickedInventoryItemParameters
    {
        public SalesOrderPickedItemKey SalesOrderPickedItemKey { get; set; }
        public SalesOrderItemKey SalesOrderItemKey { get; set; }

        public ModifySalesOrderPickedInventoryItemParameters(IInventoryPickOrderItemKey orderItemKey, IInventoryKey inventoryKey, int quantity, string customerLotCode, string customerProductCode)
            : base(inventoryKey, null, quantity, customerLotCode, customerProductCode)
        {
            if(orderItemKey != null)
            {
                SalesOrderItemKey = new SalesOrderItemKey(orderItemKey);
            }
        }

        public ModifySalesOrderPickedInventoryItemParameters(SalesOrderPickedItem pickedItem) : base(pickedItem.PickedInventoryItem)
        {
            SalesOrderPickedItemKey = new SalesOrderPickedItemKey((ISalesOrderPickedItemKey) pickedItem);
            if(pickedItem.OrderItemSequence != null)
            {
                SalesOrderItemKey = new SalesOrderItemKey(pickedItem);
            }
        }
    }
}