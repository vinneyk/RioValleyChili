using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class PickedInventoryItemWithDestination : PickedInventoryItem
    {
        public FacilityLocationResponse DestinationLocation { get; set; }
    }
}