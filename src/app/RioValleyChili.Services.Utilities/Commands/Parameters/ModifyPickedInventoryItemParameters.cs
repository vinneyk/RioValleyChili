using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class ModifyPickedInventoryItemParameters
    {
        public int DeltaQuantity { get { return NewQuantity - OriginalQuantity; } }
        public int NewQuantity { get; set; }
        public int OriginalQuantity { get; set; }
        public PickedInventoryItemKey PickedInventoryItemKey { get; set; }
        public InventoryKey InventoryKey { get; set; }
        public LocationKey CurrentLocationKey { get; set; }
        public InventoryPickOrderItemKey OrderItemKey { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }

        public ModifyPickedInventoryItemParameters(IInventoryKey inventoryKey, ILocationKey currentLocationKey, int quantity, string customerLotCode, string customerProductCode, IInventoryPickOrderItemKey orderItemKey = null)
        {
            InventoryKey = inventoryKey.ToInventoryKey();
            CurrentLocationKey = currentLocationKey != null ? currentLocationKey.ToLocationKey() : inventoryKey.ToLocationKey();
            OrderItemKey = orderItemKey == null ? null : orderItemKey.ToInventoryPickOrderItemKey();
            NewQuantity = quantity;
            OriginalQuantity = 0;
            CustomerLotCode = customerLotCode;
            CustomerProductCode = customerProductCode;
        }

        public ModifyPickedInventoryItemParameters(PickedInventoryItem pickedItem)
        {
            if(pickedItem.CurrentLocation == null)
            {
                throw new ArgumentNullException("Expected pickedItem.CurrentLocation but was null, verify navigational property is being included in selection.");
            }
            
            PickedInventoryItemKey = new PickedInventoryItemKey(pickedItem);
            InventoryKey = new InventoryKey(pickedItem, pickedItem, pickedItem, pickedItem, pickedItem.ToteKey);
            CurrentLocationKey = new LocationKey(pickedItem.CurrentLocation);
            OriginalQuantity = pickedItem.Quantity;
            CustomerLotCode = pickedItem.CustomerLotCode;
            CustomerProductCode = pickedItem.CustomerProductCode;
            NewQuantity = 0;
        }

        public ModifyInventoryParameters ToModifySourceInventoryParameters()
        {
            return new ModifyInventoryParameters(InventoryKey, -DeltaQuantity);
        }

        public ModifyInventoryParameters ToModifyDestinationInventoryParameters()
        {
            return new ModifyInventoryParameters(InventoryKey, InventoryKey, CurrentLocationKey, InventoryKey, InventoryKey.InventoryKey_ToteKey, DeltaQuantity);
        }
    }
}