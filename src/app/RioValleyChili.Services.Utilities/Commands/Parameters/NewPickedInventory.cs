using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class NewPickedInventory
    {
        public InventoryKey InventoryKey { get; private set; }

        public int QuantityPicked { get; private set;}

        public LocationKey CurrentLocationKey { get; private set; }

        public NewPickedInventory(IInventoryKey inventoryKey, int quantity, ILocationKey currentLocationKey)
        {
            InventoryKey = new InventoryKey(inventoryKey);
            QuantityPicked = quantity;
            CurrentLocationKey = new LocationKey(currentLocationKey);
        }
    }
}
