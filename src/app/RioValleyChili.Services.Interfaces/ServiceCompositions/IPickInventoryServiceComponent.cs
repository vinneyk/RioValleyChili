using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces.ServiceCompositions
{
    public interface IPickInventoryServiceComponent
    {
        /// <summary>
        /// Picks inventory items for use with the specified order and removes the specified items from inventory. 
        /// This collection represents the desired final state of the picked inventory items. If previously existing 
        /// picked items are found, they will be updated or removed.
        /// </summary>
        IResult SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters);

        IResult<IPickableInventoryReturn> GetPickableInventoryForContext(FilterInventoryForPickingContextParameters parameters);
    }
}