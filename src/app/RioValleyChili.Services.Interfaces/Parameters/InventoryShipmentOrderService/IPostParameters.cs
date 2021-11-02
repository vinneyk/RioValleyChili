using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService
{
    public interface IPostParameters : IUserIdentifiable
    {
        string OrderKey { get; }
        IEnumerable<IPostItemParameters> PickedItemDestinations { get; }
    }

    public interface IPostItemParameters
    {
        string PickedInventoryItemKey { get; }
        string DestinationLocationKey { get; }
    }
}