using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class PostAndCloseShipmentOrderRequestParameter
    {
        public IEnumerable<PostItemDestinationsRequestParameter> PickedItemDestinations { get; set; }
    }
}