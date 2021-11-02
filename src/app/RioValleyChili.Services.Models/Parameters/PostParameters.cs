using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class PostParameters : IPostParameters
    {
        public string UserToken { get; set; }
        public string OrderKey { get; set; }
        public IEnumerable<PostItemParameters> PickedItemDestinations { get; set; }
        IEnumerable<IPostItemParameters> IPostParameters.PickedItemDestinations { get { return PickedItemDestinations; } }
    }

    public class PostItemParameters : IPostItemParameters
    {
        public string PickedInventoryItemKey { get; set; }
        public string DestinationLocationKey { get; set; }
    }
}